namespace dExplorer.Runtime.Mathematics
{
    using System;
	using Unity.Mathematics;
	using UnityEngine;

    /// <summary>
    /// Base class for simulation runtime diplay.
    /// </summary>
    /// <typeparam name="T_CONTAINER">Simulation container type</typeparam>
    /// <typeparam name="T_VARIABLE">Variable type</typeparam>
    public abstract class DESimulationVisualizer<T_CONTAINER, T_VARIABLE> : MonoBehaviour
        where T_CONTAINER : class
        where T_VARIABLE : struct
    {
		#region Local Structures
        /// <summary>
        /// Unit value of the simulation display.
        /// </summary>
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
		private T_CONTAINER _previousContainer = null;

        private ReadingData _currentReadingData;

        public T_CONTAINER Container = null;
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
        abstract protected Tuple<float, T_VARIABLE> GetSimulationValue(DESolvingType solvingType, int parameterStepIndex, int parameterIndex, out float parameterStep);

		protected void OnValidate()
        {
            PlaybackSpeed = math.max(0.0f, PlaybackSpeed);

            if (Container != null)
            {
                if (_previousContainer == null || _previousContainer != Container)
                {
                    _previousContainer = Container;

                    DESolvingType solvingType = SolvingType;
                    int parameterStepIndex = ParameterStepIndex;
                    int parameterIndex = 0;
					float parameter;
					T_VARIABLE variable;

                    Tuple<float, T_VARIABLE> value = GetSimulationValue(solvingType, parameterStepIndex, parameterIndex, out float parameterStep);

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
                _previousContainer = null;

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
            if (Container != null && Pause == false)
            {
                FixedUpdateElapsedTime += Time.fixedDeltaTime * PlaybackSpeed;

                int nextParameterIndex = ParameterIndex + 1;
                Tuple<float, T_VARIABLE> nextValue = GetSimulationValue(SolvingType, ParameterStepIndex, nextParameterIndex, out float nextParameterStep);

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
                    nextValue = GetSimulationValue(SolvingType, ParameterStepIndex, nextParameterIndex, out nextParameterStep);

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
                    nextValue = GetSimulationValue(SolvingType, ParameterStepIndex, nextParameterIndex, out nextParameterStep);

                    if (nextValue == null)
                    {
                        nextParameterIndex = 0;
                        nextValue = GetSimulationValue(SolvingType, ParameterStepIndex, nextParameterIndex, out nextParameterStep);

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
