using dExplorer.Editor.Mathematics;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class SimpleCompressionModelVisualizer : AnalysableDEModelVisualizer
{
    #region Fields
    private FloatField _massField;
    private FloatField _surfaceField;
    private FloatField _heatCapacityRatioField;
    private FloatField _incompressibleLengthField;
    private FloatField _maxCompressibleLengthField;
    private FloatField _outerPressureField;
    private FloatField _initialPressureField;
    private FloatField _initialLengthField;
    private FloatField _initialSpeedField;
    #endregion Fields

    #region Methods
    public override string GetName()
    {
        return "Adiabatic compression";
    }

    public override AnalysableDEModel InstantiateModel()
    {
        return new SimpleCompressionModel();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnMassChanged(ChangeEvent<float> evt)
    {
        if (_model != null)
        {
            SimpleCompressionModel model = (_model as SimpleCompressionModel);
            model.Mass = math.max(0.0f, evt.newValue);
            _massField.value = model.Mass;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnSurfaceChanged(ChangeEvent<float> evt)
    {
        if (_model != null)
        {
            SimpleCompressionModel model = (_model as SimpleCompressionModel);
            model.Surface = math.max(0.0f, evt.newValue);
            _surfaceField.value = model.Surface;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnHeatCapacityRatioChanged(ChangeEvent<float> evt)
    {
        if (_model != null)
        {
            SimpleCompressionModel model = (_model as SimpleCompressionModel);
            model.HeatCapacityRatio = math.max(0.0f, evt.newValue);
            _heatCapacityRatioField.value = model.HeatCapacityRatio;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnIncompressibleLengthChanged(ChangeEvent<float> evt)
    {
        if (_model != null)
        {
            SimpleCompressionModel model = (_model as SimpleCompressionModel);
            model.IncompressibleLength = math.min(0.0f, evt.newValue);
            _incompressibleLengthField.value = model.IncompressibleLength;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnMaxCompressibleLengthChanged(ChangeEvent<float> evt)
    {
        if (_model != null)
        {
            SimpleCompressionModel model = (_model as SimpleCompressionModel);
            model.MaxCompressibleLength = math.max(0.0f, evt.newValue);
            _maxCompressibleLengthField.value = model.MaxCompressibleLength;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnOuterPressureChanged(ChangeEvent<float> evt)
    {
        if (_model != null)
        {
            SimpleCompressionModel model = (_model as SimpleCompressionModel);
            model.OuterPressure = math.max(0.0f, evt.newValue);
            _outerPressureField.value = model.OuterPressure;
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnInitialPressureChanged(ChangeEvent<float> evt)
    {
        if (_model != null)
        {
            SimpleCompressionModel model = (_model as SimpleCompressionModel);
            model.InitialPressure = math.max(0.0f, evt.newValue);
            _initialPressureField.value = model.InitialPressure;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnInitialLengthChanged(ChangeEvent<float> evt)
    {
        if (_model != null)
        {
            SimpleCompressionModel model = (_model as SimpleCompressionModel);
            model.InitialLength = math.max(0.0f, evt.newValue);
            _initialLengthField.value = model.InitialLength;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnInitialSpeedChanged(ChangeEvent<float> evt)
    {
        if (_model != null)
        {
            SimpleCompressionModel model = (_model as SimpleCompressionModel);
            model.InitialSpeed = evt.newValue;
        }
    }

    public override void Init()
    {
        base.Init();

        _fullReportOption.value = true;
        _fullReportOption.SetEnabled(false);

        _minParameterField.label = "Initial Time [s]";
        _maxParameterField.label = "Final Time [s]";
        _maxParameterStepField.label = "Max Time Step [s]";
        _minParameterStepField.label = "Min Time Step [s]";

        _massField = new FloatField("Mass [kg]");
        _massField.RegisterValueChangedCallback(OnMassChanged);

        _surfaceField = new FloatField("Surface [m^2]");
        _surfaceField.RegisterValueChangedCallback(OnSurfaceChanged);

        _heatCapacityRatioField = new FloatField("Heat Capacity Ratio [N.A]");
        _heatCapacityRatioField.RegisterValueChangedCallback(OnHeatCapacityRatioChanged);

        _incompressibleLengthField = new FloatField("Incompressible Length [m]");
        _incompressibleLengthField.RegisterValueChangedCallback(OnIncompressibleLengthChanged);

        _maxCompressibleLengthField = new FloatField("Max Compressible Length [m]");
        _maxCompressibleLengthField.RegisterValueChangedCallback(OnMaxCompressibleLengthChanged);

        _outerPressureField = new FloatField("Outer Pressure [N/m^2]");
        _outerPressureField.RegisterValueChangedCallback(OnOuterPressureChanged);

        _initialPressureField = new FloatField("Initial Pressure [N/m^2]");
        _initialPressureField.RegisterValueChangedCallback(OnInitialPressureChanged);

        _initialLengthField = new FloatField("Initial Length [m]");
        _initialLengthField.RegisterValueChangedCallback(OnInitialLengthChanged);

        _initialSpeedField = new FloatField("Initial Speed [m/s]");
        _initialSpeedField.RegisterValueChangedCallback(OnInitialSpeedChanged);

        Add(_massField);
        Add(_surfaceField);
        Add(_heatCapacityRatioField);
        Add(_incompressibleLengthField);
        Add(_maxCompressibleLengthField);
        Add(_outerPressureField);
        Add(_initialPressureField);
        Add(_initialLengthField);
        Add(_initialSpeedField);
    }
    #endregion Methods
}
