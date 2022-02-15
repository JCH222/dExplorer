namespace dExplorer.Editor.Mathematics
{
	using dExplorer.Runtime.Mathematics;
	using System.Collections.Generic;
	using Unity.Burst;
	using Unity.Collections;
	using Unity.Collections.LowLevel.Unsafe;
	using Unity.Jobs;
	using UnityEngine;

	/// <summary>
	/// Dimension 1 differential equation simulations with multiple solving types and parameter steps.
	/// </summary>
	public class FloatDEAnalyser
	{
		#region Accessors
		public float MinParameter { get; set; }
		public float MaxParameter { get; set; }
		public SortedSet<float> ParameterSteps { get; private set; }
		public HashSet<DESolvingType> SolvingTypes { get; private set; }
		public DEModel Model { get; private set; }
		public FunctionPointer<FloatInitialVariableFunction> InitialVariableFunctionPointer { get; private set; }
		public FunctionPointer<FloatDerivativeFunction> DerivativeFunctionPointer { get; private set; }
		public FunctionPointer<FloatAnalyticalSolutionFunction> AnalyticalSolutionFunctionPointer { get; private set; }
		#endregion Accessors

		#region Constructors
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="model">Decorated differential equation model</param>
		/// <param name="initialVariableFunctionPointer">Initial state function pointer</param>
		/// <param name="derivativeFunctionPointer">Derivative computation function pointer</param>
		/// <param name="analyticalSolutionFunctionPointer">Analytical solution computation function pointer</param>
		/// <param name="minParameter">Min parameter value</param>
		/// <param name="maxParameter">Max parameter value</param>
		public FloatDEAnalyser(DEModel model,
			FunctionPointer<FloatInitialVariableFunction> initialVariableFunctionPointer,
			FunctionPointer<FloatDerivativeFunction> derivativeFunctionPointer,
			FunctionPointer<FloatAnalyticalSolutionFunction> analyticalSolutionFunctionPointer,
			float minParameter = 0.0f, float maxParameter = 0.0f)
		{
			MinParameter = minParameter;
			MaxParameter = maxParameter;
			ParameterSteps = new SortedSet<float>();
			SolvingTypes = new HashSet<DESolvingType>();

			Model = model;
			InitialVariableFunctionPointer = initialVariableFunctionPointer;
			DerivativeFunctionPointer = derivativeFunctionPointer;
			AnalyticalSolutionFunctionPointer = analyticalSolutionFunctionPointer;
		}
		#endregion Constructors

		#region Methods
		/// <summary>
		/// Launch all simulations and save the aggregate results into a report.
		/// </summary>
		/// <returns>The analysis report</returns>
		public unsafe FloatDEAnalysisReport Analyse()
		{
			List<Dictionary<DESolvingType, FloatDESimulationJob>> simulationJobs = new List<Dictionary<DESolvingType, FloatDESimulationJob>>(); ;
			List<Dictionary<DESolvingType, NativeArray<float>>> results = new List<Dictionary<DESolvingType, NativeArray<float>>>();
			List<Dictionary<DESolvingType, JobHandle>> analyseJobHandles = new List<Dictionary<DESolvingType, JobHandle>>();
			List<Dictionary<DESolvingType, FloatDEAnalysisJob>> analyseJobs = new List<Dictionary<DESolvingType, FloatDEAnalysisJob>>();
			NativeArray<float> meanAbsoluteErrors = new NativeArray<float>(ParameterSteps.Count * SolvingTypes.Count, Allocator.Persistent, NativeArrayOptions.ClearMemory);

			int globalIndex = 0;
			int meanAbsoluteErrorsPtrIndex = 0;
			float* meanAbsoluteErrorsPtr = (float*)NativeArrayUnsafeUtility.GetUnsafePtr<float>(meanAbsoluteErrors);

			foreach (float parameterStep in ParameterSteps)
			{
				int simulationIterationNb = (int)((MaxParameter - MinParameter) / parameterStep) + 1;
				float realMaxParameter = (float)MinParameter + (float)(simulationIterationNb - 1) * parameterStep;
				Dictionary<DESolvingType, JobHandle> simulationJobHandles = new Dictionary<DESolvingType, JobHandle>();

				simulationJobs.Add(new Dictionary<DESolvingType, FloatDESimulationJob>());
				results.Add(new Dictionary<DESolvingType, NativeArray<float>>());
				analyseJobHandles.Add(new Dictionary<DESolvingType, JobHandle>());
				analyseJobs.Add(new Dictionary<DESolvingType, FloatDEAnalysisJob>());

				foreach (DESolvingType solvingType in new HashSet<DESolvingType>(SolvingTypes) { DESolvingType.ANALYTICAL })
				{
					results[globalIndex].Add(solvingType, new NativeArray<float>(simulationIterationNb, Allocator.Persistent, NativeArrayOptions.UninitializedMemory));

					simulationJobs[globalIndex].Add(solvingType, new FloatDESimulationJob()
					{
						ModelData = Model.Data,
						InitialVariableFunctionPointer = InitialVariableFunctionPointer,
						DerivativeFunctionPointer = DerivativeFunctionPointer,
						AnalyticalSolutionFunctionPointer = AnalyticalSolutionFunctionPointer,

						MinParameter = MinParameter,
						MaxParameter = realMaxParameter,
						ParameterStep = parameterStep,
						SolvingType = solvingType,

						Result = results[globalIndex][solvingType]
					});

					simulationJobHandles.Add(solvingType, simulationJobs[globalIndex][solvingType].Schedule());
				}

				foreach (DESolvingType solvingType in SolvingTypes)
				{
					analyseJobs[globalIndex].Add(solvingType, new FloatDEAnalysisJob()
					{
						ExactValues = results[globalIndex][DESolvingType.ANALYTICAL],
						Approximations = results[globalIndex][solvingType],

						MeanAbsoluteErrorPtr = meanAbsoluteErrorsPtr + meanAbsoluteErrorsPtrIndex
					});

					JobHandle dependency = JobHandle.CombineDependencies(simulationJobHandles[solvingType], simulationJobHandles[DESolvingType.ANALYTICAL]);
					analyseJobHandles[globalIndex][solvingType] = analyseJobs[globalIndex][solvingType].Schedule(dependency);

					meanAbsoluteErrorsPtrIndex++;
				}

				globalIndex++;
			}

			FloatDEAnalysisReport report = ScriptableObject.CreateInstance<FloatDEAnalysisReport>();

			globalIndex = 0;
			int meanAbsoluteErrorIndex = 0;

			foreach (float parameterStep in ParameterSteps)
			{
				foreach (DESolvingType solvingType in SolvingTypes)
				{
					analyseJobHandles[globalIndex][solvingType].Complete();
					report.AddValue(solvingType, parameterStep, meanAbsoluteErrors[meanAbsoluteErrorIndex]);
					results[globalIndex][solvingType].Dispose();
					meanAbsoluteErrorIndex++;
				}

				globalIndex++;
			}

			meanAbsoluteErrors.Dispose();

			return report;
		}
		#endregion Methods
	}
}
