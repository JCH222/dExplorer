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
			float minParameter = 0.0f, float maxParameter = 0.0f) : base(model, minParameter, maxParameter)
		{
			InitialVariableFunctionPointer = initialVariableFunctionPointer;
			DerivativeFunctionPointer = derivativeFunctionPointer;
			AnalyticalSolutionFunctionPointer = analyticalSolutionFunctionPointer;
		}
		#endregion Constructors

		#region Methods
		protected override FloatDESimulationJob GenerateSimulationJob(float realMaxParameter, float parameterStep, DESolvingType solvingType, NativeArray<float> time, NativeArray<float> result)
		{
			return new FloatDESimulationJob()
			{
				ModelData = Model.Data,
				InitialVariableFunctionPointer = InitialVariableFunctionPointer,
				DerivativeFunctionPointer = DerivativeFunctionPointer,
				AnalyticalSolutionFunctionPointer = AnalyticalSolutionFunctionPointer,

				MinParameter = MinParameter,
				MaxParameter = realMaxParameter,
				ParameterStep = parameterStep,
				SolvingType = solvingType,

				Time = time,
				Result = result
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
