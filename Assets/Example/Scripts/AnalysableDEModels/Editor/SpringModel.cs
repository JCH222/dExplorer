using AOT;
using dExplorer.Editor.Mathematics;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

[BurstCompile]
public unsafe class SpringModel : AnalysableDEModel
{
	#region Properties
	public float Mass
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

	public float Stiffness
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

	public float NeutralLength
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

	public float InitialLength
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
	public float InitialSpeed
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
	public SpringModel(float mass = 1.0f, float stiffness = 0.0f, float neutralLength = 0.0f, 
		float initialLength = 0.0f, float initialSpeed = 0.0f) : base(5, 3, Allocator.Persistent,
			GetInitialVariable, ComputeDerivative, ComputeAnalyticalSolution)
	{
		Mass = mass;
		Stiffness = stiffness;
		NeutralLength = neutralLength;
		InitialLength = initialLength;
		InitialSpeed = initialSpeed;
	}
	#endregion Constructors

	#region Methods
	protected override void InitAnalysis()
	{
		float mass = _model.Data[0];
		float stiffness = _model.Data[1];
		float neutralLength = _model.Data[2];
		float initialLength = _model.Data[3];
		float initialSpeed = _model.Data[4];

		Initialize(mass, stiffness, neutralLength, initialLength, initialSpeed, 
			out float naturalFrequency, out float phase, out float amplitude);

		_model.SetTemporaryDataValue(0, naturalFrequency);
		_model.SetTemporaryDataValue(1, phase);
		_model.SetTemporaryDataValue(2, amplitude);
	}

	protected override void GenerateDefaultDescriptions(out string shortDescription, out string longDescription)
	{
		shortDescription = "Classic spring model";
		longDescription =
			"Parameter :\n" +
			"-> Elapsed Time [s]\n\n" +
			"Parameter Step :\n" +
			"-> Time Step [s] \n\n" +
			"Mean Absolute Errors :\n" +
			"-> Spring length [m]\n" +
			"-> Spring speed [m/s]";
	}
	#endregion Methods

	#region Static Methods
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Initialize(float mass, float stiffness, float neutralLength, float initialLength, 
		float initialSpeed, out float naturalFrequency, out float phase, out float amplitude)
	{
		float displacement = initialLength - neutralLength;
		
		naturalFrequency = math.sqrt(stiffness / mass);
		phase = math.atan(-initialSpeed / (displacement * naturalFrequency));
		amplitude = displacement / math.cos(phase);
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(Float2InitialVariableFunction))]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GetInitialVariable(float* modelData, float* modelTemporaryDataPtr, float2* initialVariable)
	{
		float initialLength = modelData[3];
		float initialSpeed = modelData[4];
		*initialVariable = new float2(initialLength, initialSpeed);
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(Float2InitialVariableFunction))]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ComputeDerivative(float* modelData, float* modelTemporaryDataPtr, float2* currentVariable, float currentParameter, float2* currentDerivative)
	{
		float mass = modelData[0];
		float neutralLength = modelData[2];
		float stiffness = modelData[1];
		float acceleration = stiffness * (neutralLength - (*currentVariable).x) / mass;
		float speed = (*currentVariable).y;
		*currentDerivative = new float2(speed, acceleration);
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(Float2InitialVariableFunction))]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ComputeAnalyticalSolution(float* modelData, float* modelTemporaryDataPtr, float currentParameter, float2* currentVariable)
	{
		float naturalFrequency = modelTemporaryDataPtr[0];
		float phase = modelTemporaryDataPtr[1];
		float amplitude = modelTemporaryDataPtr[2];
		float neutralLength = modelData[2];
		float angle = naturalFrequency * currentParameter + phase;
		float currentLength = neutralLength + amplitude * math.cos(angle);
		float currentSpeed = -amplitude * naturalFrequency * math.sin(angle);
		*currentVariable = new float2(currentLength, currentSpeed);
	}
	#endregion Static Methods
}
