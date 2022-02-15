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
	#endregion Properties

	#region Constructors
	public SpringModel(float mass = 1.0f, float stiffness = 0.0f, float neutralLength = 0.0f, 
		float initialLength = 0.0f, float initialSpeed = 0.0f) : base(8, Allocator.Persistent) 
	{
		Mass = mass;
		Stiffness = stiffness;
		NeutralLength = neutralLength;
		InitialLength = initialLength;
		InitialSpeed = initialSpeed;
	}
	#endregion Constructors

	#region Methods
	protected override void Init()
	{
		float displacement = InitialLength - NeutralLength;
		float naturalFrequency = math.sqrt(Stiffness / Mass);
		float phase = math.atan(-InitialSpeed / (displacement * naturalFrequency));
		float amplitude = displacement / math.cos(phase);

		_model.SetDataValue(5, naturalFrequency);
		_model.SetDataValue(6, phase);
		_model.SetDataValue(7, amplitude);

		ActivateFloat2Dimension(GetInitialVariable, ComputeDerivative, ComputeAnalyticalSolution);
	}
	#endregion Methods


	#region Static Methods
	[BurstCompile]
	[MonoPInvokeCallback(typeof(Float2InitialVariableFunction))]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void GetInitialVariable(float* data, float2* initialVariable)
	{
		float initialLength = data[3];
		float initialSpeed = data[4];
		*initialVariable = new float2(initialLength, initialSpeed);
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(Float2InitialVariableFunction))]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ComputeDerivative(float* data, float2* currentVariable, float currentParameter, float2* currentDerivative)
	{
		float mass = data[0];
		float neutralLength = data[2];
		float stiffness = data[1];
		float acceleration = stiffness * (neutralLength - (*currentVariable).x) / mass;
		float speed = (*currentVariable).y;
		*currentDerivative = new float2(speed, acceleration);
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(Float2InitialVariableFunction))]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ComputeAnalyticalSolution(float* data, float currentParameter, float2* currentVariable)
	{
		float naturalFrequency = data[5];
		float phase = data[6];
		float amplitude = data[7];
		float neutralLength = data[2];
		float angle = naturalFrequency * currentParameter + phase;
		float currentLength = neutralLength + amplitude * math.cos(angle);
		float currentSpeed = -amplitude * naturalFrequency * math.sin(angle);
		*currentVariable = new float2(currentLength, currentSpeed);
	}
	#endregion Static Methods
}
