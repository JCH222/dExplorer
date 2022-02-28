namespace dExplorer.Editor.Mathematics
{
	using dExplorer.Runtime.Mathematics;
	using Unity.Burst;
	using Unity.Collections;
	using Unity.Collections.LowLevel.Unsafe;
	using Unity.Jobs;
	using UnityEngine;

	public unsafe delegate void Float2InitialVariableFunction(float* modelData, Vector2* initialVariable);
	public unsafe delegate void Float2DerivativeFunction(float* modelData, Vector2* currentVariable, float currentParameter, Vector2* currentDerivative);
	public unsafe delegate void Float2AnalyticalSolutionFunction(float* modelData, float currentParameter, Vector2* currentVariable);

	/// <summary>
	/// Dimension 2 differential equation simulation with specific solving type, duration and parameter step.
	/// </summary>
	[BurstCompile]
    public struct Float2DESimulationJob : IJob
    {
		#region Fields
		[ReadOnly] public NativeArray<float> ModelData;
		[ReadOnly] public FunctionPointer<Float2InitialVariableFunction> InitialVariableFunctionPointer;
		[ReadOnly] public FunctionPointer<Float2DerivativeFunction> DerivativeFunctionPointer;
		[ReadOnly] public FunctionPointer<Float2AnalyticalSolutionFunction> AnalyticalSolutionFunctionPointer;

		[ReadOnly] public float MinParameter;
		[ReadOnly] public float MaxParameter;
		[ReadOnly] public float ParameterStep;
		[ReadOnly] public DESolvingType SolvingType;

		[WriteOnly] public NativeArray<Vector2> Result;
		#endregion Fields

		#region Methods
		public unsafe void Execute()
        {
			float* modelDataPtr = (float*)ModelData.GetUnsafeReadOnlyPtr();
			Vector2 initialVariable;
			InitialVariableFunctionPointer.Invoke(modelDataPtr, &initialVariable);

			Result[0] = initialVariable;
			Vector2 currentVariable = initialVariable;
			
			int index = 1;
			float currentParameter = MinParameter;

			while (currentParameter < MaxParameter && index < Result.Length)
			{
				Vector2 nextVariable;

				switch (SolvingType)
				{
					case DESolvingType.ANALYTICAL:
						AnalyticalSolutionFunctionPointer.Invoke(modelDataPtr, currentParameter + ParameterStep, &nextVariable);
						break;

					case DESolvingType.EXPLICIT_EULER:
						Vector2 currentDerivative;
						DerivativeFunctionPointer.Invoke(modelDataPtr, &currentVariable, currentParameter, &currentDerivative);
						nextVariable = DESolver.SolveWithExplicitEuler(currentVariable, currentDerivative, ParameterStep);
						break;

					case DESolvingType.EXPLICIT_RUNGE_KUTTA_2:
						Vector2 rk2_k1;
						DerivativeFunctionPointer.Invoke(modelDataPtr, &currentVariable, currentParameter, &rk2_k1);

						Vector2 tempVariable = DESolver.SolveWithExplicitEuler(currentVariable, rk2_k1, ParameterStep);
						Vector2 rk2_k2;
						DerivativeFunctionPointer.Invoke(modelDataPtr, &tempVariable, currentParameter, &rk2_k2);

						nextVariable = DESolver.SolveWithExplicitRungeKutta2(currentVariable, rk2_k1, rk2_k2, ParameterStep);
						break;

					case DESolvingType.EXPLICIT_RUNGE_KUTTA_4:
						float halfParameterStep = 0.5f * ParameterStep;

						Vector2 rk4_k1;
						DerivativeFunctionPointer.Invoke(modelDataPtr, &currentVariable, currentParameter, &rk4_k1);

						tempVariable = DESolver.SolveWithExplicitEuler(currentVariable, rk4_k1, halfParameterStep);
						Vector2 rk4_k2;
						DerivativeFunctionPointer.Invoke(modelDataPtr, &tempVariable, currentParameter + halfParameterStep, &rk4_k2);

						tempVariable = DESolver.SolveWithExplicitEuler(currentVariable, rk4_k2, halfParameterStep);
						Vector2 rk4_k3;
						DerivativeFunctionPointer.Invoke(modelDataPtr, &tempVariable, currentParameter + halfParameterStep, &rk4_k3);

						tempVariable = DESolver.SolveWithExplicitEuler(currentVariable, rk4_k3, ParameterStep);
						Vector2 rk4_k4;
						DerivativeFunctionPointer.Invoke(modelDataPtr, &tempVariable, currentParameter + ParameterStep, &rk4_k4);

						nextVariable = DESolver.SolveWithExplicitRungeKutta4(currentVariable, rk4_k1, rk4_k2, rk4_k3, rk4_k4, ParameterStep);
						break;

					default:
						nextVariable = new Vector2(float.NaN, float.NaN);
						break;
				}

				Result[index] = nextVariable;
				currentVariable = nextVariable;

				index++;
				currentParameter += ParameterStep;
			}
        }
		#endregion Methods
	}
}
