namespace dExplorer.Runtime.Mathematics
{
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;

	/// <summary>
	/// Base structure of the unit value in the differential equation analysis report.
	/// </summary>
	/// <typeparam name="T_VARIABLE">Variable type</typeparam>
	/// <typeparam name="T_ADDITIONAL_VALUE">Additional Values container type</typeparam>
	public interface ISimulationValue<T_VARIABLE, T_ADDITIONAL_VALUE>
		where T_VARIABLE : struct
		where T_ADDITIONAL_VALUE : struct
	{
		#region Properties
		public DESolvingType SolvingType { get; set; }
		public float ParameterStep { get; set; }
		public float Parameter { get; set; }
		public T_VARIABLE Value { get; set; }
		public T_ADDITIONAL_VALUE AdditionalValue { get; set; }
		#endregion Properties
	}

	/// <summary>
	/// Differential equation simulation report.
	/// </summary>
	/// <typeparam name="T_SIMULATION_VALUE">Simulation value type</typeparam>
	/// <typeparam name="T_ADDITIONAL_VALUE">Additional value type</typeparam>
	/// <typeparam name="T_VARIABLE">Variable type</typeparam>
	public abstract class DESimulationReport<T_SIMULATION_VALUE, T_VARIABLE, T_ADDITIONAL_VALUE> : DEReport
		where T_SIMULATION_VALUE : ISimulationValue<T_VARIABLE, T_ADDITIONAL_VALUE>
		where T_VARIABLE : struct
		where T_ADDITIONAL_VALUE : struct
	{
		#region Fields
		protected List<T_SIMULATION_VALUE> _data;
		#endregion Fields

		#region Constructors
		/// <summary>
		/// Constructor.
		/// </summary>
		public DESimulationReport() : base()
		{
			_data = new List<T_SIMULATION_VALUE>();
		}
		#endregion Constructors

		#region Methods
		/// <summary>
		/// Callback called before Unity serializes the report.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void OnBeforeSerialize()
		{
			base.PreSerialize();
		}

		/// <summary>
		/// Callback called after Unity deserializes the report.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void OnAfterDeserialize()
		{
			base.PostDeserialize();
		}
		#endregion Methods
	}
}
