namespace dExplorer.Editor.Serializations
{
	using UnityEngine;

	public class Float3CsvVariableSerializer : CsvVariableSerializer<Vector3>
	{
		#region Methods
		public override void UpdateSerialization()
		{
			SerializedHeader = string.Format("{0} [x]{1}{0} [y]{1}{0} [z]", VariableName, Separator);
			SerializedVariable = string.Format("{1}{0}{2}{0}{3}", Separator, Variable.x, Variable.y, Variable.z);
		}
		#endregion Methods
	}
}
