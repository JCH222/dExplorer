namespace dExplorer.Editor.Mathematics
{
	using dExplorer.Runtime.Mathematics;
	using Unity.Burst;
	using Unity.Collections;
	using Unity.Collections.LowLevel.Unsafe;
	using Unity.Jobs;

	public unsafe delegate void FloatInitialVariableFunction(float* modelData, float* initialVariable);
	public unsafe delegate void FloatDerivativeFunction(float* modelData, float* currentVariable, float currentParameter, float* currentDerivative);
	public unsafe delegate void FloatAnalyticalSolutionFunction(float* modelData, float currentParameter, float* currentVariable);

	/// <summary>
	/// Dimension 1 differential equation simulation with specific solving type, duration and parameter step.
	/// </summary>
	[BurstCompile]
	public struct FloatDESimulationJob : IJob
	{
		#region Fields
		[ReadOnly] public NativeArray<float> ModelData;
		[ReadOnly] public FunctionPointer<FloatInitialVariableFunction> InitialVariableFunctionPointer;
		[ReadOnly] public FunctionPointer<FloatDerivativeFunction> DerivativeFunctionPointer;
		[ReadOnly] public FunctionPointer<FloatAnalyticalSolutionFunction> AnalyticalSolutionFunctionPointer;

		[ReadOnly] public float MinParameter;
		[ReadOnly] public float MaxParameter;
		[ReadOnly] public float ParameterStep;
		[ReadOnly] public DESolvingType SolvingType;

		[WriteOnly] public NativeArray<float> Time;
		[WriteOnly] public NativeArray<float> Result;
		#endregion Fields

		#region Methods
		public unsafe void Execute()
		{
			float* modelDataPtr = (float*)ModelData.GetUnsafeReadOnlyPtr();
			float initialVariable;
			InitialVariableFunctionPointer.Invoke(modelDataPtr, &initialVariable);

			Time[0] = MinParameter;
			Result[0] = initialVariable;
			float currentVariable = initialVariable;

			int index = 1;
			float currentParameter = MinParameter;

			while (currentParameter < MaxParameter && index < Result.Length)
			{
				float nextVariable;
				float tempVariable;

				switch (SolvingType)
				{
					case DESolvingType.ANALYTICAL:
						AnalyticalSolutionFunctionPointer.Invoke(modelDataPtr, currentParameter + ParameterStep, &nextVariable);
						break;

					case DESolvingType.EXPLICIT_EULER:
						float currentDerivative;
						DerivativeFunctionPointer.Invoke(modelDataPtr, &currentVariable, currentParameter, &currentDerivative);
						nextVariable = DESolver.SolveWithExplicitEuler(currentVariable, currentDerivative, ParameterStep);
						break;

					case DESolvingType.EXPLICIT_RUNGE_KUTTA_2:
						float rk2_k1;
						DerivativeFunctionPointer.Invoke(modelDataPtr, &currentVariable, currentParameter, &rk2_k1);

						tempVariable = DESolver.SolveWithExplicitEuler(currentVariable, rk2_k1, ParameterStep);
						float rk2_k2;
						DerivativeFunctionPointer.Invoke(modelDataPtr, &tempVariable, currentParameter + ParameterStep, &rk2_k2);

						nextVariable = DESolver.SolveWithExplicitRungeKutta2(currentVariable, rk2_k1, rk2_k2, ParameterStep);
						break;

					case DESolvingType.EXPLICIT_RUNGE_KUTTA_4:
						float halfParameterStep = 0.5f * ParameterStep;

						float rk4_k1;
						DerivativeFunctionPointer.Invoke(modelDataPtr, &currentVariable, currentParameter, &rk4_k1);
						
						tempVariable = DESolver.SolveWithExplicitEuler(currentVariable, rk4_k1, halfParameterStep);
						float rk4_k2;
						DerivativeFunctionPointer.Invoke(modelDataPtr, &tempVariable, currentParameter + halfParameterStep, &rk4_k2);

						tempVariable = DESolver.SolveWithExplicitEuler(currentVariable, rk4_k2, halfParameterStep);
						float rk4_k3;
						DerivativeFunctionPointer.Invoke(modelDataPtr, &tempVariable, currentParameter + halfParameterStep, &rk4_k3);

						tempVariable = DESolver.SolveWithExplicitEuler(currentVariable, rk4_k3, ParameterStep);
						float rk4_k4;
						DerivativeFunctionPointer.Invoke(modelDataPtr, &tempVariable, currentParameter + ParameterStep, &rk4_k4);

						nextVariable = DESolver.SolveWithExplicitRungeKutta4(currentVariable, rk4_k1, rk4_k2, rk4_k3, rk4_k4, ParameterStep);
						break;

					default:
						nextVariable = float.NaN;
						break;
				}

				currentParameter += ParameterStep;

				Time[index] = currentParameter;
				Result[index] = nextVariable;
				currentVariable = nextVariable;

				index++;
			}
		}
		#endregion Methods
	}
}
