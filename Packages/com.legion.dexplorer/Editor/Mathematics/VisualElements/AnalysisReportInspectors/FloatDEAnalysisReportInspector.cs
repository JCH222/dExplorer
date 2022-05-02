namespace dExplorer.Editor.Mathematics
{
	using dExplorer.Editor.Serializations;
	using UnityEditor;

	/// <summary>
	/// Float analysis report visualizer.
	/// </summary>
	[CustomEditor(typeof(FloatDESerializableAnalysisReport))]
	public class FloatDEAnalysisReportInspector : DEAnalysisReportInspector<FloatDEAnalysisValues, float, FloatXmlVariableSerializer, FloatCsvVariableSerializer>
	{
		#region Methods
		protected override string GetUxmlPath()
		{
			return "Packages/com.legion.dexplorer/Editor/Mathematics/VisualElements/AnalysisReportInspectors/Templates/FloatDEAnalysisReportInspector.uxml";
		}

		protected override float ExtractMeanAbsoluteError(int index)
		{
			return _dataMeanAbsoluteErrors.GetArrayElementAtIndex(index).floatValue;
		}
		#endregion Methods
	}
}
