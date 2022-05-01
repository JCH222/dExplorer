using dExplorer.Editor.Mathematics;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class SimpleCrankshaftCompressionModelVisualizer : AnalysableDEModelVisualizer
{
    #region Fields
    private FloatField _surfaceField;
    private FloatField _heatCapacityRatioField;
    private FloatField _incompressibleLengthField;
    private FloatField _outerPressureField;
    private FloatField _initialPressureField;
    private FloatField _inertiaMomentField;
    private FloatField _crankRadiusField;
    private FloatField _connectingRodLengthField;
    private FloatField _initialAngularPositionField;
    private FloatField _initialAngularSpeedField;
    #endregion Fields

    #region Methods
    public override string GetName()
    {
        return "Crankshaft compression";
    }

    public override AnalysableDEModel InstantiateModel()
    {
        return new SimpleCrankshaftCompressionModel();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnSurfaceChanged(ChangeEvent<float> evt)
    {
        if (_model != null)
        {
            SimpleCrankshaftCompressionModel model = (_model as SimpleCrankshaftCompressionModel);
            model.Surface = math.max(0.0f, evt.newValue);
            _surfaceField.value = model.Surface;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnHeatCapacityRatioChanged(ChangeEvent<float> evt)
    {
        if (_model != null)
        {
            SimpleCrankshaftCompressionModel model = (_model as SimpleCrankshaftCompressionModel);
            model.HeatCapacityRatio = math.max(0.0f, evt.newValue);
            _heatCapacityRatioField.value = model.HeatCapacityRatio;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnIncompressibleLengthChanged(ChangeEvent<float> evt)
    {
        if (_model != null)
        {
            SimpleCrankshaftCompressionModel model = (_model as SimpleCrankshaftCompressionModel);
            model.IncompressibleLength = math.min(0.0f, evt.newValue);
            _incompressibleLengthField.value = model.IncompressibleLength;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnOuterPressureChanged(ChangeEvent<float> evt)
    {
        if (_model != null)
        {
            SimpleCrankshaftCompressionModel model = (_model as SimpleCrankshaftCompressionModel);
            model.OuterPressure = math.max(0.0f, evt.newValue);
            _outerPressureField.value = model.OuterPressure;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnInitialPressureChanged(ChangeEvent<float> evt)
    {
        if (_model != null)
        {
            SimpleCrankshaftCompressionModel model = (_model as SimpleCrankshaftCompressionModel);
            model.InitialPressure = math.max(0.0f, evt.newValue);
            _initialPressureField.value = model.InitialPressure;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnInertiaMomentChanged(ChangeEvent<float> evt)
    {
        if (_model != null)
        {
            SimpleCrankshaftCompressionModel model = (_model as SimpleCrankshaftCompressionModel);
            model.InertiaMoment = math.max(0.0f, evt.newValue);
            _inertiaMomentField.value = model.InertiaMoment;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnCrankRadiusChanged(ChangeEvent<float> evt)
    {
        if (_model != null)
        {
            SimpleCrankshaftCompressionModel model = (_model as SimpleCrankshaftCompressionModel);
            model.CrankRadius = math.max(0.0f, evt.newValue);
            _crankRadiusField.value = model.CrankRadius;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnConnectingRodLengthChanged(ChangeEvent<float> evt)
    {
        if (_model != null)
        {
            SimpleCrankshaftCompressionModel model = (_model as SimpleCrankshaftCompressionModel);
            model.ConnectingRodLength = math.max(0.0f, evt.newValue);
            _connectingRodLengthField.value = model.ConnectingRodLength;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnInitialAngularPositionChanged(ChangeEvent<float> evt)
    {
        if (_model != null)
        {
            SimpleCrankshaftCompressionModel model = (_model as SimpleCrankshaftCompressionModel);
            model.InitialAngularPosition = evt.newValue;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnInitialAngularSpeedChanged(ChangeEvent<float> evt)
    {
        if (_model != null)
        {
            SimpleCrankshaftCompressionModel model = (_model as SimpleCrankshaftCompressionModel);
            model.InitialAngularSpeed = evt.newValue;
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

        _surfaceField = new FloatField("Surface [m^2]");
        _surfaceField.RegisterValueChangedCallback(OnSurfaceChanged);

        _heatCapacityRatioField = new FloatField("Heat Capacity Ratio [N.A]");
        _heatCapacityRatioField.RegisterValueChangedCallback(OnHeatCapacityRatioChanged);

        _incompressibleLengthField = new FloatField("Incompressible Length [m]");
        _incompressibleLengthField.RegisterValueChangedCallback(OnIncompressibleLengthChanged);

        _outerPressureField = new FloatField("Outer Pressure [N/m^2]");
        _outerPressureField.RegisterValueChangedCallback(OnOuterPressureChanged);

        _initialPressureField = new FloatField("Initial Pressure [N/m^2]");
        _initialPressureField.RegisterValueChangedCallback(OnInitialPressureChanged);

        _inertiaMomentField = new FloatField("Inertia moment [kg.m^2]");
        _inertiaMomentField.RegisterValueChangedCallback(OnInertiaMomentChanged);

        _crankRadiusField = new FloatField("Crank Radius [m]");
        _crankRadiusField.RegisterValueChangedCallback(OnCrankRadiusChanged);

        _connectingRodLengthField = new FloatField("Connecting Rod Length [m]");
        _connectingRodLengthField.RegisterValueChangedCallback(OnConnectingRodLengthChanged);

        _initialAngularPositionField = new FloatField("Initial Angular Position [rad]");
        _initialAngularPositionField.RegisterValueChangedCallback(OnInitialAngularPositionChanged);

        _initialAngularSpeedField = new FloatField("Initial Angular Speed [rad/s]");
        _initialAngularSpeedField.RegisterValueChangedCallback(OnInitialAngularSpeedChanged);

        
        Add(_surfaceField);
        Add(_heatCapacityRatioField);
        Add(_incompressibleLengthField);
        Add(_outerPressureField);
        Add(_initialPressureField);
        Add(_inertiaMomentField);
        Add(_crankRadiusField);
        Add(_connectingRodLengthField);
        Add(_initialAngularPositionField);
        Add(_initialAngularSpeedField);
    }
    #endregion Methods
}
