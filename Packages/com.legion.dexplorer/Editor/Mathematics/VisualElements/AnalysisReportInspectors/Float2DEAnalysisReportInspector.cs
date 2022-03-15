namespace dExplorer.Editor.Mathematics
{
	using dExplorer.Editor.Serializations;
	using UnityEditor;
	using UnityEngine;

	[CustomEditor(typeof(Float2DEAnalysisReport))]
	public class Float2DEAnalysisReportInspector : DEAnalysisReportInspector<Float2DEAnalysisValues, Vector2, Float2XmlVariableSerializer>
	{
		#region Methods
		protected override Vector2 ExtractMeanAbsoluteError(int index)
		{
			return _dataMeanAbsoluteErrors.GetArrayElementAtIndex(index).vector2Value;
		}

		protected override string GetUxmlPath()
		{
			return "Packages/com.legion.dexplorer/Editor/Mathematics/VisualElements/AnalysisReportInspectors/Templates/Float2DEAnalysisReportInspector.uxml"; ;
		}
		#endregion Methods
	}
}
