using AOT;
using dExplorer.Editor.Mathematics;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

[BurstCompile]
public unsafe class SimpleCrankshaftCompressionModel : AnalysableDEModel
{
	#region Properties
	public float Surface
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

	public float HeatCapacityRatio
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

	public float IncompressibleLength
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

	public float OuterPressure
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

	public float InitialPressure
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

	public float InertiaMoment
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

	public float CrankRadius
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

	public float ConnectingRodLength
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

	public float InitialAngularPosition
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

	public float InitialAngularSpeed
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
	public SimpleCrankshaftCompressionModel(float surface = 0.0f, float heatCapacityRatio = 0.0f,
		float incompressibleLength = 0.0f, float outerPressure = 0.0f, float initialPressure = 0.0f, 
		float inertiaMoment = 0.0f, float crankRadius = 0.0f, float connectingRodLength = 0.0f, 
		float initialAngularPosition = 0.0f, float initialAngularSpeed = 0.0f) :
		base(11, 1, Allocator.Persistent, GetInitialVariable, PreSimulate, PostSimulate, ComputeDerivative, ComputeAnalyticalSolution)
	{
		Surface = surface;
		HeatCapacityRatio = heatCapacityRatio;
		IncompressibleLength = incompressibleLength;
		OuterPressure = outerPressure;
		InitialPressure = initialPressure;
		InertiaMoment = inertiaMoment;
		CrankRadius = crankRadius;
		ConnectingRodLength = connectingRodLength;
		InitialAngularPosition = initialAngularPosition;
		InitialAngularSpeed = initialAngularSpeed;
	}
	#endregion Constructors

	#region Methods
	protected override void InitAnalysis()
	{
		float initialPistonLength = ComputePistonLength(InitialAngularPosition, CrankRadius, ConnectingRodLength);
		_model.SetTemporaryDataValue(0, initialPistonLength);
	}

	protected override void GenerateDefaultDescriptions(out string shortDescription, out string longDescription)
	{
		shortDescription = "Crankshaft compression model";
		longDescription =
			"Parameter :\n" +
			"-> Elapsed Time [s]\n\n" +
			"Parameter Step :\n" +
			"-> Time Step [s]\n\n" +
			"Variable :\n" +
			"-> Axle angular speed [rad/s]\n" +
			"-> Axle angular position [rad]\n\n" +
			"NB : There is no simple analytical solution";
	}
	#endregion Methods

	#region Static Methods
	[BurstCompile]
	public static float ComputePistonLength(float angularPosition, float crankRadius, float connectingRodLength) 
	{
		return crankRadius * (1.0f - math.cos(angularPosition)) + connectingRodLength - 
			math.sqrt(connectingRodLength * connectingRodLength - math.pow(crankRadius * math.sin(angularPosition), 2));
	}

	[BurstCompile]
	public static float ComputePistonForce(float length, float surface, float initialPressure, float initialLength, float incompressibleLength, float heatCapacityRatio, float outerPressure)
	{
		float coefficient = surface * initialPressure * math.pow(initialLength - incompressibleLength, heatCapacityRatio);
		return coefficient / math.pow(length - incompressibleLength, heatCapacityRatio) - surface * outerPressure;
	}

	[BurstCompile]
	public static float ComputeTorque(float pistonForce, float angularPosition, float crankRadius, float connectingRodLength)
	{
		float beta = math.asin(crankRadius * math.sin(angularPosition) / connectingRodLength);
		return crankRadius * pistonForce * math.sin(angularPosition + beta) / math.cos(beta);
	}


	[BurstCompile]
	[MonoPInvokeCallback(typeof(Float2PreSimulationFunction))]
	public static void PreSimulate(float* modelData, float* modelTemporaryData, float2* currentVariable, float* currentParameter) { }

	[BurstCompile]
	[MonoPInvokeCallback(typeof(Float2PostSimulationFunction))]
	public static void PostSimulate(float* modelData, float* modelTemporaryData, float2* nextVariable, float2* exportedNextVariable)
	{
		*exportedNextVariable = *nextVariable;
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(Float2InitialVariableFunction))]
	public static void GetInitialVariable(float* modelData, float* modelTemporaryData, float2* initialVariable)
	{
		float initialAngularPosition = modelData[8];
		float initialAngularSpeed = modelData[9];
		*initialVariable = new float2(initialAngularSpeed, initialAngularPosition);
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(Float2DerivativeFunction))]
	public static void ComputeDerivative(float* modelData, float* modelTemporaryData, float2* currentVariable, float currentParameter, float2* currentDerivative)
	{
		float initialPistonLength = modelTemporaryData[0];
		float surface = modelData[0];
		float heatCapacityRatio = modelData[1];
		float incompressibleLength = modelData[2];
		float outerPressure = modelData[3];
		float initialPressure = modelData[4];
		float inertiaMoment = modelData[5];
		float crankRadius = modelData[6];
		float connectingRodLength = modelData[7];

		float currentAngularSpeed = (*currentVariable).x;
		float currentAngularPosition = (*currentVariable).y;

		float currentPistonLength = ComputePistonLength(currentAngularPosition, crankRadius, connectingRodLength);
		float currentPistonForce = ComputePistonForce(currentPistonLength, surface, initialPressure, initialPistonLength, incompressibleLength, heatCapacityRatio, outerPressure);
		float currentTorque = ComputeTorque(currentPistonForce, currentAngularPosition, crankRadius, connectingRodLength) / inertiaMoment;

		*currentDerivative = new float2(
			currentTorque,
			currentAngularSpeed);
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
