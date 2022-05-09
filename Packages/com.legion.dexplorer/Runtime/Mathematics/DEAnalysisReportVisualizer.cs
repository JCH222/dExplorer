namespace dExplorer.Runtime.Mathematics
{
    using System;
	using Unity.Mathematics;
	using UnityEngine;

    public abstract class DEAnalysisReportVisualizer<T_REPORT, T_ANALYSIS_VALUE, T_VARIABLE> : MonoBehaviour
        where T_REPORT : DEAnalysisReport<T_ANALYSIS_VALUE, T_VARIABLE>
        where T_ANALYSIS_VALUE : IAnalysisValue<T_VARIABLE>, new()
        where T_VARIABLE : struct
    {
		#region Local Structures
        private struct ReadingData
		{
            public DESolvingType SolvingType;
            public int ParameterStepIndex;
            public int ParameterIndex;
            public float ParameterStep;
            public float Parameter;
            public T_VARIABLE Variable;
		}
		#endregion Local Structures

		#region Fields
		private T_REPORT _previousReport = null;

        private ReadingData _currentReadingData;

        public T_REPORT Report = null;
        public float PlaybackSpeed = 1.0f;
        public bool Pause = false;
		#endregion Fields

		#region Properties
		public DESolvingType SolvingType { get; protected set; }
        public int ParameterStepIndex { get; protected set; }

        public float FixedUpdateElapsedTime { get; private set; }

        public int ParameterIndex
        {
            get { return _currentReadingData.ParameterIndex; }
        }

        public float Parameter 
        {
            get { return _currentReadingData.Parameter; }
        }

        public float ParameterStep 
        {
            get { return _currentReadingData.ParameterStep; } 
        }

		public T_VARIABLE Variable 
        {
            get { return _currentReadingData.Variable; }
        }
		#endregion Properties

		#region Methods
		protected void OnValidate()
        {
            PlaybackSpeed = math.max(0.0f, PlaybackSpeed);

            if (Report != null)
            {
                if (_previousReport == null || _previousReport != Report)
                {
                    _previousReport = Report;

                    DESolvingType solvingType = SolvingType;
                    int parameterStepIndex = ParameterStepIndex;
                    int parameterIndex = 0;
					float parameter;
					T_VARIABLE variable;

                    Tuple<float, T_VARIABLE> value = Report.GetSimulationValue(solvingType, parameterStepIndex, parameterIndex, out float parameterStep);

                    if (value != null)
                    {
                        FixedUpdateElapsedTime = value.Item1;

                        parameter = value.Item1;
                        variable = value.Item2;
                    }
                    else
                    {
                        FixedUpdateElapsedTime = 0.0f;

                        parameter = 0.0f;
                        variable = new T_VARIABLE();
                    }

                    _currentReadingData = new ReadingData()
                    {
                        SolvingType = solvingType,
                        ParameterStepIndex = parameterStepIndex,
                        ParameterIndex = parameterIndex,
                        ParameterStep = parameterStep,
                        Parameter = parameter,
                        Variable = variable
                    };
                }
            }
            else
            {
                _previousReport = null;

                FixedUpdateElapsedTime = 0.0f;

                _currentReadingData = new ReadingData()
                {
                    SolvingType = SolvingType,
                    ParameterStepIndex = ParameterStepIndex,
                    ParameterIndex = 0,
                    ParameterStep = float.NaN,
                    Parameter = 0.0f,
                    Variable = new T_VARIABLE()
                };
            }
        }

        protected void FixedUpdate()
        {
            if (Report != null && Pause == false)
            {
                FixedUpdateElapsedTime += Time.fixedDeltaTime * PlaybackSpeed;

                int nextParameterIndex = ParameterIndex + 1;
                Tuple<float, T_VARIABLE> nextValue = Report.GetSimulationValue(SolvingType, ParameterStepIndex, nextParameterIndex, out float nextParameterStep);

                ReadingData nextReadingData;

                if (nextValue != null)
				{
                    nextReadingData = new ReadingData()
                    {
                        SolvingType = SolvingType,
                        ParameterStepIndex = ParameterStepIndex,
                        ParameterIndex = nextParameterIndex,
                        ParameterStep = nextParameterStep,
                        Parameter = nextValue.Item1,
                        Variable = nextValue.Item2
                    };
                }
                else
				{
                    nextParameterIndex = 0;
                    nextValue = Report.GetSimulationValue(SolvingType, ParameterStepIndex, nextParameterIndex, out nextParameterStep);

                    FixedUpdateElapsedTime = nextValue.Item1;

                    nextReadingData = new ReadingData()
                    {
                        SolvingType = SolvingType,
                        ParameterStepIndex = ParameterStepIndex,
                        ParameterIndex = 0,
                        ParameterStep = nextParameterStep,
                        Parameter = nextValue.Item1,
                        Variable = nextValue.Item2
                    };

                    _currentReadingData = nextReadingData;
                    return;
                }

                while (nextReadingData.Parameter < FixedUpdateElapsedTime)
                {
                    bool stop = false;

                    _currentReadingData = nextReadingData;

                    nextParameterIndex += 1;
                    nextValue = Report.GetSimulationValue(SolvingType, ParameterStepIndex, nextParameterIndex, out nextParameterStep);

                    if (nextValue == null)
                    {
                        nextParameterIndex = 0;
                        nextValue = Report.GetSimulationValue(SolvingType, ParameterStepIndex, nextParameterIndex, out nextParameterStep);

                        FixedUpdateElapsedTime = nextValue.Item1;
                        stop = true;
                    }

                    nextReadingData = new ReadingData()
                    {
                        SolvingType = SolvingType,
                        ParameterStepIndex = ParameterStepIndex,
                        ParameterIndex = nextParameterIndex,
                        ParameterStep = nextParameterStep,
                        Parameter = nextValue.Item1,
                        Variable = nextValue.Item2
                    };

                    if (stop)
					{
                        _currentReadingData = nextReadingData;
                        return;
                    }
                }
            }
        }
        #endregion Methods
    }
}
