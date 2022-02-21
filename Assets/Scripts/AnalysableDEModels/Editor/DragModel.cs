using AOT;
using dExplorer.Editor.Mathematics;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;

[BurstCompile]
public unsafe class DragModel : AnalysableDEModel
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

	public float FluidDensity
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

	public float ReferenceSurface
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

	public float DragCoefficient
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
	public DragModel(float mass = 1.0f, float fluidDensity = 0.0f, float referenceSurface = 0.0f,
		float dragCoefficient = 0.0f, float initialSpeed = 0.0f) : base(5, Allocator.Persistent)
	{
		Mass = mass;
		FluidDensity = fluidDensity;
		ReferenceSurface = referenceSurface;
		DragCoefficient = dragCoefficient;
		InitialSpeed = initialSpeed;
	}
	#endregion Constructors

	#region Methods
	protected override void Init() 
	{
		ActivateFloat1Dimension(GetInitialVariable, ComputeDerivative, ComputeAnalyticalSolution);
	}

	protected override void GenerateDefaultDescriptions(out string shortDescription, out string longDescription)
	{
		shortDescription = "Classic drag model";
		longDescription =
			"Mean Absolute Errors :\n" +
			"-> Object speed [m/s]";
	}
	#endregion Methods

	#region Static Methods
	[BurstCompile]
	[MonoPInvokeCallback(typeof(FloatInitialVariableFunction))]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void GetInitialVariable(float* data, float* initialVariable)
	{
		*initialVariable = data[4];
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(FloatInitialVariableFunction))]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ComputeDerivative(float* data, float* currentVariable, float currentParameter, float* currentDerivative)
	{
		float mass = data[0];
		float fluidDensity = data[1];
		float referenceSurface = data[2];
		float dragCoefficient = data[3];
		float speed = *currentVariable;
		*currentDerivative = -0.5f * fluidDensity * referenceSurface * dragCoefficient * speed * speed / mass;
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(FloatInitialVariableFunction))]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ComputeAnalyticalSolution(float* data, float currentParameter, float* currentVariable)
	{
		float mass = data[0];
		float fluidDensity = data[1];
		float referenceSurface = data[2];
		float dragCoefficient = data[3];
		float initialSpeed = data[4];

		float coef = -0.5f * fluidDensity * referenceSurface * dragCoefficient / mass;

		*currentVariable = 1.0f / (-coef * currentParameter + (1.0f / initialSpeed));
	}
	#endregion Static Methods
}
