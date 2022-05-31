using dExplorer.Editor.Mathematics;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class SimpleViscousFrictionModelVisualizer : AnalysableDEModelVisualizer
{
    #region Fields
    private FloatField _inertiaMomentField;
    private FloatField _radiusField;
    private FloatField _viscousFrictionCoefficientField;
    private FloatField _initialAngularSpeedField;
    #endregion Fields

    #region Methods
    public override string GetName()
    {
        return "Viscous friction";
    }

    public override AnalysableDEModel InstantiateModel()
    {
        return new SimpleViscousFrictionModel();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnInertiaMomentChanged(ChangeEvent<float> evt)
    {
        if (_model != null)
        {
            SimpleViscousFrictionModel model = (_model as SimpleViscousFrictionModel);
            model.InertiaMoment = math.max(0.0f, evt.newValue);
            _inertiaMomentField.value = model.InertiaMoment;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnRadiusChanged(ChangeEvent<float> evt)
    {
        if (_model != null)
        {
            SimpleViscousFrictionModel model = (_model as SimpleViscousFrictionModel);
            model.Radius = math.max(0.0f, evt.newValue);
            _radiusField.value = model.Radius;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnViscousFrictionCoefficientChanged(ChangeEvent<float> evt)
    {
        if (_model != null)
        {
            SimpleViscousFrictionModel model = (_model as SimpleViscousFrictionModel);
            model.ViscousFrictionCoefficient = math.max(0.0f, evt.newValue);
            _viscousFrictionCoefficientField.value = model.ViscousFrictionCoefficient;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnInitialAngularSpeedChanged(ChangeEvent<float> evt)
    {
        if (_model != null)
        {
            SimpleViscousFrictionModel model = (_model as SimpleViscousFrictionModel);
            model.InitialAngularSpeed = math.radians(math.max(0.0f, evt.newValue));
            _initialAngularSpeedField.value = math.degrees(model.InitialAngularSpeed);
        }
    }

    public override void Init()
    {
        base.Init();

        _minParameterField.label = "Initial Time [s]";
        _maxParameterField.label = "Final Time [s]";
        _maxParameterStepField.label = "Max Time Step [s]";
        _minParameterStepField.label = "Min Time Step [s]";

        _inertiaMomentField = new FloatField("Inertia Moment [kg.m^2]");
        _inertiaMomentField.RegisterValueChangedCallback(OnInertiaMomentChanged);

        _radiusField = new FloatField("Radius [m]");
        _radiusField.RegisterValueChangedCallback(OnRadiusChanged);

        _viscousFrictionCoefficientField = new FloatField("Viscous Friction Coefficient [N.s/m]");
        _viscousFrictionCoefficientField.RegisterValueChangedCallback(OnViscousFrictionCoefficientChanged);

        _initialAngularSpeedField = new FloatField("Initial Angular Speed [deg/s]");
        _initialAngularSpeedField.RegisterValueChangedCallback(OnInitialAngularSpeedChanged);

        Add(_inertiaMomentField);
        Add(_radiusField);
        Add(_viscousFrictionCoefficientField);
        Add(_initialAngularSpeedField);
    }
    #endregion Methods
}
