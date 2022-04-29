using AOT;
using dExplorer.Editor.Mathematics;
using dExplorer.Runtime.Mathematics;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

[BurstCompile]
public unsafe class SimpleViscousFrictionModel : AnalysableDEModel
{
	#region Properties
	public float InertiaMoment
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

	public float Radius
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

	public float ViscousFrictionCoefficient
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

	public float InitialAngularSpeed
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
	public SimpleViscousFrictionModel(float inertiaMoment = 1.0f, float radius = 1.0f, 
		float viscousFrictionCoefficient = 0.0f, float initialAngularSpeed = 0.0f) :
		base(4, 1, Allocator.Persistent, GetInitialVariable, PreSimulate, PostSimulate, ComputeDerivative,
		ComputeAnalyticalSolution, DimensionalizeVariable, NondimensionalizeParameter, DimensionalizeParameter)
	{
		InertiaMoment = inertiaMoment;
		Radius = radius;
		ViscousFrictionCoefficient = viscousFrictionCoefficient;
		InitialAngularSpeed = initialAngularSpeed;
	}
	#endregion Constructors

	#region Methods
	protected override void InitAnalysis()
	{
		float coefficientA = ViscousFrictionCoefficient * Radius / InertiaMoment;
		_model.SetTemporaryDataValue(0, coefficientA);
	}

	protected override void GenerateDefaultDescriptions(out string shortDescription, out string longDescription)
	{
		shortDescription = "Classic viscous friction model";
		longDescription =
			"Parameter :\n" +
			"-> Elapsed Time [s]\n\n" +
			"Parameter Step :\n" +
			"-> Time Step [s]\n\n" +
			"Variable :\n" +
			"-> Object angular position [rad]" +
			"-> Object angular speed [rad/s]";
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
		*exportedNextVariable = *nextVariable;
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(Float2InitialVariableFunction))]
	public static void GetInitialVariable(float* modelData, float* modelTemporaryData, float2* initialVariable)
	{
		*initialVariable = new float2(0.0f, 1.0f);
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(Float2DerivativeFunction))]
	public static void ComputeDerivative(float* modelData, float* modelTemporaryData, float2* currentVariable, float currentParameter, float2* currentDerivative)
	{
		float angularSpeedRatio = (*currentVariable).y;
		*currentDerivative = new float2(angularSpeedRatio, -angularSpeedRatio);
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(Float2AnalyticalSolutionFunction))]
	public static void ComputeAnalyticalSolution(float* modelData, float* modelTemporaryData, float currentParameter, float2* currentVariable)
	{
		*currentVariable = new float2(
			1.0f - math.exp(-currentParameter),
			math.exp(-currentParameter));
	}

	[MonoPInvokeCallback(typeof(ParameterNondimensionalizationFunction))]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float NondimensionalizeParameter(in DEModel model, float parameter)
	{
		float coefficientA = model.TemporaryData[0];
		return coefficientA * parameter;
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(ParameterDimensionalizationFunction))]
	public static float DimensionalizeParameter(float* modelData, float* modelTemporaryData, float nonDimensionalizedParameter)
	{
		float coefficientA = modelTemporaryData[0];
		return nonDimensionalizedParameter / coefficientA;
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(Float2VariableDimensionalizationFunction))]
	public static void DimensionalizeVariable(float* modelData, float* modelTemporaryData, float2* nonDimensionalizedVariable, float2* dimensionalizedVariable)
	{
		float initialAngularSpeed = modelData[3];
		float coefficientA = modelTemporaryData[0];

		float angularPositionRatio = (*nonDimensionalizedVariable).x;
		float angularSpeedRatio = (*nonDimensionalizedVariable).y;

		*dimensionalizedVariable = new float2(
			angularPositionRatio * initialAngularSpeed / coefficientA,
			initialAngularSpeed * angularSpeedRatio);
	}
	#endregion Static Methods
}
