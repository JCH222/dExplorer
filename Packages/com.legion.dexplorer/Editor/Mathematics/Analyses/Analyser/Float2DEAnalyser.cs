namespace dExplorer.Editor.Mathematics
{
	using dExplorer.Runtime.Mathematics;
	using System.Collections.Generic;
	using Unity.Burst;
	using Unity.Collections;
	using Unity.Collections.LowLevel.Unsafe;
	using UnityEngine;

	/// <summary>
	/// Dimension 2 differential equation simulations with multiple solving types and parameter steps.
	/// </summary>
	public class Float2DEAnalyser : DEAnalyser<Float2DEAnalysisReport, Float2DEAnalysisValue, Vector2, Float2DESimulationJob, Float2DEAnalysisJob>
	{
		#region Accessors
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
			float minParameter = 0.0f, float maxParameter = 0.0f) : base(model, minParameter, maxParameter)
		{
			InitialVariableFunctionPointer = initialVariableFunctionPointer;
			DerivativeFunctionPointer = derivativeFunctionPointer;
			AnalyticalSolutionFunctionPointer = analyticalSolutionFunctionPointer;
		}
		#endregion Constructors

		#region Methods
		protected override Float2DESimulationJob GenerateSimulationJob(float realMaxParameter, float parameterStep, DESolvingType solvingType, NativeArray<float> time, NativeArray<Vector2> result)
		{
			return new Float2DESimulationJob()
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

		protected override unsafe Float2DEAnalysisJob GenerateAnalysisJob(DESolvingType solvingType, Dictionary<DESolvingType, NativeArray<Vector2>> results, NativeArray<Vector2> meanAbsoluteErrors, int meanAbsoluteErrorsPtrIndex)
		{
			Vector2* meanAbsoluteErrorsPtr = (Vector2*)NativeArrayUnsafeUtility.GetUnsafePtr<Vector2>(meanAbsoluteErrors);

			return new Float2DEAnalysisJob()
			{
				ExactValues = results[DESolvingType.ANALYTICAL],
				Approximations = results[solvingType],

				MeanAbsoluteErrorPtr = meanAbsoluteErrorsPtr + meanAbsoluteErrorsPtrIndex
			};
		}
		#endregion Methods
	}
}
