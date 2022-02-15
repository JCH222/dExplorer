namespace dExplorer.Editor.Mathematics
{
	using Unity.Burst;
	using Unity.Collections;
	using Unity.Collections.LowLevel.Unsafe;
	using Unity.Jobs;
	using Unity.Mathematics;

	/// <summary>
	/// Analysis computation between the analytical solution and a dimension 2 differential equation simulation.
	/// </summary>
	[BurstCompile]
	public struct Float2DEAnalysisJob : IJob
	{
		#region Fields
		[ReadOnly] public NativeArray<float2> ExactValues;
		[ReadOnly] public NativeArray<float2> Approximations;

		[NativeDisableUnsafePtrRestriction] 
		[WriteOnly] public unsafe float2* MeanAbsoluteErrorPtr;
		#endregion Fields

		#region Methods
		public unsafe void Execute()
		{
			*MeanAbsoluteErrorPtr = 0.0f;

			int valueNb = ExactValues.Length;

			for (int i = 0; i < valueNb; i++)
			{
				float2 approximation = Approximations[i];
				float2 exactValue = ExactValues[i];

				float2 absoluteError = math.abs(exactValue - approximation);

				*MeanAbsoluteErrorPtr += absoluteError;
			}

			*MeanAbsoluteErrorPtr /= (float)valueNb;
		}
		#endregion Methods
	}
}
