namespace dExplorer.Editor.Mathematics
{
    public class EmptyAnalysableDEModelVisualizer : AnalysableDEModelVisualizer
    {
		#region Methods
		public override string GetName()
		{
            return "NONE";
		}

		public override AnalysableDEModel InstantiateModel()
        {
            return null;
        }

		public override void Init()
		{
			base.Init();

            _saveFolderSelectionButton.SetEnabled(false);
            _saveFolderSelectionButton.visible = false;

            _reportNameField.SetEnabled(false);
            _reportNameField.visible = false;

            _solvingTypesField.SetEnabled(false);
            _solvingTypesField.visible = false;

            _minParameterField.SetEnabled(false);
            _minParameterField.visible = false;

            _maxParameterField.SetEnabled(false);
            _maxParameterField.visible = false;

            _maxParameterStepField.SetEnabled(false);
            _maxParameterStepField.visible = false;

            _minParameterStepField.SetEnabled(false);
            _minParameterStepField.visible = false;

            _samplingFrequencyField.SetEnabled(false);
            _samplingFrequencyField.visible = false;
        }
		#endregion Methods
	}
}
