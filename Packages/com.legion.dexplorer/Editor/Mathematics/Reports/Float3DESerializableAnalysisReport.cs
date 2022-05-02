namespace dExplorer.Editor.Mathematics
{
	using dExplorer.Editor.Serializations;
	using dExplorer.Runtime.Mathematics;
	using UnityEngine;

	/// <summary>
	/// Dimension 3 differential equation analysis report with serialization options.
	/// </summary>
	public class Float3DESerializableAnalysisReport : Float3DEAnalysisReport, IDEAnalysisReportSerializable<Vector3> { }
}
