namespace dExplorer.Editor.Serializations
{
	using System.Xml;

	public abstract class XmlVariableSerializer<T_VARIABLE> : IVariableSerializer<T_VARIABLE> where T_VARIABLE : struct
	{
		#region Properties
		internal XmlDocument Document { get; set; }
		public string VariableName { get; internal set; }
		public T_VARIABLE Variable { get; internal set; }
		public XmlElement SerializedVariable { get; protected set; }

		#endregion Properties

		#region Methods
		public abstract void UpdateSerialization();
		#endregion Methods
	}
}
