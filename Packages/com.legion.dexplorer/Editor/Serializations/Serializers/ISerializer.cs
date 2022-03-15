namespace dExplorer.Editor.Serializations
{
	public interface ISerializer<T_VARIABLE, T_VARIABLE_SERIALIZER> 
		where T_VARIABLE : struct
		where T_VARIABLE_SERIALIZER : IVariableSerializer<T_VARIABLE>
	{
		#region Methods
		public void Serialize(IDEAnalysisReportSerializable<T_VARIABLE> report);
		#endregion Methods
	}
}