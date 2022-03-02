namespace dExplorer.Editor.Mathematics
{
	using dExplorer.Editor.Commons;
	using dExplorer.Runtime.Mathematics;
	using System;
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;
	using UnityEditor;
	using UnityEditor.UIElements;
	using UnityEngine.UIElements;

	public abstract class DEAnalysisReportInspector<T_ANALYSIS_VALUES, T_VARIABLE> : Editor
		where T_ANALYSIS_VALUES : DEAnalysisValues<T_VARIABLE>
		where T_VARIABLE : struct
	{
		#region Static Fields
		private readonly string NAME_TEXT_FIELD_KEY = "name";
		private readonly string DESCRIPTION_VISUALIZER_KEY = "description";
		private readonly string CREATION_DATE_VISUALIZER_KEY = "creation-date";
		private readonly string IS_FULL_REPORT_FIELD_KEY = "is-full-report";
		private readonly string MIN_PARAMETER_FLOAT_FIELD_KEY = "min-parameter";
		private readonly string MAX_PARAMETER_FLOAT_FIELD_KEY = "max-parameter";
		private readonly string ANALYSIS_VALUES_KEY = "analysis-values";
		#endregion Static Fields

		#region Fields
		protected SerializedProperty _name;
		protected SerializedProperty _shortDescription;
		protected SerializedProperty _longDescription;
		protected SerializedProperty _creationYear;
		protected SerializedProperty _creationMonth;
		protected SerializedProperty _creationDay;
		protected SerializedProperty _creationHour;
		protected SerializedProperty _creationMinute;
		protected SerializedProperty _creationSecond;
		protected SerializedProperty _creationMillisecond;
		protected SerializedProperty _creationDateTimeZone;
		protected SerializedProperty _isFullReport;
		protected SerializedProperty _minParameter;
		protected SerializedProperty _maxParameter;
		protected SerializedProperty _dataKeys;
		protected SerializedProperty _dataParameterSteps;
		protected SerializedProperty _dataMeanAbsoluteErrors;
		#endregion Fields

		#region Methods
		private void OnEnable()
		{
			_name = serializedObject.FindProperty("Name");
			_shortDescription = serializedObject.FindProperty("ShortDescription");
			_longDescription = serializedObject.FindProperty("LongDescription");

			_creationYear = serializedObject.FindProperty("_serializedCreationYear");
			_creationMonth = serializedObject.FindProperty("_serializedCreationMonth");
			_creationDay = serializedObject.FindProperty("_serializedCreationDay");
			_creationHour = serializedObject.FindProperty("_serializedCreationHour");
			_creationMinute = serializedObject.FindProperty("_serializedCreationMinute");
			_creationSecond = serializedObject.FindProperty("_serializedCreationSecond");
			_creationMillisecond = serializedObject.FindProperty("_serializedCreationMillisecond");
			_creationDateTimeZone = serializedObject.FindProperty("_serializedCreationDateTimeZone");

			_isFullReport = serializedObject.FindProperty("IsFullReport");
			_minParameter = serializedObject.FindProperty("MinParameter");
			_maxParameter = serializedObject.FindProperty("MaxParameter");

			_dataKeys = serializedObject.FindProperty("_serializedDataKeys");
			_dataParameterSteps = serializedObject.FindProperty("_serializedDataParameterSteps");
			_dataMeanAbsoluteErrors = serializedObject.FindProperty("_serializedDataMeanAbsoluteErrors");
		}

		public override VisualElement CreateInspectorGUI()
		{
			VisualTreeAsset treeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(GetUxmlPath());
			VisualElement root = treeAsset.CloneTree();

			TextField nameField = root.Q<TextField>(NAME_TEXT_FIELD_KEY);
			nameField.value = _name.stringValue;
			nameField.RegisterValueChangedCallback(OnNameChanged);

			DescriptionVisualizer descriptionVisualizer = root.Q<DescriptionVisualizer>(DESCRIPTION_VISUALIZER_KEY);
			descriptionVisualizer.ShortDescription = _shortDescription.stringValue;
			descriptionVisualizer.RegisterShortDescriptionValueChangedCallback(OnShortDescriptionChanged);
			descriptionVisualizer.LongDescription = _longDescription.stringValue;
			descriptionVisualizer.RegisterLongDescriptionValueChangedCallback(OnLongDescriptionChanged);

			Toggle isFullReportField = root.Q<Toggle>(IS_FULL_REPORT_FIELD_KEY);
			isFullReportField.SetEnabled(false);
			isFullReportField.value = _isFullReport.boolValue;

			FloatField minParameterField = root.Q<FloatField>(MIN_PARAMETER_FLOAT_FIELD_KEY);
			minParameterField.SetEnabled(false);
			minParameterField.value = _minParameter.floatValue;

			FloatField maxParameterField = root.Q<FloatField>(MAX_PARAMETER_FLOAT_FIELD_KEY);
			maxParameterField.SetEnabled(false);
			maxParameterField.value = _maxParameter.floatValue;

			DateTimeVisualizer dateTimeVisualizer = root.Q<DateTimeVisualizer>(CREATION_DATE_VISUALIZER_KEY);
			dateTimeVisualizer.Year = _creationYear.intValue;
			dateTimeVisualizer.Month = _creationMonth.intValue;
			dateTimeVisualizer.Day = _creationDay.intValue;
			dateTimeVisualizer.Hour = _creationHour.intValue;
			dateTimeVisualizer.Minute = _creationMinute.intValue;
			dateTimeVisualizer.Second = _creationSecond.intValue;
			dateTimeVisualizer.Millisecond = _creationMillisecond.intValue;
			dateTimeVisualizer.Zone = (DateTimeKind)_creationDateTimeZone.enumValueIndex;

			T_ANALYSIS_VALUES analysisValues = root.Q<T_ANALYSIS_VALUES>(ANALYSIS_VALUES_KEY);
			for (int i = 0, length = _dataParameterSteps.arraySize; i < length; i++)
			{
				float parameterStep = _dataParameterSteps.GetArrayElementAtIndex(i).floatValue;
				Dictionary<DESolvingType, T_VARIABLE> value = null;

				if (analysisValues.ContainsKey(parameterStep))
				{
					value = analysisValues[parameterStep];
				}
				else
				{
					value = new Dictionary<DESolvingType, T_VARIABLE>();
					analysisValues[parameterStep, parameterStep.ToString()] = value;
				}

				value.Add(
					(DESolvingType)_dataKeys.GetArrayElementAtIndex(i).enumValueIndex,
					ExtractMeanAbsoluteError(i));
			}

			return root;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void OnNameChanged(ChangeEvent<string> evt)
		{
			_name.stringValue = evt.newValue;
			serializedObject.ApplyModifiedProperties();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void OnShortDescriptionChanged(ChangeEvent<string> evt)
		{
			_shortDescription.stringValue = evt.newValue;
			serializedObject.ApplyModifiedProperties();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void OnLongDescriptionChanged(ChangeEvent<string> evt)
		{
			_longDescription.stringValue = evt.newValue;
			serializedObject.ApplyModifiedProperties();
		}

		protected abstract string GetUxmlPath();

		protected abstract T_VARIABLE ExtractMeanAbsoluteError(int index);
		#endregion Methods
	}
}
