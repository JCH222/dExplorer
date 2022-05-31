using AOT;
using dExplorer.Editor.Mathematics;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

[BurstCompile]
public unsafe class SimpleTransmissionModel : AnalysableDEModel
{
	#region Properties
	public float OutputEngineInertiaMoment
	{
		get
		{
			return _model.GetDataValue(0);
		}
		set
		{
			_model.SetDataValue(0, value);
		}
	}

	public float OutputClutchInertiaMoment
	{
		get
		{
			return _model.GetDataValue(1);
		}
		set
		{
			_model.SetDataValue(1, value);
		}
	}

	public float OutputGearboxInertiaMoment
	{
		get
		{
			return _model.GetDataValue(2);
		}
		set
		{
			_model.SetDataValue(2, value);
		}
	}

	public float InitialEngineTorque
	{
		get
		{
			return _model.GetDataValue(3);
		}
		set
		{
			_model.SetDataValue(3, value);
		}
	}

	public float EngineAscendingSlope
	{
		get
		{
			return _model.GetDataValue(4);
		}
		set
		{
			_model.SetDataValue(4, value);
		}
	}

	public float EngineAscendingSlopeDuration
	{
		get
		{
			return _model.GetDataValue(5);
		}
		set
		{
			_model.SetDataValue(5, value);
		}
	}

	public float EngineConstantTorqueDuration
	{
		get
		{
			return _model.GetDataValue(6);
		}
		set
		{
			_model.SetDataValue(6, value);
		}
	}

	public float EngineDescendingSlope
	{
		get
		{
			return _model.GetDataValue(7);
		}
		set
		{
			_model.SetDataValue(7, value);
		}
	}

	public float EngineDescendingSlopeDuration
	{
		get
		{
			return _model.GetDataValue(8);
		}
		set
		{
			_model.SetDataValue(8, value);
		}
	}

	public float InitialEngineAngularSpeed
	{
		get
		{
			return _model.GetDataValue(9);
		}
		set
		{
			_model.SetDataValue(9, value);
		}
	}

	public float InitialClutchAngularSpeed
	{
		get
		{
			return _model.GetDataValue(10);
		}
		set
		{
			_model.SetDataValue(10, value);
		}
	}

	public override float MinParameter
	{
		get
		{
			return _minParameter;
		}
		set
		{
			_minParameter = 0.0f;
			_maxParameter = math.max(0.0f, _maxParameter);
		}
	}
	#endregion Properties

	#region Constructors
	public SimpleTransmissionModel(float outputEngineInertiaMoment = 1.0f, float outputClutchInertiaMoment = 1.0f, float outputGearboxInertiaMoment = 1.0f,
		float initialEngineTorque = 0.0f, float engineAscendingSlope = 0.0f, float engineAscendingSlopeDuration = 0.0f, float engineConstantTorqueDuration = 0.0f, 
		float engineDescendingSlope = 0.0f, float engineDescendingSlopeDuration = 0.0f, float initialEngineAngularSpeed = 0.0f, float initialClutchAngularSpeed = 0.0f) :
		base(11, 0, Allocator.Persistent, GetInitialVariable, PreSimulate, PostSimulate, ComputeDerivative, ComputeAnalyticalSolution)
	{
		OutputEngineInertiaMoment = outputEngineInertiaMoment;
		OutputClutchInertiaMoment = outputClutchInertiaMoment;
		OutputGearboxInertiaMoment = outputGearboxInertiaMoment;
		InitialEngineTorque = initialEngineTorque;
		EngineAscendingSlope = engineAscendingSlope;
		EngineAscendingSlopeDuration = engineAscendingSlopeDuration;
		EngineConstantTorqueDuration = engineConstantTorqueDuration;
		EngineDescendingSlope = engineDescendingSlope;
		EngineDescendingSlopeDuration = engineDescendingSlopeDuration;
		InitialEngineAngularSpeed = initialEngineAngularSpeed;
		InitialClutchAngularSpeed = initialClutchAngularSpeed;
	}
	#endregion Constructors

	#region Methods
	protected override void InitAnalysis() { }

	protected override void GenerateDefaultDescriptions(out string shortDescription, out string longDescription)
	{
		shortDescription = "Simple transmission model";
		longDescription =
			"Parameter :\n" +
			"-> Elapsed Time [s]\n\n" +
			"Parameter Step :\n" +
			"-> Time Step [s]\n\n" +
			"Variable :\n" +
			"-> Engine angular speed [rad/s]\n" +
			"-> Clutch angular speed [rad/s]\n" +
			"-> Gearbox angular speed [rad/s]\n\n" +
			"NB : There is no simple analytical solution";
	}
	#endregion Methods

	#region Static Methods
	[BurstCompile]
	[MonoPInvokeCallback(typeof(Float2PreSimulationFunction))]
	public static void PreSimulate(float* modelData, float* modelTemporaryData, float2* currentVariable, float* currentParameter) { }

	[BurstCompile]
	[MonoPInvokeCallback(typeof(Float2PostSimulationFunction))]
	public static void PostSimulate(float* modelData, float* modelTemporaryData, float2* nextVariable, float2* exportedNextVariable)
	{
		float maxCompressibleLength = modelData[4];
		float nextSpeed = (*nextVariable).x;
		float nextLength = (*nextVariable).y;

		if (nextLength <= 0.0f)
		{
			*nextVariable = new float2(0.0f, 0.0f);
		}
		else if (nextLength >= maxCompressibleLength)
		{
			*nextVariable = new float2(0.0f, maxCompressibleLength);
		}

		*exportedNextVariable = *nextVariable;
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(Float2InitialVariableFunction))]
	public static void GetInitialVariable(float* modelData, float* modelTemporaryData, float2* initialVariable)
	{
		float initialLength = modelData[7];
		float initialSpeed = modelData[8];
		*initialVariable = new float2(initialSpeed, initialLength);
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(Float2DerivativeFunction))]
	public static void ComputeDerivative(float* modelData, float* modelTemporaryData, float2* currentVariable, float currentParameter, float2* currentDerivative)
	{
		float coefficientA = modelTemporaryData[0];
		float mass = modelData[0];
		float surface = modelData[1];
		float heatCapacityRatio = modelData[2];
		float incompressibleLength = modelData[3];
		float outerPressure = modelData[5];

		float currentSpeed = (*currentVariable).x;
		float currentLength = (*currentVariable).y;

		*currentDerivative = new float2(
			(coefficientA / math.pow(currentLength - incompressibleLength, heatCapacityRatio)) - surface * outerPressure / mass,
			currentSpeed);
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(Float2AnalyticalSolutionFunction))]
	public static void ComputeAnalyticalSolution(float* modelData, float* modelTemporaryData, float currentParameter, float2* currentVariable)
	{
		// There is no simple analytical solution.
		*currentVariable = new float2(float.NaN, float.NaN);
	}
	#endregion Static Methods
}
