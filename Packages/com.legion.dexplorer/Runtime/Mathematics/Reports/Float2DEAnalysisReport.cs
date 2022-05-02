namespace dExplorer.Runtime.Mathematics
{
	using UnityEngine;

	/// <summary>
	/// Unit value in the report of the dimension 2 differential equation analysis report.
	/// </summary>
	public struct Float2DEAnalysisValue : IAnalysisValue<Vector2>
	{
		#region Fields
		public float ParameterStep { get; set; }
		public Vector2 MeanAbsoluteError { get; set; }
		public float[] SimulationParameters { get; set; }
		public Vector2[] SimulationValues { get; set; }
		#endregion Fields
	}

	/// <summary>
	/// Dimension 2 differential equation analysis report.
	/// </summary>
	public class Float2DEAnalysisReport : DEAnalysisReport<Float2DEAnalysisValue, Vector2> { }
}
