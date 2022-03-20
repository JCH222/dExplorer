namespace dExplorer.Editor.Serializations
{
	public abstract class CsvVariableSerializer<T_VARIABLE> : IVariableSerializer<T_VARIABLE> where T_VARIABLE : struct
	{
		#region Properties
		public string VariableName { get; internal set; }
		public T_VARIABLE Variable { get; internal set; }
		public string Separator { get; internal set; }
		public string SerializedHeader { get; internal set; }
		public string SerializedVariable { get; internal set; }

		#endregion Properties

		#region Methods
		public abstract void UpdateSerialization();
		#endregion Methods
	}
}
