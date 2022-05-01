namespace dExplorer.Editor.Serializations
{
	using System.Xml;
	using UnityEngine;

	public class Float3XmlVariableSerializer : XmlVariableSerializer<Vector3>
	{
		#region Methods
		public override void UpdateSerialization()
		{
			SerializedVariable = Document.CreateElement(string.Empty, VariableName, string.Empty);

			XmlElement xValue = Document.CreateElement(string.Empty, "x", string.Empty);
			xValue.AppendChild(Document.CreateTextNode(Variable.x.ToString()));
			SerializedVariable.AppendChild(xValue);

			XmlElement yValue = Document.CreateElement(string.Empty, "y", string.Empty);
			yValue.AppendChild(Document.CreateTextNode(Variable.y.ToString()));
			SerializedVariable.AppendChild(yValue);

			XmlElement zValue = Document.CreateElement(string.Empty, "z", string.Empty);
			zValue.AppendChild(Document.CreateTextNode(Variable.z.ToString()));
			SerializedVariable.AppendChild(zValue);
		}
		#endregion Methods
	}
}
