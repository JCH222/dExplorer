namespace dExplorer.Runtime.Mathematics
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;
	using Unity.Collections;
	using UnityEngine;

	/// <summary>
	/// Base structure of the unit value in the differential equation analysis report.
	/// </summary>
	/// <typeparam name="T_VARIABLE">Variable type</typeparam>
	public interface IAnalysisValue<T_VARIABLE> where T_VARIABLE : struct
	{
		#region Properties
		public float ParameterStep { get; set; }
		public T_VARIABLE MeanAbsoluteError { get; set; }
		public float[] SimulationParameters { get; set; }
		public T_VARIABLE[] SimulationValues { get; set; }
		#endregion Properties
	}

	/// <summary>
	/// Differential equation analysis report.
	/// </summary>
	/// <typeparam name="T_ANALYSIS_VALUE">Analysis value type</typeparam>
	/// <typeparam name="T_VARIABLE">Variable type</typeparam>
	public abstract class DEAnalysisReport<T_ANALYSIS_VALUE, T_VARIABLE> : DEReport
		where T_ANALYSIS_VALUE : IAnalysisValue<T_VARIABLE>, new()
		where T_VARIABLE : struct
	{
		#region Fields
		[HideInInspector] public bool IsFullReport;
		[HideInInspector] public float MinParameter;
		[HideInInspector] public float MaxParameter;

		protected Dictionary<DESolvingType, List<T_ANALYSIS_VALUE>> _data;

		#region Serialization Fields
		[HideInInspector] [SerializeField] private DESolvingType[] _serializedDataKeys;
		[HideInInspector] [SerializeField] private float[] _serializedDataParameterSteps;
		[HideInInspector] [SerializeField] private T_VARIABLE[] _serializedDataMeanAbsoluteErrors;
		[HideInInspector] [SerializeField] private int[] _serializedSimulationSizes;
		[HideInInspector] [SerializeField] private float[] _serializedSimulationsParameters;
		[HideInInspector] [SerializeField] private T_VARIABLE[] _serializedSimulationsValues;
		#endregion Serialization Fields
		#endregion Fields

		#region Constructors
		/// <summary>
		/// Constructor.
		/// </summary>
		public DEAnalysisReport() : base()
		{
			IsFullReport = false;
			MinParameter = 0.0f;
			MaxParameter = 0.0f;

			_data = new Dictionary<DESolvingType, List<T_ANALYSIS_VALUE>>();
		}
		#endregion Constructors

		#region Methods
		/// <summary>
		/// Callback called before Unity serializes the report.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void OnBeforeSerialize()
		{
			base.PreSerialize();

			int arraySize = 0;
			int longArraySize = 0;

			foreach (DESolvingType key in _data.Keys)
			{
				List<T_ANALYSIS_VALUE> analysisValues = _data[key];
				arraySize += analysisValues.Count;

				if (IsFullReport)
				{
					foreach (T_ANALYSIS_VALUE analysisValue in analysisValues)
					{
						longArraySize += analysisValue.SimulationValues.Length;
					}
				}
			}

			_serializedDataKeys = new DESolvingType[arraySize];
			_serializedDataParameterSteps = new float[arraySize];
			_serializedDataMeanAbsoluteErrors = new T_VARIABLE[arraySize];

			if (IsFullReport)
			{
				_serializedSimulationSizes = new int[arraySize];
				_serializedSimulationsParameters = new float[longArraySize];
				_serializedSimulationsValues = new T_VARIABLE[longArraySize];
			}
			else
			{
				_serializedSimulationSizes = null;
				_serializedSimulationsParameters = null;
				_serializedSimulationsValues = null;
			}

			int index = 0;
			int longIndex = 0;

			foreach (DESolvingType key in _data.Keys)
			{
				foreach (T_ANALYSIS_VALUE value in _data[key])
				{
					_serializedDataKeys[index] = key;
					_serializedDataParameterSteps[index] = value.ParameterStep;
					_serializedDataMeanAbsoluteErrors[index] = value.MeanAbsoluteError;

					if (IsFullReport)
					{
						int simulationValueNb = value.SimulationValues.Length;
						_serializedSimulationSizes[index] = simulationValueNb;

						for (int i = 0; i < simulationValueNb; i++)
						{
							_serializedSimulationsParameters[longIndex] = value.SimulationParameters[i];
							_serializedSimulationsValues[longIndex] = value.SimulationValues[i];

							longIndex++;
						}
					}

					index++;
				}
			}
		}

		/// <summary>
		/// Callback called after Unity deserializes the report.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void OnAfterDeserialize()
		{
			base.PostDeserialize();

			_data = new Dictionary<DESolvingType, List<T_ANALYSIS_VALUE>>();

			int longIndex = 0;

			for (int i = 0, length = _serializedDataKeys.Length; i < length; i++)
			{
				DESolvingType key = _serializedDataKeys[i];

				if (_data.ContainsKey(key) == false)
				{
					_data.Add(key, new List<T_ANALYSIS_VALUE>());
				}

				T_VARIABLE[] simulationValues = null;
				float[] simulationParameters = null;

				if (IsFullReport)
				{
					int simulationValueNb = _serializedSimulationSizes[i];
					simulationValues = new T_VARIABLE[simulationValueNb];
					simulationParameters = new float[simulationValueNb];

					for (int j = 0; j < simulationValueNb; j++)
					{
						simulationParameters[j] = _serializedSimulationsParameters[longIndex];
						simulationValues[j] = _serializedSimulationsValues[longIndex];

						longIndex++;
					}
				}

				_data[key].Add(new T_ANALYSIS_VALUE()
				{
					ParameterStep = _serializedDataParameterSteps[i],
					MeanAbsoluteError = _serializedDataMeanAbsoluteErrors[i],
					SimulationParameters = simulationParameters,
					SimulationValues = simulationValues
				});
			}
		}

		/// <summary>
		/// Add new value in the report.
		/// </summary>
		/// <param name="solvingType">Solving type of the simulation</param>
		/// <param name="parameterStep">Parameter step of the simulation</param>
		/// <param name="meanAbsoluteError">Mean absolute error of the simulation</param>
		/// <param name="simulationParameters">Simulation parameters</param>
		/// <param name="simulationValues">Simulation result</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddValue(DESolvingType solvingType, float parameterStep, T_VARIABLE meanAbsoluteError,
			NativeArray<float> simulationParameters, NativeArray<T_VARIABLE> simulationValues)
		{
			if (_data.ContainsKey(solvingType) == false)
			{
				_data.Add(solvingType, new List<T_ANALYSIS_VALUE>());
			}

			_data[solvingType].Add(new T_ANALYSIS_VALUE()
			{
				ParameterStep = parameterStep,
				MeanAbsoluteError = meanAbsoluteError,
				SimulationParameters = IsFullReport ? simulationParameters.ToArray() : null,
				SimulationValues = IsFullReport ? simulationValues.ToArray() : null
			});
		}

		/// <summary>
		/// Get analyzed solving types in the report.
		/// </summary>
		/// <returns>Analyzed solving type in the report</returns>
		public IEnumerable<DESolvingType> GetSolvingTypes()
		{
			foreach (DESolvingType solvingType in _data.Keys)
			{
				yield return solvingType;
			}
		}

		/// <summary>
		/// Get mean absolute errors generated in the report.
		/// </summary>
		/// <param name="solvingType">selected solving type</param>
		/// <returns>Mean absolute errors generated in the report</returns>
		public IEnumerable<Tuple<float, T_VARIABLE>> GetMeanAbsoluteErrors(DESolvingType solvingType)
		{
			List<T_ANALYSIS_VALUE> values = _data[solvingType];

			foreach (T_ANALYSIS_VALUE value in values)
			{
				yield return new Tuple<float, T_VARIABLE>(value.ParameterStep, value.MeanAbsoluteError);
			}
		}

		/// <summary>
		/// Get simulation values generated in the report.
		/// </summary>
		/// <param name="solvingType">Selected solving type</param>
		/// <param name="parameterStepIndex">Selected parameter step index</param>
		/// <returns>Simulation values generated in the report</returns>
		public IEnumerable<Tuple<float, T_VARIABLE>> GetSimulationValues(DESolvingType solvingType, int parameterStepIndex)
		{
			T_ANALYSIS_VALUE analysisValue = _data[solvingType][parameterStepIndex];

			if (analysisValue.SimulationParameters != null)
			{
				for (int i = 0, length = analysisValue.SimulationParameters.Length; i < length; i++)
				{
					yield return new Tuple<float, T_VARIABLE>(analysisValue.SimulationParameters[i], analysisValue.SimulationValues[i]);
				}
			}
		}

		/// <summary>
		/// Get simulation value generated in the report.
		/// </summary>
		/// <param name="solvingType">Selected solving type</param>
		/// <param name="parameterStepIndex">Selected parameter step index</param>
		/// <param name="parameterIndex">Selected parameter index</param>
		/// <param name="parameterStep">Selected parameter step</param>
		/// <returns>Simulation value generated in the report</returns>
		public Tuple<float, T_VARIABLE> GetSimulationValue(DESolvingType solvingType, int parameterStepIndex, int parameterIndex, out float parameterStep)
		{
			T_ANALYSIS_VALUE analysisValue = _data[solvingType][parameterStepIndex];

			if (analysisValue.SimulationParameters != null)
			{
				parameterStep = analysisValue.ParameterStep;

				if (parameterIndex >= 0 && parameterIndex < analysisValue.SimulationParameters.Length)
				{
					return new Tuple<float, T_VARIABLE>(analysisValue.SimulationParameters[parameterIndex], analysisValue.SimulationValues[parameterIndex]);
				}
				else
				{
					// TODO : Add error log
					return null;
				}
			}
			else
			{
				// TODO : Add error log
				parameterStep = float.NaN;
				return null;
			}
		}
		#endregion Methods
	}
}
