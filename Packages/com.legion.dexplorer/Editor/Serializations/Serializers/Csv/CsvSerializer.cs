namespace dExplorer.Editor.Serializations
{
	using dExplorer.Runtime.Mathematics;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	public class CsvSerializer<T_VARIABLE, T_VARIABLE_SERIALIZER> : ISerializer<T_VARIABLE, T_VARIABLE_SERIALIZER>
		where T_VARIABLE : struct
		where T_VARIABLE_SERIALIZER : CsvVariableSerializer<T_VARIABLE>, new()
	{
		#region Fields
		private readonly CsvVariableSerializer<T_VARIABLE> _variableSerializer;
		private readonly string _exportFolderPath;
		private readonly string _separator;
		#endregion Fields

		#region Constructors
		public CsvSerializer(string exportFolderPath, string separator)
		{
			_variableSerializer = new T_VARIABLE_SERIALIZER()
			{
				VariableName = string.Empty,
				Variable = new T_VARIABLE(),
				Separator = string.Empty
			};

			_exportFolderPath = exportFolderPath;
			_separator = separator;
		}
		#endregion Properties

		#region Methods
		public void Serialize(IDEAnalysisReportSerializable<T_VARIABLE> report)
		{
			string mainFolder = Path.Combine(_exportFolderPath, report.GetName());

			if (Directory.Exists(mainFolder) == false)
			{
				Directory.CreateDirectory(mainFolder);
			}

			List<float> parameterSteps = new List<float>();
			List<DESolvingType> solvingTypes = new List<DESolvingType>();
			Dictionary<float, Dictionary<DESolvingType, T_VARIABLE>> meanAbsoluteErrors = new Dictionary<float, Dictionary<DESolvingType, T_VARIABLE>>();

			foreach (DESolvingType solvingType in report.GetSolvingTypes())
			{
				if (solvingType != DESolvingType.ANALYTICAL)
				{
					solvingTypes.Add(solvingType);

					foreach (Tuple<float, T_VARIABLE> meanAbsoluteErrorData in report.GetMeanAbsoluteErrors(solvingType))
					{
						float parameterStep = meanAbsoluteErrorData.Item1;

						parameterSteps.Add(parameterStep);

						if (meanAbsoluteErrors.ContainsKey(parameterStep) == false)
						{
							meanAbsoluteErrors.Add(parameterStep, new Dictionary<DESolvingType, T_VARIABLE>());
						}

						meanAbsoluteErrors[parameterStep].Add(solvingType, meanAbsoluteErrorData.Item2);
					}
				}
			}

			parameterSteps = parameterSteps.Distinct().ToList();
			parameterSteps.Sort();

			using (StreamWriter writer = new StreamWriter(Path.Combine(mainFolder, "mean_absolute_error.csv")))
			{
				string header = "Parameter Step";

				foreach (DESolvingType solvingType in solvingTypes)
				{
					_variableSerializer.VariableName = solvingType.ToString();
					_variableSerializer.Variable = new T_VARIABLE();
					_variableSerializer.Separator = _separator;
					_variableSerializer.UpdateSerialization();

					header += _variableSerializer.Separator + _variableSerializer.SerializedHeader;
				}

				writer.WriteLine(header);

				foreach (float parameterStep in parameterSteps)
				{
					string line = parameterStep.ToString();

					foreach (DESolvingType solvingType in solvingTypes)
					{
						_variableSerializer.VariableName = solvingType.ToString();
						_variableSerializer.Variable = meanAbsoluteErrors.ContainsKey(parameterStep) ? meanAbsoluteErrors[parameterStep][solvingType] : new T_VARIABLE();
						_variableSerializer.Separator = _separator;
						_variableSerializer.UpdateSerialization();

						line += _separator + _variableSerializer.SerializedVariable;
					}

					writer.WriteLine(line);
				}
			}
		}
		#endregion Methods

	}
}
