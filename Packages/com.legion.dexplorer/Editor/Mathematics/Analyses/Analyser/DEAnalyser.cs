namespace dExplorer.Editor.Mathematics
{
	using dExplorer.Runtime.Mathematics;
	using System.Collections.Generic;
	using Unity.Collections;
	using Unity.Collections.LowLevel.Unsafe;
	using Unity.Jobs;
	using UnityEngine;

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
		}
		#endregion Constructors

		#region Methods
		/// <summary>
		/// Launch all simulations and save the aggregate results into a report.
		/// </summary>
		/// <param name="isFullReport">Generate a report with all simulation data</param>
		/// <returns>The analysis report</returns>
		public unsafe T_REPORT Analyse(bool isFullReport)
		{
			List<Dictionary<DESolvingType, T_SIMULATION_JOB>> simulationJobs = new List<Dictionary<DESolvingType, T_SIMULATION_JOB>>();
			List<Dictionary<DESolvingType, NativeArray<float>>> times = new List<Dictionary<DESolvingType, NativeArray<float>>>();
			List<Dictionary<DESolvingType, NativeArray<T_VARIABLE>>> results = new List<Dictionary<DESolvingType, NativeArray<T_VARIABLE>>>();
			List<Dictionary<DESolvingType, JobHandle>> analyseJobHandles = new List<Dictionary<DESolvingType, JobHandle>>();
			List<Dictionary<DESolvingType, T_ANALYSIS_JOB>> analyseJobs = new List<Dictionary<DESolvingType, T_ANALYSIS_JOB>>();
			NativeArray<T_VARIABLE> meanAbsoluteErrors = new NativeArray<T_VARIABLE>(ParameterSteps.Count * SolvingTypes.Count, Allocator.Persistent, NativeArrayOptions.ClearMemory);

			int globalIndex = 0;
			int meanAbsoluteErrorsPtrIndex = 0;

			foreach (float parameterStep in ParameterSteps)
			{
				int simulationIterationNb = (int)((MaxParameter - MinParameter) / parameterStep) + 1;
				float realMaxParameter = (float)MinParameter + (float)(simulationIterationNb - 1) * parameterStep;
				Dictionary<DESolvingType, JobHandle> simulationJobHandles = new Dictionary<DESolvingType, JobHandle>();

				simulationJobs.Add(new Dictionary<DESolvingType, T_SIMULATION_JOB>());
				times.Add(new Dictionary<DESolvingType, NativeArray<float>>());
				results.Add(new Dictionary<DESolvingType, NativeArray<T_VARIABLE>>());
				analyseJobHandles.Add(new Dictionary<DESolvingType, JobHandle>());
				analyseJobs.Add(new Dictionary<DESolvingType, T_ANALYSIS_JOB>());

				foreach (DESolvingType solvingType in new HashSet<DESolvingType>(SolvingTypes) { DESolvingType.ANALYTICAL })
				{
					NativeArray<float> time = new NativeArray<float>(simulationIterationNb, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
					NativeArray<T_VARIABLE> result = new NativeArray<T_VARIABLE>(simulationIterationNb, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
					T_SIMULATION_JOB simulationJob = GenerateSimulationJob(realMaxParameter, parameterStep, solvingType, time, result);

					times[globalIndex].Add(solvingType, time);
					results[globalIndex].Add(solvingType, result);
					simulationJobs[globalIndex].Add(solvingType, simulationJob);
					simulationJobHandles.Add(solvingType, simulationJob.Schedule());
				}

				foreach (DESolvingType solvingType in SolvingTypes)
				{
					analyseJobs[globalIndex].Add(solvingType, GenerateAnalysisJob(solvingType, results[globalIndex], meanAbsoluteErrors, meanAbsoluteErrorsPtrIndex));

					JobHandle dependency = JobHandle.CombineDependencies(simulationJobHandles[solvingType], simulationJobHandles[DESolvingType.ANALYTICAL]);
					analyseJobHandles[globalIndex][solvingType] = analyseJobs[globalIndex][solvingType].Schedule(dependency);

					meanAbsoluteErrorsPtrIndex++;
				}

				globalIndex++;
			}

			T_REPORT report = ScriptableObject.CreateInstance<T_REPORT>();
			report.IsFullReport = isFullReport;

			globalIndex = 0;
			int meanAbsoluteErrorIndex = 0;

			foreach (float parameterStep in ParameterSteps)
			{
				foreach (DESolvingType solvingType in SolvingTypes)
				{
					NativeArray<float> time = times[globalIndex][solvingType];
					NativeArray<T_VARIABLE> result = results[globalIndex][solvingType];
					analyseJobHandles[globalIndex][solvingType].Complete();
					report.AddValue(solvingType, parameterStep, meanAbsoluteErrors[meanAbsoluteErrorIndex], time, result);
					time.Dispose();
					result.Dispose();
					meanAbsoluteErrorIndex++;
				}

				globalIndex++;
			}

			meanAbsoluteErrors.Dispose();

			return report;
		}

		protected abstract T_SIMULATION_JOB GenerateSimulationJob(float realMaxParameter, float parameterStep, DESolvingType solvingType, NativeArray<float> times, NativeArray<T_VARIABLE> result);
		protected abstract T_ANALYSIS_JOB GenerateAnalysisJob(DESolvingType solvingType, Dictionary<DESolvingType, NativeArray<T_VARIABLE>> results, NativeArray<T_VARIABLE> meanAbsoluteErrors, int meanAbsoluteErrorsPtrIndex);
		#endregion Methods
	}
}
