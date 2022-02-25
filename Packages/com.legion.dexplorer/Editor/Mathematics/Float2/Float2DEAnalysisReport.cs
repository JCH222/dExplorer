namespace dExplorer.Editor.Mathematics
{
	using dExplorer.Runtime.Mathematics;
	using System;
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;
	using Unity.Collections;
	using Unity.Mathematics;
	using UnityEngine;

	/// <summary>
	/// Dimension 2 differential equation analysis report.
	/// </summary>
	public class Float2DEAnalysisReport : ScriptableObject, ISerializationCallbackReceiver
	{
		#region Structs
		/// <summary>
		/// Unit value of the report.
		/// </summary>
		private struct Float2DEAnalysisValue
		{
			#region Fields
			public float ParameterStep;
			public float2 MeanAbsoluteError;
			public float2[] SimulationValues;
			#endregion Fields
		}
		#endregion Structs

		#region Fields
		public string Name;
		public string ShortDescription;
		public string LongDescription;

		[HideInInspector] public bool IsFullReport;
		[HideInInspector] public float MinParameter;
		[HideInInspector] public float MaxParameter;

		private DateTime _creationDateTime;
		private Dictionary<DESolvingType, List<Float2DEAnalysisValue>> _data;

		#region Serialization Fields
		[HideInInspector] [SerializeField] private int _serializedCreationYear = 0;
		[HideInInspector] [SerializeField] private int _serializedCreationMonth = 0;
		[HideInInspector] [SerializeField] private int _serializedCreationDay = 0;
		[HideInInspector] [SerializeField] private int _serializedCreationHour = 0;
		[HideInInspector] [SerializeField] private int _serializedCreationMinute = 0;
		[HideInInspector] [SerializeField] private int _serializedCreationSecond = 0;
		[HideInInspector] [SerializeField] private int _serializedCreationMillisecond = 0;
		[HideInInspector] [SerializeField] private DateTimeKind _serializedCreationDateTimeZone = DateTimeKind.Unspecified;

		[HideInInspector] [SerializeField] private DESolvingType[] _serializedDataKeys;
		[HideInInspector] [SerializeField] private float[] _serializedDataParameterSteps;
		[HideInInspector] [SerializeField] private Vector2[] _serializedDataMeanAbsoluteErrors;
		[HideInInspector] [SerializeField] private int[] _serializedSimulationSizes;
		[HideInInspector] [SerializeField] private Vector2[] _serializedSimulationsValues;
		#endregion Serialization Fields
		#endregion Fields

		#region Constructors
		/// <summary>
		/// Constructor.
		/// </summary>
		public Float2DEAnalysisReport() : base()
		{
			Name = string.Empty;
			ShortDescription = string.Empty;
			LongDescription = string.Empty;

			IsFullReport = false;
			MinParameter = 0.0f;
			MaxParameter = 0.0f;

			_creationDateTime = DateTime.UtcNow;
			_data = new Dictionary<DESolvingType, List<Float2DEAnalysisValue>>();
		}
		#endregion Constructors

		#region Methods
		/// <summary>
		/// Callback called before Unity serializes the report.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void OnBeforeSerialize()
		{
			_serializedCreationYear = _creationDateTime.Year;
			_serializedCreationMonth = _creationDateTime.Month;
			_serializedCreationDay = _creationDateTime.Day;
			_serializedCreationHour = _creationDateTime.Hour;
			_serializedCreationMinute = _creationDateTime.Minute;
			_serializedCreationSecond = _creationDateTime.Second;
			_serializedCreationMillisecond = _creationDateTime.Millisecond;
			_serializedCreationDateTimeZone = _creationDateTime.Kind;

			int arraySize = 0;
			int longArraySize = 0;

			foreach (DESolvingType key in _data.Keys)
			{
				List<Float2DEAnalysisValue> analysisValues = _data[key];
				arraySize += _data[key].Count;

				if (IsFullReport)
				{
					foreach (Float2DEAnalysisValue analysisValue in analysisValues)
					{
						longArraySize += analysisValue.SimulationValues.Length;
					}
				}
			}

			_serializedDataKeys = new DESolvingType[arraySize];
			_serializedDataParameterSteps = new float[arraySize];
			_serializedDataMeanAbsoluteErrors = new Vector2[arraySize];

			if (IsFullReport)
			{
				_serializedSimulationSizes = new int[arraySize];
				_serializedSimulationsValues = new Vector2[longArraySize];
			}
			else
			{
				_serializedSimulationSizes = null;
				_serializedSimulationsValues = null;
			}

			int index = 0;
			int longIndex = 0;

			foreach (DESolvingType key in _data.Keys)
			{
				foreach (Float2DEAnalysisValue value in _data[key])
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
		public void OnAfterDeserialize()
		{
			_creationDateTime = new DateTime(_serializedCreationYear, _serializedCreationMonth, _serializedCreationDay, _serializedCreationHour,
				_serializedCreationMinute, _serializedCreationSecond, _serializedCreationMillisecond, _serializedCreationDateTimeZone);

			_data = new Dictionary<DESolvingType, List<Float2DEAnalysisValue>>();

			int longIndex = 0;

			for (int i = 0, length = _serializedDataKeys.Length; i < length; i++)
			{
				DESolvingType key = _serializedDataKeys[i];

				if (_data.ContainsKey(key) == false)
				{
					_data.Add(key, new List<Float2DEAnalysisValue>());
				}

				float2[] simulationValues = null;

				if (IsFullReport)
				{
					int simulationValueNb = _serializedSimulationSizes[i];
					simulationValues = new float2[simulationValueNb];

					for (int j = 0; j < simulationValueNb; j++)
					{
						simulationValues[j] = _serializedSimulationsValues[longIndex];
						longIndex++;
					}
				}

				_data[key].Add(new Float2DEAnalysisValue()
				{
					ParameterStep = _serializedDataParameterSteps[i],
					MeanAbsoluteError = _serializedDataMeanAbsoluteErrors[i],
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
		/// <param name="simulationValues">Simulation result</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddValue(DESolvingType solvingType, float parameterStep, float2 meanAbsoluteError, NativeArray<float2> simulationValues)
		{
			if (_data.ContainsKey(solvingType) == false)
			{
				_data.Add(solvingType, new List<Float2DEAnalysisValue>());
			}

			_data[solvingType].Add(new Float2DEAnalysisValue()
			{
				ParameterStep = parameterStep,
				MeanAbsoluteError = meanAbsoluteError,
				SimulationValues = IsFullReport ? simulationValues.ToArray() : null
			});
		}
		#endregion Methods
	}
}