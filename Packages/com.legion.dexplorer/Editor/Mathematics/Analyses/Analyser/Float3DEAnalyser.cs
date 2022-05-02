namespace dExplorer.Editor.Mathematics
{
	using dExplorer.Runtime.Mathematics;
	using System.Collections.Generic;
	using Unity.Burst;
	using Unity.Collections;
	using Unity.Collections.LowLevel.Unsafe;
	using UnityEngine;

	/// <summary>
	/// Dimension 3 differential equation simulations with multiple solving types and parameter steps.
	/// </summary>
	public class Float3DEAnalyser : DEAnalyser<Float3DESerializableAnalysisReport, Float3DEAnalysisValue, Vector3, Float3DESimulationJob, Float3DEAnalysisJob>
	{
		#region Accessors
		public FunctionPointer<Float3InitialVariableFunction> InitialVariableFunctionPointer { get; private set; }
		public FunctionPointer<Float3PreSimulationFunction> PreSimulationFunctionPointer { get; private set; }
		public FunctionPointer<Float3PostSimulationFunction> PostSimulationFunctionPointer { get; private set; }
		public FunctionPointer<Float3DerivativeFunction> DerivativeFunctionPointer { get; private set; }
		public FunctionPointer<Float3AnalyticalSolutionFunction> AnalyticalSolutionFunctionPointer { get; private set; }
		public FunctionPointer<Float3VariableDimensionalizationFunction> VariableDimensionalizationFunctionPointer { get; private set; }
		#endregion Accessors

		#region Constructors
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="model">Decorated differential equation model</param>
		/// <param name="initialVariableFunctionPointer">Initial state function pointer</param>
		/// <param name="preSimulationFunctionPointer">Pre-simulation function pointer</param>
		/// <param name="postSimulationFunctionPointer">Post-simulation function pointer</param>
		/// <param name="derivativeFunctionPointer">Derivative computation function pointer</param>
		/// <param name="analyticalSolutionFunctionPointer">Analytical solution computation function pointer</param>
		/// <param name="minParameter">Min parameter value</param>
		/// <param name="maxParameter">Max parameter value</param>
		/// <param name="isNondimensionalized">Values are nondimensionalized</param>
		/// <param name="variableDimensionalizationFunctionPointer">Variable dimensionalization function pointer</param>
		/// <param name="parameterDimensionalizationFunction">Parameter dimensionalization function</param>
		public Float3DEAnalyser(DEModel model,
			FunctionPointer<Float3InitialVariableFunction> initialVariableFunctionPointer,
			FunctionPointer<Float3PreSimulationFunction> preSimulationFunctionPointer,
			FunctionPointer<Float3PostSimulationFunction> postSimulationFunctionPointer,
			FunctionPointer<Float3DerivativeFunction> derivativeFunctionPointer,
			FunctionPointer<Float3AnalyticalSolutionFunction> analyticalSolutionFunctionPointer,
			float minParameter, float maxParameter, bool isNondimensionalized,
			FunctionPointer<Float3VariableDimensionalizationFunction> variableDimensionalizationFunctionPointer,
			ParameterDimensionalizationFunction parameterDimensionalizationFunction)
			: base(model, minParameter, maxParameter, isNondimensionalized, parameterDimensionalizationFunction)
		{
			InitialVariableFunctionPointer = initialVariableFunctionPointer;
			PreSimulationFunctionPointer = preSimulationFunctionPointer;
			PostSimulationFunctionPointer = postSimulationFunctionPointer;
			DerivativeFunctionPointer = derivativeFunctionPointer;
			AnalyticalSolutionFunctionPointer = analyticalSolutionFunctionPointer;
			VariableDimensionalizationFunctionPointer = variableDimensionalizationFunctionPointer;
		}
		#endregion Constructors

		#region Methods
		protected override Float3DESimulationJob GenerateSimulationJob(float realMaxParameter, float parameterStep, DESolvingType solvingType, NativeArray<float> time, NativeArray<Vector3> result)
		{
			NativeArray<float> duplicatedModelTemporaryData = Model.DuplicateTemporaryData();
			DuplicatedModelTemporaryDataContainer.Add(duplicatedModelTemporaryData);

			return new Float3DESimulationJob()
			{
				IsNondimensionalized = IsNondimensionalized,
				ModelData = Model.Data,
				InitialVariableFunctionPointer = InitialVariableFunctionPointer,
				PreSimulationFunctionPointer = PreSimulationFunctionPointer,
				PostSimulationFunctionPointer = PostSimulationFunctionPointer,
				DerivativeFunctionPointer = DerivativeFunctionPointer,
				AnalyticalSolutionFunctionPointer = AnalyticalSolutionFunctionPointer,
				VariableDimensionalizationFunctionPointer = VariableDimensionalizationFunctionPointer,
				ParameterDimensionalizationFunctionPointer = ParameterDimensionalizationFunctionPointer,

				MinParameter = MinParameter,
				MaxParameter = realMaxParameter,
				ParameterStep = parameterStep,
				SolvingType = solvingType,

				Parameter = time,
				Result = result,

				ModelTemporaryData = duplicatedModelTemporaryData
			};
		}

		protected override unsafe Float3DEAnalysisJob GenerateAnalysisJob(DESolvingType solvingType, Dictionary<DESolvingType, NativeArray<Vector3>> results, NativeArray<Vector3> meanAbsoluteErrors, int meanAbsoluteErrorsPtrIndex)
		{
			Vector3* meanAbsoluteErrorsPtr = (Vector3*)NativeArrayUnsafeUtility.GetUnsafePtr<Vector3>(meanAbsoluteErrors);

			return new Float3DEAnalysisJob()
			{
				ExactValues = results[DESolvingType.ANALYTICAL],
				Approximations = results[solvingType],

				MeanAbsoluteErrorPtr = meanAbsoluteErrorsPtr + meanAbsoluteErrorsPtrIndex
			};
		}
		#endregion Methods
	}
}
