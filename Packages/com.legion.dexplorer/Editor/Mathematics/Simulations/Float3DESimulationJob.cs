namespace dExplorer.Editor.Mathematics
{
	using dExplorer.Runtime.Mathematics;
	using Unity.Burst;
	using Unity.Collections;
	using Unity.Collections.LowLevel.Unsafe;
	using Unity.Jobs;
	using Unity.Mathematics;
	using UnityEngine;

	public unsafe delegate void Float3InitialVariableFunction(float* modelData, float* modelTemporaryData, float3* initialVariable);
	public unsafe delegate void Float3PreSimulationFunction(float* modelData, float* modelTemporaryData, float3* currentVariable, float* currentParameter);
	public unsafe delegate void Float3PostSimulationFunction(float* modelData, float* modelTemporaryData, float3* nextVariable, float3* exportedNextVariable);
	public unsafe delegate void Float3DerivativeFunction(float* modelData, float* modelTemporaryData, float3* currentVariable, float currentParameter, float3* currentDerivative);
	public unsafe delegate void Float3AnalyticalSolutionFunction(float* modelData, float* modelTemporaryData, float currentParameter, float3* currentVariable);
	public unsafe delegate void Float3VariableDimensionalizationFunction(float* modelData, float* modelTemporaryData, float3* nonDimensionalizedVariable, float3* dimensionalizedVariable);

	/// <summary>
	/// Dimension 3 differential equation simulation with specific solving type, duration and parameter step.
	/// </summary>
	[BurstCompile]
	public struct Float3DESimulationJob : IJob
	{
		#region Fields
		[ReadOnly] public bool IsNondimensionalized;
		[ReadOnly] public NativeArray<float> ModelData;
		[ReadOnly] public FunctionPointer<Float3InitialVariableFunction> InitialVariableFunctionPointer;
		[ReadOnly] public FunctionPointer<Float3PreSimulationFunction> PreSimulationFunctionPointer;
		[ReadOnly] public FunctionPointer<Float3PostSimulationFunction> PostSimulationFunctionPointer;
		[ReadOnly] public FunctionPointer<Float3DerivativeFunction> DerivativeFunctionPointer;
		[ReadOnly] public FunctionPointer<Float3AnalyticalSolutionFunction> AnalyticalSolutionFunctionPointer;
		[ReadOnly] public FunctionPointer<Float3VariableDimensionalizationFunction> VariableDimensionalizationFunctionPointer;
		[ReadOnly] public FunctionPointer<ParameterDimensionalizationFunction> ParameterDimensionalizationFunctionPointer;

		[ReadOnly] public float MinParameter;
		[ReadOnly] public float MaxParameter;
		[ReadOnly] public float ParameterStep;
		[ReadOnly] public DESolvingType SolvingType;

		[WriteOnly] public NativeArray<float> Parameter;
		[WriteOnly] public NativeArray<Vector3> Result;

		public NativeArray<float> ModelTemporaryData;
		#endregion Fields

		#region Methods
		public unsafe void Execute()
		{
			float* modelDataPtr = (float*)ModelData.GetUnsafeReadOnlyPtr();
			float* modelTemporaryDataPtr = (float*)ModelTemporaryData.GetUnsafePtr();

			float3 initialVariable;
			InitialVariableFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, &initialVariable);

			if (IsNondimensionalized)
			{
				float3 dimensionalizedVariable;
				Parameter[0] = ParameterDimensionalizationFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, MinParameter);
				VariableDimensionalizationFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, &initialVariable, &dimensionalizedVariable);
				Result[0] = dimensionalizedVariable;
			}
			else
			{
				Parameter[0] = MinParameter;
				Result[0] = initialVariable;
			}

			float3 currentVariable = initialVariable;

			int index = 1;
			float currentParameter = MinParameter;
			float currentLocalParameter = MinParameter;

			while (currentParameter < MaxParameter && index < Result.Length)
			{
				float3 nextVariable;

				PreSimulationFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, &currentVariable, &currentLocalParameter);

				switch (SolvingType)
				{
					case DESolvingType.ANALYTICAL:
						AnalyticalSolutionFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, currentLocalParameter + ParameterStep, &nextVariable);
						break;

					case DESolvingType.EXPLICIT_EULER:
						float3 currentDerivative;
						DerivativeFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, &currentVariable, currentLocalParameter, &currentDerivative);
						nextVariable = DESolver.SolveWithExplicitEuler(currentVariable, currentDerivative, ParameterStep);
						break;

					case DESolvingType.EXPLICIT_RUNGE_KUTTA_2:
						float3 rk2_k1;
						DerivativeFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, &currentVariable, currentLocalParameter, &rk2_k1);

						float3 tempVariable = DESolver.SolveWithExplicitEuler(currentVariable, rk2_k1, ParameterStep);
						float3 rk2_k2;
						DerivativeFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, &tempVariable, currentLocalParameter, &rk2_k2);

						nextVariable = DESolver.SolveWithExplicitRungeKutta2(currentVariable, rk2_k1, rk2_k2, ParameterStep);
						break;

					case DESolvingType.EXPLICIT_RUNGE_KUTTA_4:
						float halfParameterStep = 0.5f * ParameterStep;

						float3 rk4_k1;
						DerivativeFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, &currentVariable, currentLocalParameter, &rk4_k1);

						tempVariable = DESolver.SolveWithExplicitEuler(currentVariable, rk4_k1, halfParameterStep);
						float3 rk4_k2;
						DerivativeFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, &tempVariable, currentLocalParameter + halfParameterStep, &rk4_k2);

						tempVariable = DESolver.SolveWithExplicitEuler(currentVariable, rk4_k2, halfParameterStep);
						float3 rk4_k3;
						DerivativeFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, &tempVariable, currentLocalParameter + halfParameterStep, &rk4_k3);

						tempVariable = DESolver.SolveWithExplicitEuler(currentVariable, rk4_k3, ParameterStep);
						float3 rk4_k4;
						DerivativeFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, &tempVariable, currentLocalParameter + ParameterStep, &rk4_k4);

						nextVariable = DESolver.SolveWithExplicitRungeKutta4(currentVariable, rk4_k1, rk4_k2, rk4_k3, rk4_k4, ParameterStep);
						break;

					default:
						nextVariable = float.NaN;
						break;
				}

				float3 exportedNextVariable;
				PostSimulationFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, &nextVariable, &exportedNextVariable);

				currentParameter += ParameterStep;
				currentLocalParameter += ParameterStep;

				if (IsNondimensionalized)
				{
					float3 dimensionalizedVariable;
					Parameter[index] = ParameterDimensionalizationFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, currentParameter);
					VariableDimensionalizationFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, &exportedNextVariable, &dimensionalizedVariable);
					Result[index] = dimensionalizedVariable;
				}
				else
				{
					Parameter[index] = currentParameter;
					Result[index] = exportedNextVariable;
				}

				currentVariable = nextVariable;

				index++;
			}
		}
		#endregion Methods
	}
}
