namespace dExplorer.Editor.Mathematics
{
	/// <summary>
	/// Unit value in the report of the dimension 1 differential equation analysis report.
	/// </summary>
	public struct FloatDEAnalysisValue : IAnalysisValue<float>
	{
		#region Fields
		public float ParameterStep { get; set; }
		public float MeanAbsoluteError { get; set; }
		public float[] SimulationParameters { get; set; }
		public float[] SimulationValues { get; set; }
		#endregion Fields
	}

	/// <summary>
	/// Dimension 1 differential equation analysis report.
	/// </summary>
	public class FloatDEAnalysisReport : DEAnalysisReport<FloatDEAnalysisValue, float> { }
}
