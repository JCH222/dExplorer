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

	[CustomEditor(typeof(FloatDEAnalysisReport))]
	public class FloatDEAnalysisReportInspector : Editor
	{
		#region Static Fields
		private readonly string UXML_FILE_PATH = "Packages/com.legion.dexplorer/Editor/Mathematics/VisualElements/FloatDEAnalysisReportInspector.uxml";

		private readonly string NAME_TEXT_FIELD_KEY = "name";
		private readonly string DESCRIPTION_VISUALIZER_KEY = "description";
		private readonly string CREATION_DATE_VISUALIZER_KEY = "creation-date";
		private readonly string MIN_PARAMETER_FLOAT_FIELD_KEY = "min-parameter";
		private readonly string MAX_PARAMETER_FLOAT_FIELD_KEY = "max-parameter";
		private readonly string ANALYSIS_VALUES_KEY = "analysis_values";
		#endregion Static Fields

		#region Fields
		private SerializedProperty _name;
		private SerializedProperty _shortDescription;
		private SerializedProperty _longDescription;
		private SerializedProperty _creationYear;
		private SerializedProperty _creationMonth;
		private SerializedProperty _creationDay;
		private SerializedProperty _creationHour;
		private SerializedProperty _creationMinute;
		private SerializedProperty _creationSecond;
		private SerializedProperty _creationMillisecond;
		private SerializedProperty _creationDateTimeZone;
		private SerializedProperty _minParameter;
		private SerializedProperty _maxParameter;
		private SerializedProperty _dataKeys;
		private SerializedProperty _dataParameterSteps;
		private SerializedProperty _dataMeanAbsoluteErrors;
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

			_minParameter = serializedObject.FindProperty("MinParameter");
			_maxParameter = serializedObject.FindProperty("MaxParameter");
			_dataKeys = serializedObject.FindProperty("_serializedDataKeys");
			_dataParameterSteps = serializedObject.FindProperty("_serializedDataParameterSteps");
			_dataMeanAbsoluteErrors = serializedObject.FindProperty("_serializedDataMeanAbsoluteErrors");
		}

		public override VisualElement CreateInspectorGUI()
		{
			VisualTreeAsset treeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_FILE_PATH);
			VisualElement root = treeAsset.CloneTree();

			TextField nameField = root.Q<TextField>(NAME_TEXT_FIELD_KEY);
			nameField.value = _name.stringValue;
			nameField.RegisterValueChangedCallback(OnNameChanged);

			DescriptionVisualizer descriptionVisualizer = root.Q<DescriptionVisualizer>(DESCRIPTION_VISUALIZER_KEY);
			descriptionVisualizer.ShortDescription = _shortDescription.stringValue;
			descriptionVisualizer.RegisterShortDescriptionValueChangedCallback(OnShortDescriptionChanged);
			descriptionVisualizer.LongDescription = _longDescription.stringValue;
			descriptionVisualizer.RegisterLongDescriptionValueChangedCallback(OnLongDescriptionChanged);

			FloatField minParameterField = root.Q<FloatField>(MIN_PARAMETER_FLOAT_FIELD_KEY);
			minParameterField.SetEnabled(false);
			minParameterField.value = _minParameter.floatValue;
			minParameterField.RegisterValueChangedCallback(OnMinParameterChanged);

			FloatField maxParameterField = root.Q<FloatField>(MAX_PARAMETER_FLOAT_FIELD_KEY);
			maxParameterField.SetEnabled(false);
			maxParameterField.value = _maxParameter.floatValue;
			maxParameterField.RegisterValueChangedCallback(OnMaxParameterChanged);

			DateTimeVisualizer dateTimeVisualizer = root.Q<DateTimeVisualizer>(CREATION_DATE_VISUALIZER_KEY);
			dateTimeVisualizer.Year = _creationYear.intValue;
			dateTimeVisualizer.Month = _creationMonth.intValue;
			dateTimeVisualizer.Day = _creationDay.intValue;
			dateTimeVisualizer.Hour = _creationHour.intValue;
			dateTimeVisualizer.Minute = _creationMinute.intValue;
			dateTimeVisualizer.Second = _creationSecond.intValue;
			dateTimeVisualizer.Millisecond = _creationMillisecond.intValue;
			dateTimeVisualizer.Zone = (DateTimeKind)_creationDateTimeZone.enumValueIndex;

			FloatDEAnalysisValues floatDEAnalysisValues = root.Q<FloatDEAnalysisValues>(ANALYSIS_VALUES_KEY);
			for (int i = 0, length = _dataParameterSteps.arraySize; i < length; i++)
			{
				float parameterStep = _dataParameterSteps.GetArrayElementAtIndex(i).floatValue;
				Dictionary<DESolvingType, float> value = null;

				if (floatDEAnalysisValues.ContainsKey(parameterStep))
				{
					value = floatDEAnalysisValues[parameterStep];
				}
				else
				{
					value = new Dictionary<DESolvingType, float>();
					floatDEAnalysisValues[parameterStep, parameterStep.ToString()] = value;
				}

				value.Add(
					(DESolvingType)_dataKeys.GetArrayElementAtIndex(i).enumValueIndex,
					_dataMeanAbsoluteErrors.GetArrayElementAtIndex(i).floatValue);
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void OnMinParameterChanged(ChangeEvent<float> evt)
		{
			_minParameter.floatValue = evt.newValue;
			serializedObject.ApplyModifiedProperties();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void OnMaxParameterChanged(ChangeEvent<float> evt)
		{
			_maxParameter.floatValue = evt.newValue;
			serializedObject.ApplyModifiedProperties();
		}
		#endregion Methods
	}
}
