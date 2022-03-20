namespace dExplorer.Editor.Serializations
{
	using UnityEngine;

	public class Float2CsvVariableSerializer : CsvVariableSerializer<Vector2>
	{
		#region Methods
		public override void UpdateSerialization()
		{
			SerializedHeader = string.Format("{0} [x]{1}{0} [y]", VariableName, Separator);
			SerializedVariable = string.Format("{0}{1}{2}", Variable.x, Separator, Variable.y);
		}
		#endregion Methods
	}
}
