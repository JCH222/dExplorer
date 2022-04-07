namespace dExplorer.Editor.Mathematics
{
	using dExplorer.Runtime.Mathematics;
	using Unity.Burst;
	using Unity.Collections;
	using Unity.Collections.LowLevel.Unsafe;
	using Unity.Jobs;

	public unsafe delegate void FloatInitialVariableFunction(float* modelData, float* modelTemporaryData, float* initialVariable);
	public unsafe delegate void FloatPreSimulationFunction(float* modelData, float* modelTemporaryData, float* currentVariable, float* currentParameter);
	public unsafe delegate void FloatPostSimulationFunction(float* modelData, float* modelTemporaryData, float* nextVariable);
	public unsafe delegate void FloatDerivativeFunction(float* modelData, float* modelTemporaryData, float* currentVariable, float currentParameter, float* currentDerivative);
	public unsafe delegate void FloatAnalyticalSolutionFunction(float* modelData, float* modelTemporaryData, float currentParameter, float* currentVariable);
	public unsafe delegate void FloatVariableDimensionalizationFunction(float* modelData, float* modelTemporaryData, float* nonDimensionalizedVariable, float* dimensionalizedVariable);

	/// <summary>
	/// Dimension 1 differential equation simulation with specific solving type, duration and parameter step.
	/// </summary>
	[BurstCompile]
	public struct FloatDESimulationJob : IJob
	{
		#region Fields
		[ReadOnly] public bool IsNondimensionalized;
		[ReadOnly] public NativeArray<float> ModelData;
		[ReadOnly] public FunctionPointer<FloatInitialVariableFunction> InitialVariableFunctionPointer;
		[ReadOnly] public FunctionPointer<FloatPreSimulationFunction> PreSimulationFunctionPointer;
		[ReadOnly] public FunctionPointer<FloatPostSimulationFunction> PostSimulationFunctionPointer;
		[ReadOnly] public FunctionPointer<FloatDerivativeFunction> DerivativeFunctionPointer;
		[ReadOnly] public FunctionPointer<FloatAnalyticalSolutionFunction> AnalyticalSolutionFunctionPointer;
		[ReadOnly] public FunctionPointer<FloatVariableDimensionalizationFunction> VariableDimensionalizationFunctionPointer;
		[ReadOnly] public FunctionPointer<ParameterDimensionalizationFunction> ParameterDimensionalizationFunctionPointer;

		[ReadOnly] public float MinParameter;
		[ReadOnly] public float MaxParameter;
		[ReadOnly] public float ParameterStep;
		[ReadOnly] public DESolvingType SolvingType;

		[WriteOnly] public NativeArray<float> Parameter;
		[WriteOnly] public NativeArray<float> Result;

		public NativeArray<float> ModelTemporaryData;
		#endregion Fields

		#region Methods
		public unsafe void Execute()
		{
			float* modelDataPtr = (float*)ModelData.GetUnsafeReadOnlyPtr();
			float* modelTemporaryDataPtr = (float*)ModelTemporaryData.GetUnsafePtr();

			float initialVariable;
			InitialVariableFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, &initialVariable);

			if (IsNondimensionalized)
			{
				float dimensionalizedVariable;
				Parameter[0] = ParameterDimensionalizationFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, MinParameter);
				VariableDimensionalizationFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, &initialVariable, &dimensionalizedVariable);
				Result[0] = dimensionalizedVariable;
			}
			else
			{
				Parameter[0] = MinParameter;
				Result[0] = initialVariable;
			}
			
			float currentVariable = initialVariable;

			int index = 1;
			float currentParameter = MinParameter;
			float currentLocalParameter = MinParameter;

			while (currentParameter < MaxParameter && index < Result.Length)
			{
				float nextVariable;
				float tempVariable;

				PreSimulationFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, &currentVariable, &currentLocalParameter);

				switch (SolvingType)
				{
					case DESolvingType.ANALYTICAL:
						AnalyticalSolutionFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, currentLocalParameter + ParameterStep, &nextVariable);
						break;

					case DESolvingType.EXPLICIT_EULER:
						float currentDerivative;
						DerivativeFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, &currentVariable, currentLocalParameter, &currentDerivative);
						nextVariable = DESolver.SolveWithExplicitEuler(currentVariable, currentDerivative, ParameterStep);
						break;

					case DESolvingType.EXPLICIT_RUNGE_KUTTA_2:
						float rk2_k1;
						DerivativeFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, &currentVariable, currentLocalParameter, &rk2_k1);

						tempVariable = DESolver.SolveWithExplicitEuler(currentVariable, rk2_k1, ParameterStep);
						float rk2_k2;
						DerivativeFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, &tempVariable, currentLocalParameter + ParameterStep, &rk2_k2);

						nextVariable = DESolver.SolveWithExplicitRungeKutta2(currentVariable, rk2_k1, rk2_k2, ParameterStep);
						break;

					case DESolvingType.EXPLICIT_RUNGE_KUTTA_4:
						float halfParameterStep = 0.5f * ParameterStep;

						float rk4_k1;
						DerivativeFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, &currentVariable, currentLocalParameter, &rk4_k1);
						
						tempVariable = DESolver.SolveWithExplicitEuler(currentVariable, rk4_k1, halfParameterStep);
						float rk4_k2;
						DerivativeFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr , &tempVariable, currentLocalParameter + halfParameterStep, &rk4_k2);

						tempVariable = DESolver.SolveWithExplicitEuler(currentVariable, rk4_k2, halfParameterStep);
						float rk4_k3;
						DerivativeFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, &tempVariable, currentLocalParameter + halfParameterStep, &rk4_k3);

						tempVariable = DESolver.SolveWithExplicitEuler(currentVariable, rk4_k3, ParameterStep);
						float rk4_k4;
						DerivativeFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, &tempVariable, currentLocalParameter + ParameterStep, &rk4_k4);

						nextVariable = DESolver.SolveWithExplicitRungeKutta4(currentVariable, rk4_k1, rk4_k2, rk4_k3, rk4_k4, ParameterStep);
						break;

					default:
						nextVariable = float.NaN;
						break;
				}

				float modifiedNextVariable = nextVariable;
				PostSimulationFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, &modifiedNextVariable);

				currentParameter += ParameterStep;
				currentLocalParameter += ParameterStep;

				if (IsNondimensionalized)
				{
					float dimensionalizedVariable;
					Parameter[index] = ParameterDimensionalizationFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, currentParameter);
					VariableDimensionalizationFunctionPointer.Invoke(modelDataPtr, modelTemporaryDataPtr, &modifiedNextVariable, &dimensionalizedVariable);
					Result[index] = dimensionalizedVariable;
				}
				else
				{
					Parameter[index] = currentParameter;
					Result[index] = modifiedNextVariable;
				}
				
				currentVariable = nextVariable;

				index++;
			}
		}
		#endregion Methods
	}
}
