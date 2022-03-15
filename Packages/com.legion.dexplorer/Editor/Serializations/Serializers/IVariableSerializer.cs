namespace dExplorer.Editor.Serializations
{
	public interface IVariableSerializer<T_VARIABLE> where T_VARIABLE : struct
	{
		#region Methods
		public void UpdateSerialization();
		#endregion Methods
	}
}
