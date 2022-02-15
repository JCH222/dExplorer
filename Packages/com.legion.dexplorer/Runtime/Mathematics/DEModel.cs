namespace dExplorer.Runtime.Mathematics
{
	using System.Runtime.CompilerServices;
	using Unity.Collections;

	/// <summary>
	/// Differential equation model basic structure.
	/// </summary>
	public struct DEModel
	{
		#region Fields
		// Variables container
		private NativeArray<float> _data;
		#endregion Fields

		#region Accessors
		public NativeArray<float> Data { get { return _data; } }
		#endregion Accessors

		#region Constructors
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="dataNb">Variable number</param>
		/// <param name="allocator">Allocation type</param>
		public DEModel(int dataNb, Allocator allocator)
		{
			_data = new NativeArray<float>(dataNb, allocator, NativeArrayOptions.ClearMemory);
		}
		#endregion Constructors

		#region Methods
		/// <summary>
		/// Get the variable value.
		/// </summary>
		/// <param name="index">Index of the variable in the data container</param>
		/// <returns>Value of the variable</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float GetDataValue(int index)
		{
			return _data[index];
		}

		/// <summary>
		/// Set the variable value
		/// </summary>
		/// <param name="index">Index of the variable in the data container</param>
		/// <param name="value">Value of the variable</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetDataValue(int index, float value)
		{
			_data[index] = value;
		}

		/// <summary>
		/// Dispose the data in the unmanaged memory.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Dispose()
		{
			_data.Dispose();
		}
		#endregion Methods
	}
}
