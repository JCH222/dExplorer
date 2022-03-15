namespace dExplorer.Editor.Serializations
{
	using dExplorer.Runtime.Mathematics;
	using System;
	using System.Collections.Generic;

	public interface IDEAnalysisReportSerializable<T_VARIABLE>
		where T_VARIABLE : struct
	{
		#region Methods
		public string GetName();
		public string GetShortDescription();
		public string GetLongDescription();
		public DateTime GetCreationDate();
		public IEnumerable<DESolvingType> GetSolvingTypes();
		public IEnumerable<Tuple<float, T_VARIABLE>> GetMeanAbsoluteErrors(DESolvingType solvingType); 
		public IEnumerable<Tuple<float, T_VARIABLE>> GetSimulationValues(DESolvingType solvingType, int index);
		#endregion Methods
	}
}
