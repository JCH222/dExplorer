using AOT;
using dExplorer.Editor.Mathematics;
using dExplorer.Runtime.Mathematics;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

[BurstCompile]
public unsafe class SimpleDragModel : AnalysableDEModel
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
	public SimpleDragModel(float mass = 1.0f, float fluidDensity = 0.0f, 
		float referenceSurface = 0.0f, float dragCoefficient = 0.0f, float initialSpeed = 0.0f) :
		base(5, 1, Allocator.Persistent, GetInitialVariable, PreSimulate, PostSimulate, ComputeDerivative, 
		ComputeAnalyticalSolution, DimensionalizeVariable, NondimensionalizeParameter, DimensionalizeParameter)
	{
		Mass = mass;
		FluidDensity = fluidDensity;
		ReferenceSurface = referenceSurface;
		DragCoefficient = dragCoefficient;
		InitialSpeed = initialSpeed;
	}
	#endregion Constructors

	#region Methods
	protected override void InitAnalysis()
	{
		float coefficientA = 0.5f * FluidDensity * ReferenceSurface * DragCoefficient / Mass;
		_model.SetTemporaryDataValue(0, coefficientA);
	}

	protected override void GenerateDefaultDescriptions(out string shortDescription, out string longDescription)
	{
		shortDescription = "Classic drag model";
		longDescription =
			"Parameter :\n" +
			"-> Elapsed Time [s]\n\n" +
			"Parameter Step :\n" +
			"-> Time Step [s]\n\n" +
			"Variable :\n" +
			"-> Object speed [m/s]";
	}
	#endregion Methods

	#region Static Methods
	[BurstCompile]
	[MonoPInvokeCallback(typeof(FloatPreSimulationFunction))]
	public static void PreSimulate(float* modelData, float* modelTemporaryData, float* currentVariable, float* currentParameter) { }

	[BurstCompile]
	[MonoPInvokeCallback(typeof(FloatPostSimulationFunction))]
	public static void PostSimulate(float* modelData, float* modelTemporaryData, float* nextVariable, float* exportedNextVariable) 
	{
		*exportedNextVariable = *nextVariable;
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(FloatInitialVariableFunction))]
	public static void GetInitialVariable(float* modelData, float* modelTemporaryData, float* initialVariable)
	{
		*initialVariable = 1.0f;
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(FloatDerivativeFunction))]
	public static void ComputeDerivative(float* modelData, float* modelTemporaryData, float* currentVariable, float currentParameter, float* currentDerivative)
	{
		float speedRatio = *currentVariable;
		*currentDerivative = -speedRatio * speedRatio;
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(FloatAnalyticalSolutionFunction))]
	public static void ComputeAnalyticalSolution(float* modelData, float* modelTemporaryData, float currentParameter, float* currentVariable)
	{
		*currentVariable = 1.0f / (currentParameter + 1.0f);
	}

	[MonoPInvokeCallback(typeof(ParameterNondimensionalizationFunction))]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float NondimensionalizeParameter(in DEModel model, float parameter)
	{
		float coefficientA = model.TemporaryData[0];
		float initialSpeed = model.Data[4];
		return initialSpeed * coefficientA * parameter;
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(ParameterDimensionalizationFunction))]
	public static float DimensionalizeParameter(float* modelData, float* modelTemporaryData, float nonDimensionalizedParameter)
	{
		float initialSpeed = modelData[4];
		float coefficientA = modelTemporaryData[0];
		return nonDimensionalizedParameter / (initialSpeed * coefficientA);
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(FloatVariableDimensionalizationFunction))]
	public static void DimensionalizeVariable(float* modelData, float* modelTemporaryData, float* nonDimensionalizedVariable, float* dimensionalizedVariable)
	{
		float initialSpeed = modelData[4];
		float speedRatio = *nonDimensionalizedVariable;
		*dimensionalizedVariable = speedRatio  * initialSpeed;
	}
	#endregion Static Methods
}
