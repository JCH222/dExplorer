namespace dExplorer.Editor.Mathematics
{
	using dExplorer.Editor.Commons;
	using dExplorer.Runtime.Mathematics;
	using System;
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;
	using UnityEditor;
	using UnityEngine;
	using UnityEngine.UIElements;

	[CustomEditor(typeof(Float2DEAnalysisReport))]
	public class Float2DEAnalysisReportInspector : Editor
	{
		#region Static Fields
		private readonly string UXML_FILE_PATH = "Packages/com.legion.dexplorer/Editor/Mathematics/VisualElements/Float2DEAnalysisReportInspector.uxml";

		private readonly string NAME_TEXT_FIELD_KEY = "name";
		private readonly string DESCRIPTION_VISUALIZER_KEY = "description";
		private readonly string CREATION_DATE_VISUALIZER_KEY = "creation-date";
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

			DateTimeVisualizer dateTimeVisualizer = root.Q<DateTimeVisualizer>(CREATION_DATE_VISUALIZER_KEY);
			dateTimeVisualizer.Year = _creationYear.intValue;
			dateTimeVisualizer.Month = _creationMonth.intValue;
			dateTimeVisualizer.Day = _creationDay.intValue;
			dateTimeVisualizer.Hour = _creationHour.intValue;
			dateTimeVisualizer.Minute = _creationMinute.intValue;
			dateTimeVisualizer.Second = _creationSecond.intValue;
			dateTimeVisualizer.Millisecond = _creationMillisecond.intValue;
			dateTimeVisualizer.Zone = (DateTimeKind)_creationDateTimeZone.enumValueIndex;

			Float2DEAnalysisValues float2DEAnalysisValues = root.Q<Float2DEAnalysisValues>(ANALYSIS_VALUES_KEY);
			for (int i = 0, length = _dataParameterSteps.arraySize; i < length; i++)
			{
				float parameterStep = _dataParameterSteps.GetArrayElementAtIndex(i).floatValue;
				Dictionary<DESolvingType, Vector2> value = null;

				if (float2DEAnalysisValues.ContainsKey(parameterStep))
				{
					value = float2DEAnalysisValues[parameterStep];
				}
				else
				{
					value = new Dictionary<DESolvingType, Vector2>();
					float2DEAnalysisValues[parameterStep, parameterStep.ToString()] = value;
				}

				value.Add(
					(DESolvingType)_dataKeys.GetArrayElementAtIndex(i).enumValueIndex,
					_dataMeanAbsoluteErrors.GetArrayElementAtIndex(i).vector2Value);
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
		#endregion Methods
	}
}
