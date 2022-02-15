namespace dExplorer.Editor.Mathematics
{
	using dExplorer.Runtime.Mathematics;
	using System.Collections.Generic;
	using Unity.Burst;
	using Unity.Collections;
	using Unity.Collections.LowLevel.Unsafe;
	using Unity.Jobs;
	using Unity.Mathematics;
	using UnityEngine;

	/// <summary>
	/// Dimension 2 differential equation simulations with multiple solving types and parameter steps.
	/// </summary>
	public class Float2DEAnalyser
	{
		#region Accessors
		public float MinParameter { get; set; }
		public float MaxParameter { get; set; }
		public SortedSet<float> ParameterSteps { get; private set; }
		public HashSet<DESolvingType> SolvingTypes { get; private set; }
		public DEModel Model { get; private set; }
		public FunctionPointer<Float2InitialVariableFunction> InitialVariableFunctionPointer { get; private set; }
		public FunctionPointer<Float2DerivativeFunction> DerivativeFunctionPointer { get; private set; }
		public FunctionPointer<Float2AnalyticalSolutionFunction> AnalyticalSolutionFunctionPointer { get; private set; }
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
		public Float2DEAnalyser(DEModel model,
			FunctionPointer<Float2InitialVariableFunction> initialVariableFunctionPointer, 
			FunctionPointer<Float2DerivativeFunction> derivativeFunctionPointer, 
			FunctionPointer<Float2AnalyticalSolutionFunction> analyticalSolutionFunctionPointer,
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
		public unsafe Float2DEAnalysisReport Analyse()
		{
			List<Dictionary<DESolvingType, Float2DESimulationJob>> simulationJobs = new List<Dictionary<DESolvingType, Float2DESimulationJob>>(); ;
			List<Dictionary<DESolvingType, NativeArray<float2>>> results = new List<Dictionary<DESolvingType, NativeArray<float2>>>();
			List<Dictionary<DESolvingType, JobHandle>> analyseJobHandles = new List<Dictionary<DESolvingType, JobHandle>>();
			List<Dictionary<DESolvingType, Float2DEAnalysisJob>> analyseJobs = new List<Dictionary<DESolvingType, Float2DEAnalysisJob>>();
			NativeArray<float2> meanAbsoluteErrors = new NativeArray<float2>(ParameterSteps.Count * SolvingTypes.Count, Allocator.Persistent, NativeArrayOptions.ClearMemory);

			int globalIndex = 0;
			int meanAbsoluteErrorsPtrIndex = 0;
			float2* meanAbsoluteErrorsPtr = (float2*)NativeArrayUnsafeUtility.GetUnsafePtr<float2>(meanAbsoluteErrors);

			foreach (float parameterStep in ParameterSteps)
			{
				int simulationIterationNb = (int)((MaxParameter - MinParameter) / parameterStep) + 1;
				float realMaxParameter = (float)MinParameter + (float)(simulationIterationNb - 1) * parameterStep;
				Dictionary<DESolvingType, JobHandle> simulationJobHandles = new Dictionary<DESolvingType, JobHandle>();

				simulationJobs.Add(new Dictionary<DESolvingType, Float2DESimulationJob>());
				results.Add(new Dictionary<DESolvingType, NativeArray<float2>>());
				analyseJobHandles.Add(new Dictionary<DESolvingType, JobHandle>());
				analyseJobs.Add(new Dictionary<DESolvingType, Float2DEAnalysisJob>());

				foreach (DESolvingType solvingType in new HashSet<DESolvingType>(SolvingTypes) { DESolvingType.ANALYTICAL })
				{
					results[globalIndex].Add(solvingType, new NativeArray<float2>(simulationIterationNb, Allocator.Persistent, NativeArrayOptions.UninitializedMemory));

					simulationJobs[globalIndex].Add(solvingType, new Float2DESimulationJob()
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
					analyseJobs[globalIndex].Add(solvingType, new Float2DEAnalysisJob()
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

			Float2DEAnalysisReport report = ScriptableObject.CreateInstance<Float2DEAnalysisReport>();

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
