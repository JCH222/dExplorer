namespace dExplorer.Editor.Mathematics
{
	using Unity.Burst;
	using Unity.Collections;
	using Unity.Collections.LowLevel.Unsafe;
	using Unity.Jobs;
	using Unity.Mathematics;
	using UnityEngine;

	/// <summary>
	/// Analysis computation between the analytical solution and a dimension 3 differential equation simulation.
	/// </summary>
	[BurstCompile]
	public struct Float3DEAnalysisJob : IJob
	{
		#region Fields
		[ReadOnly] public NativeArray<Vector3> ExactValues;
		[ReadOnly] public NativeArray<Vector3> Approximations;

		[NativeDisableUnsafePtrRestriction]
		[WriteOnly] public unsafe Vector3* MeanAbsoluteErrorPtr;
		#endregion Fields

		#region Methods
		public unsafe void Execute()
		{
			*MeanAbsoluteErrorPtr = Vector3.zero;

			int valueNb = ExactValues.Length;

			for (int i = 0; i < valueNb; i++)
			{
				Vector3 approximation = Approximations[i];
				Vector3 exactValue = ExactValues[i];

				Vector3 absoluteError = math.abs(exactValue - approximation);

				*MeanAbsoluteErrorPtr += absoluteError;
			}

			*MeanAbsoluteErrorPtr /= (float)valueNb;
		}
		#endregion Methods
	}
}
