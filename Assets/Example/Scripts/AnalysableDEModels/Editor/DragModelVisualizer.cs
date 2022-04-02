using dExplorer.Editor.Mathematics;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class DragModelVisualizer : AnalysableDEModelVisualizer
{
    #region Fields
    private FloatField _massField;
    private FloatField _fluidDensityField;
    private FloatField _referenceSurfaceField;
    private FloatField _dragCoefficientField;
    private FloatField _initialSpeedField;
    private FloatField _additionalForceField;
    #endregion Fields

    #region Methods
    public override string GetName()
    {
        return "Drag";
    }

    public override AnalysableDEModel InstantiateModel()
    {
        return new DragModel();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnMassChanged(ChangeEvent<float> evt)
    {
        if (_model != null)
		{
            DragModel model = (_model as DragModel);
            model.Mass = math.max(0.0f, evt.newValue);
            _massField.value = model.Mass;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnFluidDensityChanged(ChangeEvent<float> evt)
    {
        if (_model != null)
        {
            DragModel model = (_model as DragModel);
            model.FluidDensity = math.max(0.0f, evt.newValue);
            _fluidDensityField.value = model.FluidDensity;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnReferenceSurfaceChanged(ChangeEvent<float> evt)
    {
        if (_model != null)
        {
            DragModel model = (_model as DragModel);
            model.ReferenceSurface = math.max(0.0f, evt.newValue);
            _referenceSurfaceField.value = model.ReferenceSurface;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnDragCoefficientChanged(ChangeEvent<float> evt)
    {
        if (_model != null)
        {
            DragModel model = (_model as DragModel);
            model.DragCoefficient = math.max(0.0f, evt.newValue);
            _dragCoefficientField.value = model.DragCoefficient;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnInitialSpeedChanged(ChangeEvent<float> evt)
    {
        if (_model != null)
        {
            DragModel model = (_model as DragModel);
            model.InitialSpeed = evt.newValue;
            _initialSpeedField.value = model.InitialSpeed;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnAdditionalForceChanged(ChangeEvent<float> evt)
    {
        if (_model != null)
        {
            DragModel model = (_model as DragModel);
            model.AdditionalForce = evt.newValue;
            _additionalForceField.value = model.AdditionalForce;
        }
    }

    public override void Init()
	{
		base.Init();

        _minParameterField.label = "Initial Time [s]";
        _maxParameterField.label = "Final Time [s]";
        _maxParameterStepField.label = "Max Time Step [s]";
        _minParameterStepField.label = "Min Time Step [s]";

        _massField = new FloatField("Mass [kg]");
        _massField.RegisterValueChangedCallback(OnMassChanged);

        _fluidDensityField = new FloatField("Density [kg/m^3]");
        _fluidDensityField.RegisterValueChangedCallback(OnFluidDensityChanged);

        _referenceSurfaceField = new FloatField("Reference Surface [m^2]");
        _referenceSurfaceField.RegisterValueChangedCallback(OnReferenceSurfaceChanged);

        _dragCoefficientField = new FloatField("Drag Coefficient");
        _dragCoefficientField.RegisterValueChangedCallback(OnDragCoefficientChanged);

        _initialSpeedField = new FloatField("Initial Speed [m/s]");
        _initialSpeedField.RegisterValueChangedCallback(OnInitialSpeedChanged);

        _additionalForceField = new FloatField("Additional Force [N]");
        _additionalForceField.RegisterValueChangedCallback(OnAdditionalForceChanged);

        Add(_massField);
        Add(_fluidDensityField);
        Add(_referenceSurfaceField);
        Add(_dragCoefficientField);
        Add(_initialSpeedField);
        Add(_additionalForceField);
    }
	#endregion Methods
}