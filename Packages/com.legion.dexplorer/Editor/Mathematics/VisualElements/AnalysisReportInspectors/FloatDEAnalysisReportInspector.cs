namespace dExplorer.Editor.Mathematics
{
	using dExplorer.Editor.Serializations;
	using UnityEditor;

	[CustomEditor(typeof(FloatDEAnalysisReport))]
	public class FloatDEAnalysisReportInspector : DEAnalysisReportInspector<FloatDEAnalysisValues, float, FloatXmlVariableSerializer, FloatCsvVariableSerializer>
	{
		#region Methods
		protected override float ExtractMeanAbsoluteError(int index)
		{
			return _dataMeanAbsoluteErrors.GetArrayElementAtIndex(index).floatValue;
		}

		protected override string GetUxmlPath()
		{
			return "Packages/com.legion.dexplorer/Editor/Mathematics/VisualElements/AnalysisReportInspectors/Templates/FloatDEAnalysisReportInspector.uxml";
		}
		#endregion Methods
	}
}
