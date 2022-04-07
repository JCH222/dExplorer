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
			string mainFolderPath = Path.Combine(_exportFolderPath, report.GetName());

			if (Directory.Exists(mainFolderPath) == false)
			{
				Directory.CreateDirectory(mainFolderPath);
			}

			List<float> parameterSteps = new List<float>();
			Dictionary<float, List<float>> times = new Dictionary<float, List<float>>();
			List<DESolvingType> solvingTypes = new List<DESolvingType>();
			Dictionary<float, Dictionary<DESolvingType, T_VARIABLE>> meanAbsoluteErrors = new Dictionary<float, Dictionary<DESolvingType, T_VARIABLE>>();
			Dictionary<float, Dictionary<float, List<Tuple<DESolvingType, T_VARIABLE>>>> simulationValues = new Dictionary<float, Dictionary<float, List<Tuple<DESolvingType, T_VARIABLE>>>>();
			bool hasSimulationValues = false;

			foreach (DESolvingType solvingType in report.GetSolvingTypes())
			{
				int index = 0;

				if (solvingType != DESolvingType.ANALYTICAL)
				{
					solvingTypes.Add(solvingType);
				}

				foreach (Tuple<float, T_VARIABLE> meanAbsoluteErrorData in report.GetMeanAbsoluteErrors(solvingType))
				{
					float parameterStep = meanAbsoluteErrorData.Item1;

					parameterSteps.Add(parameterStep);

					if (solvingType != DESolvingType.ANALYTICAL)
					{
						if (meanAbsoluteErrors.ContainsKey(parameterStep) == false)
						{
							meanAbsoluteErrors.Add(parameterStep, new Dictionary<DESolvingType, T_VARIABLE>());
						}

						meanAbsoluteErrors[parameterStep].Add(solvingType, meanAbsoluteErrorData.Item2);
					}

					if (simulationValues.ContainsKey(parameterStep) == false)
					{
						simulationValues.Add(parameterStep, new Dictionary<float, List<Tuple<DESolvingType, T_VARIABLE>>>());
					}

					foreach (Tuple<float, T_VARIABLE> value in report.GetSimulationValues(solvingType, index))
					{
						if (times.ContainsKey(parameterStep) == false)
						{
							times.Add(parameterStep, new List<float>());
						}

						float time = value.Item1;
						times[parameterStep].Add(time);
						Dictionary<float, List<Tuple<DESolvingType, T_VARIABLE>>> simulations = simulationValues[parameterStep];

						if (simulations.ContainsKey(time) == false)
						{
							simulations.Add(time, new List<Tuple<DESolvingType, T_VARIABLE>>());
						}

						simulations[time].Add(new Tuple<DESolvingType, T_VARIABLE>(solvingType, value.Item2));

						hasSimulationValues = true;
					}

					index++;
				}
			}

			parameterSteps = parameterSteps.Distinct().ToList();
			parameterSteps.Sort();

			for (int i = 0, length = times.Count; i < length; i++)
			{
				float parameterStep = times.Keys.ToList()[i];
				times[parameterStep] = times[parameterStep].Distinct().ToList();
				times[parameterStep].Sort();
			}
			

			using (StreamWriter writer = new StreamWriter(Path.Combine(mainFolderPath, "mean_absolute_error.csv")))
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

			if (hasSimulationValues)
			{
				string simulationsFolderPath = Path.Combine(mainFolderPath, "simulations");
				Directory.CreateDirectory(simulationsFolderPath);

				foreach (float parameterStep in parameterSteps)
				{
					string header = "Time";
					List<DESolvingType> currentSolvingTypes = new HashSet<DESolvingType>(solvingTypes) { DESolvingType.ANALYTICAL }.ToList();

					foreach (DESolvingType solvingType in currentSolvingTypes)
					{
						_variableSerializer.VariableName = solvingType.ToString();
						_variableSerializer.Variable = new T_VARIABLE();
						_variableSerializer.Separator = _separator;
						_variableSerializer.UpdateSerialization();

						header += _variableSerializer.Separator + _variableSerializer.SerializedHeader;
					}

					using (StreamWriter writer = new StreamWriter(Path.Combine(simulationsFolderPath, string.Format("parameter_step_{0}.csv", parameterStep))))
					{
						writer.WriteLine(header);

						foreach (float time in times[parameterStep])
						{
							int index = 0;
							string line = time.ToString();

							foreach (DESolvingType solvingType in currentSolvingTypes)
							{
								_variableSerializer.VariableName = solvingType.ToString();
								_variableSerializer.Variable = simulationValues.ContainsKey(parameterStep) && simulationValues[parameterStep].ContainsKey(time) ? simulationValues[parameterStep][time][index].Item2 : new T_VARIABLE();
								_variableSerializer.Separator = _separator;
								_variableSerializer.UpdateSerialization();

								index++;
								line += _separator + _variableSerializer.SerializedVariable;
							}

							writer.WriteLine(line);
						}
					}
				}
			}
		}
		#endregion Methods

	}
}
