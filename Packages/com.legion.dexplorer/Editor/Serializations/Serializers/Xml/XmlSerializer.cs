namespace dExplorer.Editor.Serializations
{
	using dExplorer.Runtime.Mathematics;
	using System;
	using System.Text;
	using System.Xml;

	public class XmlSerializer<T_VARIABLE, T_VARIABLE_SERIALIZER> : ISerializer<T_VARIABLE, T_VARIABLE_SERIALIZER>
		where T_VARIABLE : struct
		where T_VARIABLE_SERIALIZER : XmlVariableSerializer<T_VARIABLE>, new()
	{
		#region Fields
		private readonly XmlVariableSerializer<T_VARIABLE> _variableSerializer;
		private readonly string _exportFilePath;
		private readonly XmlWriterSettings _settings ;
		#endregion Fields

		#region Constructors
		public XmlSerializer(string exportFilePath, Encoding encoding, bool indent)
		{
			_variableSerializer = new T_VARIABLE_SERIALIZER()
			{
				Document = null,
				VariableName = string.Empty,
				Variable = new T_VARIABLE(),
			};

			_exportFilePath = exportFilePath;

			_settings = new XmlWriterSettings()
			{
				Encoding = encoding,
				Indent = indent
			};
		}
		#endregion Properties

		#region Methods
		public void Serialize(IDEAnalysisReportSerializable<T_VARIABLE> report)
		{
			XmlWriter writer = XmlWriter.Create(_exportFilePath, _settings);
			XmlDocument document = new XmlDocument();
			_variableSerializer.Document = document;

			XmlDeclaration declaration = document.CreateXmlDeclaration("1.0", _settings.Encoding.ToString(), null);
			XmlElement root = document.DocumentElement;
			document.InsertBefore(declaration, root);

			XmlElement body = document.CreateElement(string.Empty, "body", string.Empty);
			document.AppendChild(body);

			XmlElement name = document.CreateElement(string.Empty, "name", string.Empty);
			name.AppendChild(document.CreateTextNode(report.GetName()));
			body.AppendChild(name);

			XmlElement creationDate = document.CreateElement(string.Empty, "creation-date", string.Empty);
			body.AppendChild(creationDate);

			XmlElement year = document.CreateElement(string.Empty, "year", string.Empty);
			year.AppendChild(document.CreateTextNode(report.GetCreationDate().Year.ToString()));
			creationDate.AppendChild(year);

			XmlElement month = document.CreateElement(string.Empty, "month", string.Empty);
			month.AppendChild(document.CreateTextNode(report.GetCreationDate().Month.ToString()));
			creationDate.AppendChild(month);

			XmlElement day = document.CreateElement(string.Empty, "day", string.Empty);
			day.AppendChild(document.CreateTextNode(report.GetCreationDate().Day.ToString()));
			creationDate.AppendChild(day);

			XmlElement hour = document.CreateElement(string.Empty, "hour", string.Empty);
			hour.AppendChild(document.CreateTextNode(report.GetCreationDate().Hour.ToString()));
			creationDate.AppendChild(hour);

			XmlElement minute = document.CreateElement(string.Empty, "minute", string.Empty);
			minute.AppendChild(document.CreateTextNode(report.GetCreationDate().Minute.ToString()));
			creationDate.AppendChild(minute);

			XmlElement second = document.CreateElement(string.Empty, "second", string.Empty);
			second.AppendChild(document.CreateTextNode(report.GetCreationDate().Second.ToString()));
			creationDate.AppendChild(second);

			XmlElement millisecond = document.CreateElement(string.Empty, "millisecond", string.Empty);
			millisecond.AppendChild(document.CreateTextNode(report.GetCreationDate().Millisecond.ToString()));
			creationDate.AppendChild(millisecond);

			XmlElement shortDescription = document.CreateElement(string.Empty, "short-description", string.Empty);
			shortDescription.AppendChild(document.CreateTextNode(report.GetShortDescription()));
			body.AppendChild(shortDescription);

			XmlElement longDescription = document.CreateElement(string.Empty, "long-description", string.Empty);
			longDescription.AppendChild(document.CreateTextNode(report.GetLongDescription()));
			body.AppendChild(longDescription);

			XmlElement analyses = document.CreateElement(string.Empty, "analyses", string.Empty);
			body.AppendChild(analyses);

			foreach (DESolvingType solvingType in report.GetSolvingTypes())
			{
				int index = 0;

				foreach (Tuple<float, T_VARIABLE> meanAbsoluteErrorData in report.GetMeanAbsoluteErrors(solvingType))
				{
					XmlElement analysis = document.CreateElement(string.Empty, "analysis", string.Empty);
					analyses.AppendChild(analysis);

					XmlElement solvingTypeElement = document.CreateElement(string.Empty, "solving-type", string.Empty);
					solvingTypeElement.AppendChild(document.CreateTextNode(solvingType.ToString()));
					analysis.AppendChild(solvingTypeElement);

					XmlElement parameterStepElement = document.CreateElement(string.Empty, "parameter-step", string.Empty);
					parameterStepElement.AppendChild(document.CreateTextNode(meanAbsoluteErrorData.Item1.ToString()));
					analysis.AppendChild(parameterStepElement);

					_variableSerializer.VariableName = "mean-absolute-error";
					_variableSerializer.Variable = meanAbsoluteErrorData.Item2;
					_variableSerializer.UpdateSerialization();
					XmlElement meanAbsoluteError = _variableSerializer.SerializedVariable;
					analysis.AppendChild(meanAbsoluteError);

					XmlElement simulation = document.CreateElement(string.Empty, "simulation", string.Empty);
					analysis.AppendChild(simulation);

					foreach (Tuple<float, T_VARIABLE> simulationData in report.GetSimulationValues(solvingType, index))
					{
						XmlElement valueItem = document.CreateElement(string.Empty, "value", string.Empty);
						simulation.AppendChild(valueItem);

						XmlElement time = document.CreateElement(string.Empty, "time", string.Empty);
						time.AppendChild(document.CreateTextNode(simulationData.Item1.ToString()));
						valueItem.AppendChild(time);

						_variableSerializer.VariableName = "variable";
						_variableSerializer.Variable = simulationData.Item2;
						_variableSerializer.UpdateSerialization();
						XmlElement variable = _variableSerializer.SerializedVariable;
						valueItem.AppendChild(variable);
					}

					index++;
				}
			}

			document.Save(writer);
			writer.Close();
		}
		#endregion Methods

	}
}
