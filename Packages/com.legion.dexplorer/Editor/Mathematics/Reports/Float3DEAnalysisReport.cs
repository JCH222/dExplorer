namespace dExplorer.Editor.Mathematics
{
	using UnityEngine;

	/// <summary>
	/// Unit value in the report of the dimension 3 differential equation analysis report.
	/// </summary>
	public struct Float3DEAnalysisValue : IAnalysisValue<Vector3>
	{
		#region Fields
		public float ParameterStep { get; set; }
		public Vector3 MeanAbsoluteError { get; set; }
		public float[] SimulationParameters { get; set; }
		public Vector3[] SimulationValues { get; set; }
		#endregion Fields
	}

	/// <summary>
	/// Dimension 3 differential equation analysis report.
	/// </summary>
	public class Float3DEAnalysisReport : DEAnalysisReport<Float3DEAnalysisValue, Vector3> { }
}
