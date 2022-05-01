namespace dExplorer.Editor.Mathematics
{
	using dExplorer.Editor.Serializations;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Float 3 analysis report visualizer.
	/// </summary>
	[CustomEditor(typeof(Float3DEAnalysisReport))]
	public class Float3DEAnalysisReportInspector : DEAnalysisReportInspector<Float3DEAnalysisValues, Vector3, Float3XmlVariableSerializer, Float3CsvVariableSerializer>
	{
		#region Methods
		protected override string GetUxmlPath()
		{
			return "Packages/com.legion.dexplorer/Editor/Mathematics/VisualElements/AnalysisReportInspectors/Templates/Float3DEAnalysisReportInspector.uxml"; ;
		}

		protected override Vector3 ExtractMeanAbsoluteError(int index)
		{
			return _dataMeanAbsoluteErrors.GetArrayElementAtIndex(index).vector3Value;
		}
		#endregion Methods
	}
}
