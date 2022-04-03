using AOT;
using dExplorer.Editor.Mathematics;
using dExplorer.Runtime.Mathematics;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

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

	public float AdditionalForce
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
	#endregion Properties

	#region Constructors
	public DragModel(float mass = 1.0f, float fluidDensity = 0.0f, float referenceSurface = 0.0f,
		float dragCoefficient = 0.0f, float initialSpeed = 0.0f, float additionalForce = 0.0f) : 
		base(6, 0, Allocator.Persistent, GetInitialVariable, ComputeDerivative, ComputeAnalyticalSolution, 
			DimensionalizeVariable, NondimensionalizeParameter, DimensionalizeParameter)
	{
		Mass = mass;
		FluidDensity = fluidDensity;
		ReferenceSurface = referenceSurface;
		DragCoefficient = dragCoefficient;
		InitialSpeed = initialSpeed;
		AdditionalForce = additionalForce;
	}
	#endregion Constructors

	#region Methods
	protected override void InitAnalysis() { }

	protected override void GenerateDefaultDescriptions(out string shortDescription, out string longDescription)
	{
		shortDescription = "Classic drag model";
		longDescription =
			"Parameter :\n" +
			"-> Elapsed Time [s]\n\n" +
			"Parameter Step :\n" +
			"-> Time Step [s]\n\n" +
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
		float mass = data[0];
		float fluidDensity = data[1];
		float referenceSurface = data[2];
		float dragCoefficient = data[3];
		float additionalForce = data[5];

		float coefficientA = 0.5f * fluidDensity * referenceSurface * dragCoefficient;
		float coefficientB = additionalForce / mass;

		if (coefficientB > math.EPSILON)
		{
			float limitSpeed = math.sqrt(additionalForce / coefficientA);

			*initialVariable = data[4] / limitSpeed;
		}
		else if (coefficientB < -math.EPSILON)
		{
			float limitSpeed = math.sqrt(-additionalForce / coefficientA);

			*initialVariable = data[4] / limitSpeed;
		}
		else
		{
			*initialVariable = data[4];
		}
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(FloatInitialVariableFunction))]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ComputeDerivative(float* data, float* currentVariable, float currentParameter, float* currentDerivative)
	{
		float mass = data[0];
		float additionalForce = data[5];

		float coefficientB = additionalForce / mass;

		if (coefficientB > math.EPSILON)
		{
			float speedRatio = *currentVariable;

			*currentDerivative = -speedRatio * speedRatio + 1.0f;
		}
		else if (coefficientB < -math.EPSILON)
		{
			float speedRatio = *currentVariable;

			*currentDerivative = -(speedRatio * speedRatio + 1.0f);
		}
		else
		{
			float speed = *currentVariable;

			*currentDerivative = -speed * speed;
		}
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(FloatInitialVariableFunction))]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ComputeAnalyticalSolution(float* data, float currentParameter, float *currentVariable)
	{
		float mass = data[0];
		float fluidDensity = data[1];
		float referenceSurface = data[2];
		float dragCoefficient = data[3];
		float additionalForce = data[5];

		float coefficientA = 0.5f * fluidDensity * referenceSurface * dragCoefficient;
		float coefficientB = additionalForce / mass;

		if (coefficientB > math.EPSILON)
		{
			float defaultValue = float.NaN;
			float* initialSpeedRatio = &defaultValue;
			GetInitialVariable(data, initialSpeedRatio);

			float constant = (*initialSpeedRatio - 1.0f) / (*initialSpeedRatio + 1.0f);
			float coefficient = constant * math.exp(-2.0f * currentParameter);

			*currentVariable = (1.0f + coefficient) / (1.0f - coefficient);
		}
		else if (coefficientB < -math.EPSILON)
		{
			float defaultValue = float.NaN;
			float* initialSpeedRatio = &defaultValue;
			GetInitialVariable(data, initialSpeedRatio);

			float tanParameter = math.tan(-currentParameter);
			float tanConstant = -*initialSpeedRatio;

			*currentVariable = (tanParameter - tanConstant) / (1.0f + tanParameter * tanConstant);
		}
		else
		{
			float defaultValue = float.NaN;
			float* initialSpeed = &defaultValue;
			GetInitialVariable(data, initialSpeed);

			float constant = 1.0f / *initialSpeed;

			*currentVariable = 1.0f / (currentParameter + constant);
		}
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(ParameterNondimensionalizationFunction))]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float NondimensionalizeParameter(in DEModel model, float parameter)
	{
		float mass = model.Data[0];
		float fluidDensity = model.Data[1];
		float referenceSurface = model.Data[2];
		float dragCoefficient = model.Data[3];
		float additionalForce = model.Data[5];

		float invMass = 1.0f / mass;
		float coefficientA = 0.5f * fluidDensity * referenceSurface * dragCoefficient;
		float coefficientB = additionalForce * invMass;

		if (coefficientB > math.EPSILON)
		{
			return math.sqrt(additionalForce * coefficientA) * parameter * invMass;
		}
		else if (coefficientB < -math.EPSILON)
		{
			return math.sqrt(-additionalForce * coefficientA) * parameter * invMass;
		}
		else
		{
			return parameter * coefficientA * invMass;
		}
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(ParameterDimensionalizationFunction))]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float DimensionalizeParameter(float* modelData, float nonDimensionalizedParameter)
	{
		float mass = modelData[0];
		float fluidDensity = modelData[1];
		float referenceSurface = modelData[2];
		float dragCoefficient = modelData[3];
		float additionalForce = modelData[5];

		float coefficientA = 0.5f * fluidDensity * referenceSurface * dragCoefficient;
		float coefficientB = additionalForce / mass;

		if (coefficientB > math.EPSILON)
		{
			return nonDimensionalizedParameter * mass / math.sqrt(additionalForce * coefficientA);
		}
		else if (coefficientB < -math.EPSILON)
		{
			return nonDimensionalizedParameter * mass / math.sqrt(-additionalForce * coefficientA);
		}
		else
		{
			return nonDimensionalizedParameter * mass / coefficientA;
		}
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(FloatVariableDimensionalizationFunction))]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void DimensionalizeVariable(float* modelData, float* nonDimensionalizedVariable, float* dimensionalizedVariable)
	{
		float mass = modelData[0];
		float fluidDensity = modelData[1];
		float referenceSurface = modelData[2];
		float dragCoefficient = modelData[3];
		float additionalForce = modelData[5];

		float coefficientA = 0.5f * fluidDensity * referenceSurface * dragCoefficient;
		float coefficientB = additionalForce / mass;

		if (coefficientB > math.EPSILON)
		{
			float limitSpeed = math.sqrt(additionalForce / coefficientA);
			float speedRatio = *nonDimensionalizedVariable;

			*dimensionalizedVariable = speedRatio * limitSpeed;
		}
		else if (coefficientB < -math.EPSILON)
		{
			float limitSpeed = math.sqrt(-additionalForce / coefficientA);
			float speedRatio = *nonDimensionalizedVariable;

			*dimensionalizedVariable = speedRatio * limitSpeed;
		}
		else
		{
			*dimensionalizedVariable = *nonDimensionalizedVariable;
		}
	}
	#endregion Static Methods
}
