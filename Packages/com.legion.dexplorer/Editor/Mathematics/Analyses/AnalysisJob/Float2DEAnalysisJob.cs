namespace dExplorer.Editor.Mathematics
{
	using Unity.Burst;
	using Unity.Collections;
	using Unity.Collections.LowLevel.Unsafe;
	using Unity.Jobs;
	using Unity.Mathematics;
	using UnityEngine;

	/// <summary>
	/// Analysis computation between the analytical solution and a dimension 2 differential equation simulation.
	/// </summary>
	[BurstCompile]
	public struct Float2DEAnalysisJob : IJob
	{
		#region Fields
		[ReadOnly] public NativeArray<Vector2> ExactValues;
		[ReadOnly] public NativeArray<Vector2> Approximations;

		[NativeDisableUnsafePtrRestriction] 
		[WriteOnly] public unsafe Vector2* MeanAbsoluteErrorPtr;
		#endregion Fields

		#region Methods
		public unsafe void Execute()
		{
			*MeanAbsoluteErrorPtr = Vector2.zero;

			int valueNb = ExactValues.Length;

			for (int i = 0; i < valueNb; i++)
			{
				Vector2 approximation = Approximations[i];
				Vector2 exactValue = ExactValues[i];

				Vector2 absoluteError = math.abs(exactValue - approximation);

				*MeanAbsoluteErrorPtr += absoluteError;
			}

			*MeanAbsoluteErrorPtr /= (float)valueNb;
		}
		#endregion Methods
	}
}
