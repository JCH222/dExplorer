namespace dExplorer.Runtime.Mathematics
{
	using System.Runtime.CompilerServices;
	using Unity.Mathematics;

	/// <summary>
	/// Differential equation solving type.
	/// </summary>
	public enum DESolvingType
	{
		ANALYTICAL = 0,
		EXPLICIT_EULER = 1,
		EXPLICIT_RUNGE_KUTTA_2 = 2,
		EXPLICIT_RUNGE_KUTTA_4 = 3
	}

	/// <summary>
	/// Differential equation solvers.
	/// </summary>
	public static class DESolver
	{
		#region Static Methods
		/// <summary>
		/// Solve first order differential equation for the next step with the explicit Euler method.
		/// </summary>
		/// <param name="currentVariable">Current value of the variable</param>
		/// <param name="currentDerivative">Current value of the derivative</param>
		/// <param name="step">Simulation step</param>
		/// <returns>Next value of the variable</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float SolveWithExplicitEuler(float currentVariable, float currentDerivative, float step)
		{
			return currentVariable + step * currentDerivative;
		}

		/// <summary>
		/// Solve second order differential equation for the next step with the explicit Euler method.
		/// </summary>
		/// <param name="currentVariable">Current value of the variable</param>
		/// <param name="currentDerivative">Current value of the derivative</param>
		/// <param name="step">Simulation step</param>
		/// <returns>Next value of the variable and the derivative</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 SolveWithExplicitEuler(float2 currentVariable, float2 currentDerivative, float step)
		{
			return currentVariable + step * currentDerivative;
		}

		/// <summary>
		/// Solve second order differential equation for the next step with the explicit Euler method.
		/// </summary>
		/// <param name="currentVariable">Current value of the variable</param>
		/// <param name="currentDerivative">Current value of the derivative</param>
		/// <param name="step">Simulation step</param>
		/// <returns>Next value of the variable and the derivative</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 SolveWithExplicitEuler(float3 currentVariable, float3 currentDerivative, float step)
		{
			return currentVariable + step * currentDerivative;
		}

		/// <summary>
		/// Solve second order differential equation for the next step with the explicit second order Runge-Kutta method.
		/// </summary>
		/// <param name="currentVariable">Current value of the variable</param>
		/// <param name="k1">Current value of the derivative</param>
		/// <param name="k2">Next value of the derivative (at time + step) computed with explicit Euler method (k1 as drivative)</param>
		/// <param name="step">Simulation step</param>
		/// <returns>Next value of the variable</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float SolveWithExplicitRungeKutta2(float currentVariable, float k1, float k2, float step)
		{
			return currentVariable + step * (0.5f * k1 + 0.5f * k2);
		}

		/// <summary>
		/// Solve second order differential equation for the next step with the explicit second order Runge-Kutta method.
		/// </summary>
		/// <param name="currentVariable">Current value of the variable</param>
		/// <param name="k1">Current value of the derivative</param>
		/// <param name="k2">Next value of the derivative (at time + step) computed with explicit Euler method (k1 as drivative)</param>
		/// <param name="step">Simulation step</param>
		/// <returns>Next value of the variable</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 SolveWithExplicitRungeKutta2(float2 currentVariable, float2 k1, float2 k2, float step)
		{
			return currentVariable + step * (0.5f * k1 + 0.5f * k2);
		}

		/// <summary>
		/// Solve second order differential equation for the next step with the explicit second order Runge-Kutta method.
		/// </summary>
		/// <param name="currentVariable">Current value of the variable</param>
		/// <param name="k1">Current value of the derivative</param>
		/// <param name="k2">Next value of the derivative (at time + step) computed with explicit Euler method (k1 as drivative)</param>
		/// <param name="step">Simulation step</param>
		/// <returns>Next value of the variable</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 SolveWithExplicitRungeKutta2(float3 currentVariable, float3 k1, float3 k2, float step)
		{
			return currentVariable + step * (0.5f * k1 + 0.5f * k2);
		}

		/// <summary>
		/// Solve first order differential equation for the next step with the explicit fourth order Runge-Kutta method.
		/// </summary>
		/// <param name="currentVariable">Current value of the variable</param>
		/// <param name="k1">Current value of the derivative</param>
		/// <param name="k2">Value of the derivative (at time + 0.5 * step) computed with explicit Euler method (k1 as drivative)</param>
		/// <param name="k3">Value of the derivative (at time + 0.5 * step) computed with explicit Euler method (k2 as drivative)</param>
		/// <param name="k4">Next value of the derivative (at time + step) computed with explicit Euler method (k3 as drivative)</param>
		/// <param name="step">Simulation step</param>
		/// <returns>Next value of the variable</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float SolveWithExplicitRungeKutta4(float currentVariable, float k1, float k2, float k3, float k4, float step)
		{
			return currentVariable + (1.0f / 6.0f) * step * (k1 + 2.0f * k2 + 2.0f * k3 + k4);
		}

		/// <summary>
		/// Solve first order differential equation for the next step with the explicit fourth order Runge-Kutta method.
		/// </summary>
		/// <param name="currentVariable">Current value of the variable</param>
		/// <param name="k1">Current value of the derivative</param>
		/// <param name="k2">Value of the derivative (at time + 0.5 * step) computed with explicit Euler method (k1 as drivative)</param>
		/// <param name="k3">Value of the derivative (at time + 0.5 * step) computed with explicit Euler method (k2 as drivative)</param>
		/// <param name="k4">Next value of the derivative (at time + step) computed with explicit Euler method (k3 as drivative)</param>
		/// <param name="step">Simulation step</param>
		/// <returns>Next value of the variable</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 SolveWithExplicitRungeKutta4(float2 currentVariable, float2 k1, float2 k2, float2 k3, float2 k4, float step)
		{
			return currentVariable + (1.0f / 6.0f) * step * (k1 + 2.0f * k2 + 2.0f * k3 + k4);
		}

		/// <summary>
		/// Solve first order differential equation for the next step with the explicit fourth order Runge-Kutta method.
		/// </summary>
		/// <param name="currentVariable">Current value of the variable</param>
		/// <param name="k1">Current value of the derivative</param>
		/// <param name="k2">Value of the derivative (at time + 0.5 * step) computed with explicit Euler method (k1 as drivative)</param>
		/// <param name="k3">Value of the derivative (at time + 0.5 * step) computed with explicit Euler method (k2 as drivative)</param>
		/// <param name="k4">Next value of the derivative (at time + step) computed with explicit Euler method (k3 as drivative)</param>
		/// <param name="step">Simulation step</param>
		/// <returns>Next value of the variable</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 SolveWithExplicitRungeKutta4(float3 currentVariable, float3 k1, float3 k2, float3 k3, float3 k4, float step)
		{
			return currentVariable + (1.0f / 6.0f) * step * (k1 + 2.0f * k2 + 2.0f * k3 + k4);
		}
		#endregion Static Methods
	}
}
