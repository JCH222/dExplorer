using dExplorer.Editor.Mathematics;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class SpringModelVisualizer : AnalysableDEModelVisualizer
{
    #region Fields
    private FloatField _massField;
    private FloatField _stiffnessField;
    private FloatField _neutralLengthField;
    private FloatField _initialLengthField;
    private FloatField _initialSpeedField;
	#endregion Fields

	#region Methods
	public override string GetName()
	{
        return "Spring";
	}

	public override AnalysableDEModel InstantiateModel()
    {
        return new SpringModel();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnMassChanged(ChangeEvent<float> evt)
    {
        if (_model != null)
        {
            SpringModel model = (_model as SpringModel);
            model.Mass = math.max(0.0f, evt.newValue);
            _massField.value = model.Mass;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnStiffnessChanged(ChangeEvent<float> evt)
    {
        if (_model != null)
        {
            SpringModel model = (_model as SpringModel);
            model.Stiffness = math.max(0.0f, evt.newValue);
            _stiffnessField.value = model.Stiffness;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnNeutralLengthChanged(ChangeEvent<float> evt)
    {
        if (_model != null)
        {
            SpringModel model = (_model as SpringModel);
            model.NeutralLength = math.max(0.0f, evt.newValue);
            _neutralLengthField.value = model.NeutralLength;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnInitialLengthChanged(ChangeEvent<float> evt)
    {
        if (_model != null)
        {
            SpringModel model = (_model as SpringModel);
            model.InitialLength = evt.newValue;
            _initialLengthField.value = model.InitialLength;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnInitialSpeedChanged(ChangeEvent<float> evt)
    {
        if (_model != null)
        {
            SpringModel model = (_model as SpringModel);
            model.InitialSpeed = evt.newValue;
            _initialSpeedField.value = model.InitialSpeed;
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

        _stiffnessField = new FloatField("Stiffness [N/m]");
        _stiffnessField.RegisterValueChangedCallback(OnStiffnessChanged);

        _neutralLengthField = new FloatField("Neutral Length [m]");
        _neutralLengthField.RegisterValueChangedCallback(OnNeutralLengthChanged);

        _initialLengthField = new FloatField("Initial Length [m]");
        _initialLengthField.RegisterValueChangedCallback(OnInitialLengthChanged);

        _initialSpeedField = new FloatField("Initial Speed [m/s]");
        _initialSpeedField.RegisterValueChangedCallback(OnInitialSpeedChanged);

        Add(_massField);
        Add(_stiffnessField);
        Add(_neutralLengthField);
        Add(_initialLengthField);
        Add(_initialSpeedField);
    }
    #endregion Methods
}
