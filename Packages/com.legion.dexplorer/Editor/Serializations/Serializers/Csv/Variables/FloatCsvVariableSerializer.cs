namespace dExplorer.Editor.Serializations
{
	public class FloatCsvVariableSerializer : CsvVariableSerializer<float>
	{
		#region Methods
		public override void UpdateSerialization()
		{
			SerializedHeader = VariableName;
			SerializedVariable = Variable.ToString();
		}
		#endregion Methods
	}
}
