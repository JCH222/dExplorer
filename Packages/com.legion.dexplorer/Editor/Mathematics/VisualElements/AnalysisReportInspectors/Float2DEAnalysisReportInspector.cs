namespace dExplorer.Editor.Mathematics
{
	using dExplorer.Editor.Serializations;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Float 2 analysis report visualizer.
	/// </summary>
	[CustomEditor(typeof(Float2DEAnalysisReport))]
	public class Float2DEAnalysisReportInspector : DEAnalysisReportInspector<Float2DEAnalysisValues, Vector2, Float2XmlVariableSerializer, Float2CsvVariableSerializer>
	{
		#region Methods
		protected override string GetUxmlPath()
		{
			return "Packages/com.legion.dexplorer/Editor/Mathematics/VisualElements/AnalysisReportInspectors/Templates/Float2DEAnalysisReportInspector.uxml"; ;
		}

		protected override Vector2 ExtractMeanAbsoluteError(int index)
		{
			return _dataMeanAbsoluteErrors.GetArrayElementAtIndex(index).vector2Value;
		}
		#endregion Methods
	}
}
