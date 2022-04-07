namespace dExplorer.Editor.Mathematics
{
	using dExplorer.Runtime.Mathematics;
	using System.Collections.Generic;
	using Unity.Burst;
	using Unity.Collections;
	using Unity.Collections.LowLevel.Unsafe;

	/// <summary>
	/// Dimension 1 differential equation simulations with multiple solving types and parameter steps.
	/// </summary>
	public class FloatDEAnalyser : DEAnalyser<FloatDEAnalysisReport, FloatDEAnalysisValue, float, FloatDESimulationJob, FloatDEAnalysisJob>
	{
		#region Accessors
		public FunctionPointer<FloatInitialVariableFunction> InitialVariableFunctionPointer { get; private set; }
		public FunctionPointer<FloatPreSimulationFunction> PreSimulationFunctionPointer { get; private set; }
		public FunctionPointer<FloatPostSimulationFunction> PostSimulationFunctionPointer { get; private set; }
		public FunctionPointer<FloatDerivativeFunction> DerivativeFunctionPointer { get; private set; }
		public FunctionPointer<FloatAnalyticalSolutionFunction> AnalyticalSolutionFunctionPointer { get; private set; }
		public FunctionPointer<FloatVariableDimensionalizationFunction> VariableDimensionalizationFunctionPointer { get; private set; }
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
		public FloatDEAnalyser(DEModel model,
			FunctionPointer<FloatInitialVariableFunction> initialVariableFunctionPointer,
			FunctionPointer<FloatPreSimulationFunction> preSimulationFunctionPointer, 
			FunctionPointer<FloatPostSimulationFunction> postSimulationFunctionPointer,
			FunctionPointer<FloatDerivativeFunction> derivativeFunctionPointer,
			FunctionPointer<FloatAnalyticalSolutionFunction> analyticalSolutionFunctionPointer,
			float minParameter, float maxParameter, bool isNondimensionalized,
			FunctionPointer<FloatVariableDimensionalizationFunction> variableDimensionalizationFunctionPointer,
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
		protected override FloatDESimulationJob GenerateSimulationJob(float realMaxParameter, float parameterStep, DESolvingType solvingType, NativeArray<float> time, NativeArray<float> result)
		{
			NativeArray<float> duplicatedModelTemporaryData = Model.DuplicateTemporaryData();
			DuplicatedModelTemporaryDataContainer.Add(duplicatedModelTemporaryData);

			return new FloatDESimulationJob()
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

		protected override unsafe FloatDEAnalysisJob GenerateAnalysisJob(DESolvingType solvingType, Dictionary<DESolvingType, NativeArray<float>> results, NativeArray<float> meanAbsoluteErrors, int meanAbsoluteErrorsPtrIndex)
		{
			float* meanAbsoluteErrorsPtr = (float*)NativeArrayUnsafeUtility.GetUnsafePtr<float>(meanAbsoluteErrors);

			return new FloatDEAnalysisJob()
			{
				ExactValues = results[DESolvingType.ANALYTICAL],
				Approximations = results[solvingType],

				MeanAbsoluteErrorPtr = meanAbsoluteErrorsPtr + meanAbsoluteErrorsPtrIndex
			};
		}
		#endregion Methods
	}
}
