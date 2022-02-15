namespace dExplorer.Editor.Mathematics
{
	using Unity.Burst;
	using Unity.Collections;
	using Unity.Collections.LowLevel.Unsafe;
	using Unity.Jobs;
	using Unity.Mathematics;


	/// <summary>
	/// Analysis computation between the analytical solution and a dimension 1 differential equation simulation.
	/// </summary>
	[BurstCompile]
	public struct FloatDEAnalysisJob : IJob
	{
		#region Fields
		[ReadOnly] public NativeArray<float> ExactValues;
		[ReadOnly] public NativeArray<float> Approximations;

		[NativeDisableUnsafePtrRestriction]
		[WriteOnly] public unsafe float* MeanAbsoluteErrorPtr;
		#endregion Fields

		#region Methods
		public unsafe void Execute()
		{
			*MeanAbsoluteErrorPtr = 0.0f;

			int valueNb = ExactValues.Length;

			for (int i = 0; i < valueNb; i++)
			{
				float approximation = Approximations[i];
				float exactValue = ExactValues[i];

				float absoluteError = math.abs(exactValue - approximation);

				*MeanAbsoluteErrorPtr += absoluteError;
			}

			*MeanAbsoluteErrorPtr /= (float)valueNb;
		}
		#endregion Methods
	}
}
