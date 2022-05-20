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
    /// <typeparam name="T_ADDITIONAL_VALUE">Additional value type</typeparam>
    public abstract class DESimulationVisualizer<T_CONTAINER, T_VARIABLE, T_ADDITIONAL_VALUE> : MonoBehaviour
        where T_CONTAINER : class
        where T_VARIABLE : struct
        where T_ADDITIONAL_VALUE : struct
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
            public T_ADDITIONAL_VALUE AdditionalValue;
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

        public T_ADDITIONAL_VALUE AdditionalValue
		{
            get { return _currentReadingData.AdditionalValue; }
		}
        #endregion Properties

        #region Methods
        abstract protected Tuple<float, T_VARIABLE, T_ADDITIONAL_VALUE> GetSimulationValue(DESolvingType solvingType, int parameterStepIndex, int parameterIndex, out float parameterStep);

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
                    T_ADDITIONAL_VALUE additionalValue;

                    Tuple<float, T_VARIABLE, T_ADDITIONAL_VALUE> value = GetSimulationValue(solvingType, parameterStepIndex, parameterIndex, out float parameterStep);

                    if (value != null)
                    {
                        FixedUpdateElapsedTime = value.Item1;

                        parameter = value.Item1;
                        variable = value.Item2;
                        additionalValue = value.Item3;
                    }
                    else
                    {
                        FixedUpdateElapsedTime = float.NaN;

                        parameter = float.NaN;
                        variable = new T_VARIABLE();
                        additionalValue = new T_ADDITIONAL_VALUE();
                    }

                    _currentReadingData = new ReadingData()
                    {
                        SolvingType = solvingType,
                        ParameterStepIndex = parameterStepIndex,
                        ParameterIndex = parameterIndex,
                        ParameterStep = parameterStep,
                        Parameter = parameter,
                        Variable = variable,
                        AdditionalValue = additionalValue
                    };
                }
            }
            else
            {
                _previousContainer = null;

                FixedUpdateElapsedTime = float.NaN;

                _currentReadingData = new ReadingData()
                {
                    SolvingType = SolvingType,
                    ParameterStepIndex = ParameterStepIndex,
                    ParameterIndex = 0,
                    ParameterStep = float.NaN,
                    Parameter = float.NaN,
                    Variable = new T_VARIABLE(),
                    AdditionalValue = new T_ADDITIONAL_VALUE()
                };
            }
        }

        protected void FixedUpdate()
        {
            if (Container != null && Pause == false)
            {
                FixedUpdateElapsedTime += Time.fixedDeltaTime * PlaybackSpeed;

                int nextParameterIndex = ParameterIndex + 1;
                Tuple<float, T_VARIABLE, T_ADDITIONAL_VALUE> nextValue = GetSimulationValue(SolvingType, ParameterStepIndex, nextParameterIndex, out float nextParameterStep);

                if (nextValue != null)
				{
                    ReadingData nextReadingData = new ReadingData()
                    {
                        SolvingType = SolvingType,
                        ParameterStepIndex = ParameterStepIndex,
                        ParameterIndex = nextParameterIndex,
                        ParameterStep = nextParameterStep,
                        Parameter = nextValue.Item1,
                        Variable = nextValue.Item2,
                        AdditionalValue = nextValue.Item3
                    };

                    while (nextReadingData.Parameter < FixedUpdateElapsedTime)
                    {
                        _currentReadingData = nextReadingData;

                        nextParameterIndex += 1;
                        nextValue = GetSimulationValue(SolvingType, ParameterStepIndex, nextParameterIndex, out nextParameterStep);

                        if (nextValue != null)
                        {
                            nextReadingData = new ReadingData()
                            {
                                SolvingType = SolvingType,
                                ParameterStepIndex = ParameterStepIndex,
                                ParameterIndex = nextParameterIndex,
                                ParameterStep = nextParameterStep,
                                Parameter = nextValue.Item1,
                                Variable = nextValue.Item2,
                                AdditionalValue = nextValue.Item3
                            };
                        }
                        else
						{
                            break;
						}
                    }
                }
            }
        }
        #endregion Methods
    }
}
