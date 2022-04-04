namespace dExplorer.Editor.Mathematics
{
	using dExplorer.Runtime.Mathematics;
	using Unity.Burst;
	using Unity.Collections;
	using Unity.Collections.LowLevel.Unsafe;
	using Unity.Jobs;
	using Unity.Mathematics;
	using UnityEngine;

	public unsafe delegate void Float2InitialVariableFunction(float* modelData, float* modelTemporaryData, float2* initialVariable);
	public unsafe delegate void Float2DerivativeFunction(float* modelData, float* modelTemporaryData, float2* currentVariable, float currentParameter, float2* currentDerivative);
	public unsafe delegate void Float2AnalyticalSolutionFunction(float* modelData, float* modelTemporaryData, float currentParameter, float2* currentVariable);
	public unsafe delegate void Float2VariableDimensionalizationFunction(float* modelData, float* modelTemporaryData, float2* nonDimensionalizedVariable, float2* dimensionalizedVariable);

	/// <summary>
	/// Dimension 2 differential equation simulation with specific solving type, duration and parameter step.
	/// </summary>
	[BurstCompile]
    public struct Float2DESimulationJob : IJob
    {
		#region Fields
		[ReadOnly] public bool IsNondimensionalized;
		[ReadOnly] public NativeArray<float> ModelData;
		[ReadOnly] public NativeArray<float> ModelTemporaryData;
		[ReadOnly] public FunctionPointer<Float2InitialVariableFunction> InitialVariableFunctionPointer;
		[ReadOnly] public FunctionPointer<Float2DerivativeFunction> DerivativeFunctionPointer;
		[ReadOnly] public FunctionPointer<Float2AnalyticalSolutionFunction> AnalyticalSolutionFunctionPointer;
		[ReadOnly] public FunctionPointer<Float2VariableDimensionalizationFunction> VariableDimensionalizationFunctionPointer;
		[ReadOnly] public FunctionPointer<ParameterDimensionalizationFunction> ParameterDimensionalizationFunctionPointer;

		[ReadOnly] public float MinParameter;
		[ReadOnly] public float MaxParameter;
		[ReadOnly] public float ParameterStep;
		[ReadOnly] public DESolvingType SolvingType;

		[WriteOnly] public NativeArray<float> Parameter;
		[WriteOnly] public NativeArray<Vector2> Result;
		#endregion Fields

		#region Methods
		public unsafe void Execute()
        {
			float* modelDataPtr = (float*)ModelData.GetUnsafeReadOnlyPtr();
			float* modelTemporaryDataPtr = (float*)ModelTemporaryData.GetUnsafeReadOnlyPtr();

			float2 initialVariable;
			InitialVariableFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, &initialVariable);

			if (IsNondimensionalized)
			{
				float2 dimensionalizedVariable;

				Parameter[0] = ParameterDimensionalizationFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, MinParameter);
				VariableDimensionalizationFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, &initialVariable, &dimensionalizedVariable);
				Result[0] = dimensionalizedVariable;
			}
			else
			{
				Parameter[0] = MinParameter;
				Result[0] = initialVariable;
			}

			float2 currentVariable = initialVariable;
			
			int index = 1;
			float currentParameter = MinParameter;

			while (currentParameter < MaxParameter && index < Result.Length)
			{
				float2 nextVariable;

				switch (SolvingType)
				{
					case DESolvingType.ANALYTICAL:
						AnalyticalSolutionFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, currentParameter + ParameterStep, &nextVariable);
						break;

					case DESolvingType.EXPLICIT_EULER:
						float2 currentDerivative;
						DerivativeFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, &currentVariable, currentParameter, &currentDerivative);
						nextVariable = DESolver.SolveWithExplicitEuler(currentVariable, currentDerivative, ParameterStep);
						break;

					case DESolvingType.EXPLICIT_RUNGE_KUTTA_2:
						float2 rk2_k1;
						DerivativeFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, &currentVariable, currentParameter, &rk2_k1);

						float2 tempVariable = DESolver.SolveWithExplicitEuler(currentVariable, rk2_k1, ParameterStep);
						float2 rk2_k2;
						DerivativeFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, &tempVariable, currentParameter, &rk2_k2);

						nextVariable = DESolver.SolveWithExplicitRungeKutta2(currentVariable, rk2_k1, rk2_k2, ParameterStep);
						break;

					case DESolvingType.EXPLICIT_RUNGE_KUTTA_4:
						float halfParameterStep = 0.5f * ParameterStep;

						float2 rk4_k1;
						DerivativeFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, &currentVariable, currentParameter, &rk4_k1);

						tempVariable = DESolver.SolveWithExplicitEuler(currentVariable, rk4_k1, halfParameterStep);
						float2 rk4_k2;
						DerivativeFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, &tempVariable, currentParameter + halfParameterStep, &rk4_k2);

						tempVariable = DESolver.SolveWithExplicitEuler(currentVariable, rk4_k2, halfParameterStep);
						float2 rk4_k3;
						DerivativeFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, &tempVariable, currentParameter + halfParameterStep, &rk4_k3);

						tempVariable = DESolver.SolveWithExplicitEuler(currentVariable, rk4_k3, ParameterStep);
						float2 rk4_k4;
						DerivativeFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, &tempVariable, currentParameter + ParameterStep, &rk4_k4);

						nextVariable = DESolver.SolveWithExplicitRungeKutta4(currentVariable, rk4_k1, rk4_k2, rk4_k3, rk4_k4, ParameterStep);
						break;

					default:
						nextVariable = float.NaN;
						break;
				}

				currentParameter += ParameterStep;

				if (IsNondimensionalized)
				{
					float2 dimensionalizedVariable;
					Parameter[index] = ParameterDimensionalizationFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, currentParameter);
					VariableDimensionalizationFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, &nextVariable, &dimensionalizedVariable);
					Result[index] = dimensionalizedVariable;
				}
				else
				{
					Parameter[index] = currentParameter;
					Result[index] = nextVariable;
				}

				currentVariable = nextVariable;

				index++;
			}
        }
		#endregion Methods
	}
}
