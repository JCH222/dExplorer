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
	public DragModel(float mass = 1.0f, float fluidDensity = 0.0f, float referenceSurface = 0.0f,
		float dragCoefficient = 0.0f, float initialSpeed = 0.0f, float additionalForce = 0.0f) : 
		base(6, 4, Allocator.Persistent, GetInitialVariable, PreSimulate, PostSimulate, ComputeDerivative, 
			ComputeAnalyticalSolution, DimensionalizeVariable, NondimensionalizeParameter, DimensionalizeParameter)
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
	protected override void InitAnalysis() 
	{
		float mass = _model.Data[0];
		float fluidDensity = _model.Data[1];
		float referenceSurface = _model.Data[2];
		float dragCoefficient = _model.Data[3];
		float initialSpeed = _model.Data[4];
		float additionalForce = _model.Data[5];

		Initialize(mass, fluidDensity, referenceSurface, dragCoefficient, initialSpeed, additionalForce, 
			out float coefficientA, out float coefficientB, out float initialVariable);

		_model.SetTemporaryDataValue(0, coefficientA);
		_model.SetTemporaryDataValue(1, coefficientB);
		_model.SetTemporaryDataValue(2, initialVariable);
		_model.SetTemporaryDataValue(3, 1.0f);
	}

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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void Initialize(float mass, float fluidDensity, float referenceSurface,
		float dragCoefficient, float initialSpeed, float additionalForce, 
		out float coefficientA, out float coefficientB, out float initialVariable)
	{
		coefficientA = 0.5f * fluidDensity * referenceSurface * dragCoefficient;
		coefficientB = additionalForce / mass;

		if (coefficientB > math.EPSILON || coefficientB < -math.EPSILON)
		{
			float absLimitSpeed = math.sqrt(math.abs(additionalForce) / coefficientA);
			initialVariable = initialSpeed / absLimitSpeed;
		}
		else
		{
			initialVariable = initialSpeed;
		}
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(FloatPreSimulationFunction))]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void PreSimulate(float* modelData, float* modelTemporaryData, float* currentVariable, float* currentParameter)
	{
		float coefficientB = modelTemporaryData[1];
		float realSpeedSign = modelTemporaryData[3];

		if (*currentVariable < -math.EPSILON)
		{
			*currentVariable = -*currentVariable;
			modelTemporaryData[1] = -coefficientB;
			modelTemporaryData[2] = *currentVariable;
			modelTemporaryData[3] = -realSpeedSign;
			*currentParameter = 0.0f;
		}
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(FloatPostSimulationFunction))]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void PostSimulate(float* modelData, float* modelTemporaryData, float* nextVariable)
	{
		*nextVariable = math.sign(modelTemporaryData[3]) * *nextVariable;
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(FloatInitialVariableFunction))]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GetInitialVariable(float* modelData, float* modelTemporaryData, float* initialVariable)
	{
		*initialVariable = modelTemporaryData[2];
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(FloatInitialVariableFunction))]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ComputeDerivative(float* modelData, float* modelTemporaryData, float* currentVariable, float currentParameter, float* currentDerivative)
	{
		float coefficientB = modelTemporaryData[1];

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
	public static void ComputeAnalyticalSolution(float* modelData, float* modelTemporaryData, float currentParameter, float *currentVariable)
	{
		float coefficientA = modelTemporaryData[0];
		float coefficientB = modelTemporaryData[1];

		if (coefficientB > math.EPSILON)
		{
			float initialSpeedRatio = modelTemporaryData[2];
			float constant = (initialSpeedRatio - 1.0f) / (initialSpeedRatio + 1.0f);
			float coefficient = constant * math.exp(-2.0f * currentParameter);

			*currentVariable = (1.0f + coefficient) / (1.0f - coefficient);
		}
		else if (coefficientB < -math.EPSILON)
		{
			float initialSpeedRatio = modelTemporaryData[2];
			float tanParameter = math.tan(-currentParameter);
			float tanConstant = -initialSpeedRatio;

			*currentVariable = (tanParameter - tanConstant) / (1.0f + tanParameter * tanConstant);
		}
		else
		{
			float initialSpeed = modelTemporaryData[2];
			float constant = 1.0f / initialSpeed;

			*currentVariable = 1.0f / (currentParameter + constant);
		}
	}

	[MonoPInvokeCallback(typeof(ParameterNondimensionalizationFunction))]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float NondimensionalizeParameter(in DEModel model, float parameter)
	{
		float mass = model.Data[0];

		float invMass = 1.0f / mass;
		float coefficientA = model.TemporaryData[0];
		float coefficientB = model.TemporaryData[1];

		if (coefficientB > math.EPSILON || coefficientB < -math.EPSILON)
		{
			float additionalForce = math.abs(model.Data[5]);
			return math.sqrt(additionalForce * coefficientA) * parameter * invMass;
		}
		else
		{
			return parameter * coefficientA * invMass;
		}
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(ParameterDimensionalizationFunction))]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float DimensionalizeParameter(float* modelData, float* modelTemporaryData, float nonDimensionalizedParameter)
	{
		float mass = modelData[0];

		float coefficientA = modelTemporaryData[0];
		float coefficientB = modelTemporaryData[1];

		if (coefficientB > math.EPSILON || coefficientB < -math.EPSILON)
		{
			float additionalForce = math.abs(modelData[5]);
			return nonDimensionalizedParameter * mass / math.sqrt(additionalForce * coefficientA);
		}
		else
		{
			return nonDimensionalizedParameter * mass / coefficientA;
		}
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(FloatVariableDimensionalizationFunction))]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void DimensionalizeVariable(float* modelData, float* modelTemporaryData, float* nonDimensionalizedVariable, float* dimensionalizedVariable)
	{
		float coefficientA = modelTemporaryData[0];
		float coefficientB = modelTemporaryData[1];

		if (coefficientB > math.EPSILON || coefficientB < -math.EPSILON)
		{
			float additionalForce = math.abs(modelData[5]);
			float absLimitSpeed = math.sqrt(additionalForce / coefficientA);
			float speedRatio = *nonDimensionalizedVariable;

			*dimensionalizedVariable = speedRatio * absLimitSpeed;
		}
		else
		{
			*dimensionalizedVariable = *nonDimensionalizedVariable;
		}
	}
	#endregion Static Methods
}
