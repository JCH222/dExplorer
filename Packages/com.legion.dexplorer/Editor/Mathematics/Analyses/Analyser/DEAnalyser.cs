namespace dExplorer.Editor.Mathematics
{
	using dExplorer.Runtime.Mathematics;
	using System.Collections.Generic;
	using Unity.Collections;
	using Unity.Jobs;
	using UnityEngine;

	public struct AnalysisProgression
	{
		#region Fields
		public float Ratio;
		public string Message;
		#endregion Fields
	}

	/// <summary>
	/// Differential equation simulations with multiple solving types and parameter steps.
	/// </summary>
	/// <typeparam name="T_REPORT"></typeparam>
	/// <typeparam name="T_ANALYSIS_VALUE"></typeparam>
	/// <typeparam name="T_VARIABLE"></typeparam>
	/// <typeparam name="T_SIMULATION_JOB"></typeparam>
	/// <typeparam name="T_ANALYSIS_JOB"></typeparam>
	public abstract class DEAnalyser<T_REPORT, T_ANALYSIS_VALUE, T_VARIABLE, T_SIMULATION_JOB, T_ANALYSIS_JOB>
		where T_REPORT : DEAnalysisReport<T_ANALYSIS_VALUE, T_VARIABLE>
		where T_ANALYSIS_VALUE : IAnalysisValue<T_VARIABLE>, new()
		where T_VARIABLE : struct
		where T_SIMULATION_JOB : struct, IJob
		where T_ANALYSIS_JOB : struct, IJob
	{
		#region Fields
		private List<Dictionary<DESolvingType, NativeArray<float>>> _times = new List<Dictionary<DESolvingType, NativeArray<float>>>();
		private List<Dictionary<DESolvingType, NativeArray<T_VARIABLE>>> _results = new List<Dictionary<DESolvingType, NativeArray<T_VARIABLE>>>();
		private List<Dictionary<DESolvingType, JobHandle>> _analysisJobHandles = new List<Dictionary<DESolvingType, JobHandle>>();
		private NativeArray<T_VARIABLE> _meanAbsoluteErrors;
		private bool _isAnalysing;
		#endregion Fields

		#region Accessors
		public float MinParameter { get; set; }
		public float MaxParameter { get; set; }
		public SortedSet<float> ParameterSteps { get; private set; }
		public HashSet<DESolvingType> SolvingTypes { get; private set; }
		public DEModel Model { get; private set; }
		#endregion Accessors

		#region Constructors
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="model">Decorated differential equation model</param>
		/// <param name="minParameter">Min parameter value</param>
		/// <param name="maxParameter">Max parameter value</param>
		public DEAnalyser(DEModel model, float minParameter = 0.0f, float maxParameter = 0.0f)
		{
			MinParameter = minParameter;
			MaxParameter = maxParameter;
			ParameterSteps = new SortedSet<float>();
			SolvingTypes = new HashSet<DESolvingType>();
			Model = model;

			_isAnalysing = false;
		}
		#endregion Constructors

		#region Methods
		/// <summary>
		/// Launch all simulations and save the aggregate results into a report.
		/// </summary>
		public void StartAnalysis()
		{
			if (_isAnalysing == false)
			{
				_isAnalysing = true;

				List<Dictionary<DESolvingType, T_SIMULATION_JOB>> simulationJobs = new List<Dictionary<DESolvingType, T_SIMULATION_JOB>>();
				List<Dictionary<DESolvingType, T_ANALYSIS_JOB>> analyseJobs = new List<Dictionary<DESolvingType, T_ANALYSIS_JOB>>();
				_meanAbsoluteErrors = new NativeArray<T_VARIABLE>(ParameterSteps.Count * (SolvingTypes.Count + 1), Allocator.Persistent, NativeArrayOptions.ClearMemory);

				int globalIndex = 0;
				int meanAbsoluteErrorsPtrIndex = 0;

				foreach (float parameterStep in ParameterSteps)
				{
					int simulationIterationNb = (int)((MaxParameter - MinParameter) / parameterStep) + 1;
					float realMaxParameter = (float)MinParameter + (float)(simulationIterationNb - 1) * parameterStep;
					Dictionary<DESolvingType, JobHandle> simulationJobHandles = new Dictionary<DESolvingType, JobHandle>();

					simulationJobs.Add(new Dictionary<DESolvingType, T_SIMULATION_JOB>());
					_times.Add(new Dictionary<DESolvingType, NativeArray<float>>());
					_results.Add(new Dictionary<DESolvingType, NativeArray<T_VARIABLE>>());
					_analysisJobHandles.Add(new Dictionary<DESolvingType, JobHandle>());
					analyseJobs.Add(new Dictionary<DESolvingType, T_ANALYSIS_JOB>());

					HashSet<DESolvingType> solvingTypes = new HashSet<DESolvingType>(SolvingTypes) { DESolvingType.ANALYTICAL };

					foreach (DESolvingType solvingType in solvingTypes)
					{
						NativeArray<float> time = new NativeArray<float>(simulationIterationNb, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
						NativeArray<T_VARIABLE> result = new NativeArray<T_VARIABLE>(simulationIterationNb, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
						T_SIMULATION_JOB simulationJob = GenerateSimulationJob(realMaxParameter, parameterStep, solvingType, time, result);

						_times[globalIndex].Add(solvingType, time);
						_results[globalIndex].Add(solvingType, result);
						simulationJobs[globalIndex].Add(solvingType, simulationJob);
						simulationJobHandles.Add(solvingType, simulationJob.Schedule());
					}

					foreach (DESolvingType solvingType in solvingTypes)
					{
						if (solvingType != DESolvingType.ANALYTICAL)
						{
							analyseJobs[globalIndex].Add(solvingType, GenerateAnalysisJob(solvingType, _results[globalIndex], _meanAbsoluteErrors, meanAbsoluteErrorsPtrIndex));

							JobHandle dependency = JobHandle.CombineDependencies(simulationJobHandles[solvingType], simulationJobHandles[DESolvingType.ANALYTICAL]);
							_analysisJobHandles[globalIndex][solvingType] = analyseJobs[globalIndex][solvingType].Schedule(dependency);
						}
						else
						{
							_meanAbsoluteErrors[meanAbsoluteErrorsPtrIndex] = new T_VARIABLE();
						}

						meanAbsoluteErrorsPtrIndex++;
					}

					globalIndex++;
				}
			}
			else
			{
				// TODO : Add error log
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IEnumerable<AnalysisProgression> CheckAnalysisProgression()
		{
			if (_isAnalysing)
			{
				bool isCompleted = false;
				int analysisJobNb = 0;
				int completedAnalysisJobNb = 0;
				
				for (int i = 0, length = _analysisJobHandles.Count; i < length; i++)
				{
					analysisJobNb += _analysisJobHandles[i].Count;
				}

				while (isCompleted == false)
				{
					for (int i = 0, length = _analysisJobHandles.Count; i < length; i++)
					{
						foreach (DESolvingType solvingType in SolvingTypes)
						{
							isCompleted = _analysisJobHandles[i][solvingType].IsCompleted;

							if (isCompleted)
							{
								completedAnalysisJobNb++;
							}
						}
					}

					if (analysisJobNb > 0)
					{
						yield return new AnalysisProgression()
						{
							Ratio = (float)completedAnalysisJobNb / (float)analysisJobNb,
							Message = string.Format("Simulating... [{0} / {1}]", completedAnalysisJobNb, analysisJobNb)
						};
					}
					else
					{
						isCompleted = true;
						yield return new AnalysisProgression()
						{
							Ratio = 1.0f,
							Message = "Simulating... [0 / 0]"
						};
					}
				}
			}
			else
			{
				// TODO : Add error log
				yield return new AnalysisProgression()
				{
					Ratio = float.NaN,
					Message = string.Empty
				};
			}
		}

		/// <summary>
		/// Save all simulations and aggregate results into a report
		/// </summary>
		/// <param name="isFullReport">Generate a report with all simulation data</param>
		/// <returns>The analysis report</returns>
		public T_REPORT GetAnalysisReport(bool isFullReport)
		{
			T_REPORT report = ScriptableObject.CreateInstance<T_REPORT>();
			report.IsFullReport = isFullReport;

			int globalIndex = 0;
			int meanAbsoluteErrorIndex = 0;

			foreach (float parameterStep in ParameterSteps)
			{
				foreach (DESolvingType solvingType in new HashSet<DESolvingType>(SolvingTypes) { DESolvingType.ANALYTICAL })
				{
					NativeArray<float> time = _times[globalIndex][solvingType];
					NativeArray<T_VARIABLE> result = _results[globalIndex][solvingType];

					if (solvingType != DESolvingType.ANALYTICAL)
					{
						_analysisJobHandles[globalIndex][solvingType].Complete();
					}

					report.AddValue(solvingType, parameterStep, _meanAbsoluteErrors[meanAbsoluteErrorIndex], time, result);
					time.Dispose();
					result.Dispose();
					meanAbsoluteErrorIndex++;
				}

				globalIndex++;
			}

			_times.Clear();
			_results.Clear();
			_analysisJobHandles.Clear();
			_meanAbsoluteErrors.Dispose();

			_isAnalysing = false;

			return report;
		}

		protected abstract T_SIMULATION_JOB GenerateSimulationJob(float realMaxParameter, float parameterStep, DESolvingType solvingType, NativeArray<float> times, NativeArray<T_VARIABLE> result);
		protected abstract T_ANALYSIS_JOB GenerateAnalysisJob(DESolvingType solvingType, Dictionary<DESolvingType, NativeArray<T_VARIABLE>> results, NativeArray<T_VARIABLE> meanAbsoluteErrors, int meanAbsoluteErrorsPtrIndex);
		#endregion Methods
	}
}
