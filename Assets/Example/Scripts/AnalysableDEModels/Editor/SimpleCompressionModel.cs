using AOT;
using dExplorer.Editor.Mathematics;
using dExplorer.Runtime.Mathematics;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

[BurstCompile]
public unsafe class SimpleCompressionModel : AnalysableDEModel
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

	public float Surface
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

	public float HeatCapacityRatio
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

	public float IncompressibleLength
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

	public float MaxCompressibleLength
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

	public float InitialPressure
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

	public float InitialLength
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

	public float InitialSpeed
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
	public SimpleCompressionModel(float mass = 1.0f, float surface = 0.0f, float heatCapacityRatio = 0.0f, 
		float incompressibleLength = 0.0f, float maxCompressibleLength = 0.0f, float initialPressure = 0.0f, 
		float initialLength = 0.0f, float initialSpeed = 0.0f) :
		base(8, 1, Allocator.Persistent, GetInitialVariable, PreSimulate, PostSimulate, ComputeDerivative,
		ComputeAnalyticalSolution, DimensionalizeVariable, NondimensionalizeParameter, DimensionalizeParameter)
	{
		Mass = mass;
		Surface = surface;
		HeatCapacityRatio = heatCapacityRatio;
		IncompressibleLength = incompressibleLength;
		MaxCompressibleLength = maxCompressibleLength;
		InitialPressure = initialPressure;
		InitialLength = initialLength;
		InitialSpeed = initialSpeed;
	}
	#endregion Constructors

	#region Methods
	protected override void InitAnalysis()
	{
		float coefficientA = (Surface * InitialPressure) / (Mass * (InitialLength - IncompressibleLength));
		_model.SetTemporaryDataValue(0, coefficientA);
	}

	protected override void GenerateDefaultDescriptions(out string shortDescription, out string longDescription)
	{
		shortDescription = "Classic compression model";
		longDescription =
			"Parameter :\n" +
			"-> Elapsed Time [s]\n\n" +
			"Parameter Step :\n" +
			"-> Time Step [s]\n\n" +
			"Mean Absolute Errors :\n" +
			"-> Piston speed [m/s]" +
			"-> Piston position [m]";
	}
	#endregion Methods

	#region Static Methods
	[BurstCompile]
	[MonoPInvokeCallback(typeof(Float2PreSimulationFunction))]
	public static void PreSimulate(float* modelData, float* modelTemporaryData, float2* currentVariable, float* currentParameter) { }

	[BurstCompile]
	[MonoPInvokeCallback(typeof(Float2PostSimulationFunction))]
	public static void PostSimulate(float* modelData, float* modelTemporaryData, float2* nextVariable) 
	{
		float maxCompressibleLength = modelData[3];
		float nextSpeed = (*nextVariable).x;
		float nextPosition = (*nextVariable).y;

		if (nextPosition < 0.0f)
		{
			nextPosition = 0.0f;
			nextSpeed = 0.0f;
		}
		else if (nextPosition > maxCompressibleLength)
		{
			nextPosition = maxCompressibleLength;
			nextSpeed = 0.0f;
		}
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(Float2InitialVariableFunction))]
	public static void GetInitialVariable(float* modelData, float* modelTemporaryData, float2* initialVariable)
	{
		float initialLength = modelData[6];
		float maxCompressibleLength = modelData[3];
		*initialVariable = new float2(0.0f, initialLength / maxCompressibleLength);
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(Float2DerivativeFunction))]
	public static void ComputeDerivative(float* modelData, float* modelTemporaryData, float2* currentVariable, float currentParameter, float2* currentDerivative)
	{
		float coefficientA = modelTemporaryData[0];
		float heatCapacityRatio = modelData[2];
		float incompressibleLength = modelData[3];
		float initialLength = modelData[6];

		float currentSpeed = (*currentVariable).x;
		float currentLength = (*currentVariable).y;

		*currentDerivative = new float2(
			coefficientA * math.pow(initialLength - incompressibleLength, heatCapacityRatio + 1.0f) / math.pow(currentLength - incompressibleLength, heatCapacityRatio),
			currentSpeed);
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(Float2AnalyticalSolutionFunction))]
	public static void ComputeAnalyticalSolution(float* modelData, float* modelTemporaryData, float currentParameter, float2* currentVariable)
	{
		*currentVariable = 0.0f;
	}

	[MonoPInvokeCallback(typeof(ParameterNondimensionalizationFunction))]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float NondimensionalizeParameter(in DEModel model, float parameter)
	{
		float coefficientA = model.TemporaryData[0];
		return math.sqrt(coefficientA) * parameter;
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(ParameterDimensionalizationFunction))]
	public static float DimensionalizeParameter(float* modelData, float* modelTemporaryData, float nonDimensionalizedParameter)
	{
		float coefficientA = modelTemporaryData[0];
		return nonDimensionalizedParameter / math.sqrt(coefficientA);
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(Float2VariableDimensionalizationFunction))]
	public static void DimensionalizeVariable(float* modelData, float* modelTemporaryData, float2* nonDimensionalizedVariable, float2* dimensionalizedVariable)
	{
		float maxCompressibleLength = modelData[4];
		float nonDimensionalizedSpeed = (*nonDimensionalizedVariable).x;
		float nonDimensionalizedPosition = (*nonDimensionalizedVariable).y;
		*dimensionalizedVariable = new float2(nonDimensionalizedSpeed, nonDimensionalizedPosition * maxCompressibleLength);
	}
	#endregion Static Methods
}
