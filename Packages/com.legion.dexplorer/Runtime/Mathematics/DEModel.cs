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
		private Allocator _allocator;
		// Variables container
		private NativeArray<float> _data;
		// Temporary variable container
		private NativeArray<float> _temporaryData;
		#endregion Fields

		#region Accessors
		public NativeArray<float> Data { get { return _data; } }
		public NativeArray<float> TemporaryData { get { return _temporaryData; } }
		#endregion Accessors

		#region Constructors
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="dataNb">Variable number</param>
		/// <param name="temporaryDataNb">Temporary data number</param>
		/// <param name="allocator">Allocation type</param>
		public DEModel(int dataNb, int temporaryDataNb, Allocator allocator)
		{
			_allocator = allocator;
			_data = new NativeArray<float>(dataNb, allocator, NativeArrayOptions.ClearMemory);
			_temporaryData = new NativeArray<float>(temporaryDataNb, allocator, NativeArrayOptions.ClearMemory);
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
		/// Get the temporary variable value.
		/// </summary>
		/// <param name="index">Index of the temporary variable in the data container</param>
		/// <returns>Value of the temporary variable</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float GetTemporaryDataValue(int index)
		{
			return _temporaryData[index];
		}

		/// <summary>
		/// Set the temporary variable value
		/// </summary>
		/// <param name="index">Index of the temporary variable in the data container</param>
		/// <param name="value">Value of the temporary variable</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetTemporaryDataValue(int index, float value)
		{
			_temporaryData[index] = value;
		}

		/// <summary>
		/// Duplicate temporary variables container
		/// </summary>
		/// <returns>Duplicated temporary variables container</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public NativeArray<float> DuplicateTemporaryData()
		{
			return new NativeArray<float>(_temporaryData.ToArray(), _allocator);
		}

		/// <summary>
		/// Dispose the data in the unmanaged memory.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Dispose()
		{
			if (_data.IsCreated)
			{
				_data.Dispose();
			}
			
			if (_temporaryData.IsCreated)
			{
				_temporaryData.Dispose();
			}
		}
		#endregion Methods
	}
}
