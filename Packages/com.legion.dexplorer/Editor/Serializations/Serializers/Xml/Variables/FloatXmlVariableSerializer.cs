namespace dExplorer.Editor.Serializations
{
	using System.Xml;

	public class FloatXmlVariableSerializer : XmlVariableSerializer<float>
	{
		#region Methods
		public override void UpdateSerialization()
		{
			SerializedVariable = Document.CreateElement(string.Empty, VariableName, string.Empty);
			SerializedVariable.AppendChild(Document.CreateTextNode(Variable.ToString()));
		}
		#endregion Methods
	}
}
