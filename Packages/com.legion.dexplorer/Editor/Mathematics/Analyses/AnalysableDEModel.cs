namespace dExplorer.Editor.Mathematics
{
	using dExplorer.Runtime.Mathematics;
	using System;
	using System.Collections.Generic;
	using Unity.Burst;
	using Unity.Collections;
	using Unity.Mathematics;
	using UnityEditor;

	public unsafe delegate float ParameterNondimensionalizationFunction(in DEModel model, float parameter);
	public unsafe delegate float ParameterDimensionalizationFunction(float* modelData, float* modelTemporaryData, float nonDimensionalizedParameter);

	/// <summary>
	/// Differential equation model wrapper used for analyses.
	/// </summary>
	public abstract class AnalysableDEModel
	{
		#region Fields
		protected DEModel _model;
		protected float _minParameter;
		protected float _maxParameter;
		protected HashSet<float> _parameterSteps;
		protected HashSet<DESolvingType> _solvingTypes;

		private readonly Type _variableType;
		private readonly bool _isNondimensionalized;

		private readonly FunctionPointer<FloatInitialVariableFunction> _floatInitialVariableFunctionPointer;
		private readonly FunctionPointer<FloatDerivativeFunction> _floatDerivativeFunctionPointer;
		private readonly FunctionPointer<FloatAnalyticalSolutionFunction> _floatAnalyticalSolutionFunctionPointer;
		private readonly FunctionPointer<FloatVariableDimensionalizationFunction> _floatVariableDimensionalizationFunctionPointer;

		private readonly FunctionPointer<Float2InitialVariableFunction> _float2InitialVariableFunctionPointer;
		private readonly FunctionPointer<Float2DerivativeFunction> _float2DerivativeFunctionPointer;
		private readonly FunctionPointer<Float2AnalyticalSolutionFunction> _float2AnalyticalSolutionFunctionPointer;
		private readonly FunctionPointer<Float2VariableDimensionalizationFunction> _float2VariableDimensionalizationFunctionPointer;

		private readonly ParameterNondimensionalizationFunction _parameterNondimensionalizationFunction;
		private readonly ParameterDimensionalizationFunction _parameterDimensionalizationFunction;
		#endregion Fields

		#region Constructors
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parameterNondimensionalizationFunction">Parameter nondimensionalization function</param>
		/// <param name="parameterDimensionalizationFunction">Parameter nondimensionalization function</param>
		private AnalysableDEModel(
			ParameterNondimensionalizationFunction parameterNondimensionalizationFunction = null,
			ParameterDimensionalizationFunction parameterDimensionalizationFunction = null)
		{
			_parameterNondimensionalizationFunction = parameterNondimensionalizationFunction;
			_parameterDimensionalizationFunction = parameterDimensionalizationFunction;

			if (parameterNondimensionalizationFunction != null && parameterDimensionalizationFunction != null)
			{
				_isNondimensionalized = true;
			}
			else
			{
				_isNondimensionalized = false;
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="dataNb">Variable number</param>
		/// <param name="temporaryDataNb">Variable number</param>
		/// <param name="allocator">Allocation type</param>
		/// <param name="initialVariableFunction">Initial state function</param>
		/// <param name="derivativeFunction">Derivative computation function</param>
		/// <param name="analyticalSolutionFunction">Analytical solution computation function</param>
		/// <param name="variableDimensionalizationFunction">Variable dimensionalization function</param>
		/// <param name="parameterNondimensionalizationFunction">Parameter nondimensionalization function</param>
		/// <param name="parameterDimensionalizationFunction">Parameter dimensionalization function</param>
		public AnalysableDEModel(int dataNb, int temporaryDataNb, Allocator allocator, 
			FloatInitialVariableFunction initialVariableFunction,
			FloatDerivativeFunction derivativeFunction, FloatAnalyticalSolutionFunction analyticalSolutionFunction,
			FloatVariableDimensionalizationFunction variableDimensionalizationFunction = null,
			ParameterNondimensionalizationFunction parameterNondimensionalizationFunction = null,
			ParameterDimensionalizationFunction parameterDimensionalizationFunction = null) : 
			this(parameterNondimensionalizationFunction, parameterDimensionalizationFunction)
		{
			Init(dataNb, temporaryDataNb, allocator);
			_variableType = Type.GetType("System.Single");
			_floatInitialVariableFunctionPointer = BurstCompiler.CompileFunctionPointer<FloatInitialVariableFunction>(initialVariableFunction);
			_floatDerivativeFunctionPointer = BurstCompiler.CompileFunctionPointer<FloatDerivativeFunction>(derivativeFunction);
			_floatAnalyticalSolutionFunctionPointer = BurstCompiler.CompileFunctionPointer<FloatAnalyticalSolutionFunction>(analyticalSolutionFunction);

			if (_isNondimensionalized)
			{
				_floatVariableDimensionalizationFunctionPointer = BurstCompiler.CompileFunctionPointer<FloatVariableDimensionalizationFunction>(variableDimensionalizationFunction);
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="dataNb">Variable number</param>
		/// <param name="temporaryDataNb">Variable number</param>
		/// <param name="allocator">Allocation type</param>
		/// <param name="initialVariableFunction">Initial state function</param>
		/// <param name="derivativeFunction">Derivative computation function</param>
		/// <param name="analyticalSolutionFunction">Analytical solution computation function</param>
		/// <param name="parameterNondimensionalizationFunction">Parameter nondimensionalization function</param>
		/// <param name="parameterDimensionalizationFunction">Parameter dimensionalization function</param>
		public AnalysableDEModel(int dataNb, int temporaryDataNb, Allocator allocator,
			Float2InitialVariableFunction initialVariableFunction,
			Float2DerivativeFunction derivativeFunction, Float2AnalyticalSolutionFunction analyticalSolutionFunction,
			Float2VariableDimensionalizationFunction variableDimensionalizationFunction = null,
			ParameterNondimensionalizationFunction parameterNondimensionalizationFunction = null,
			ParameterDimensionalizationFunction parameterDimensionalizationFunction = null) :
			this(parameterNondimensionalizationFunction, parameterDimensionalizationFunction)
		{
			Init(dataNb, temporaryDataNb, allocator);
			_variableType = Type.GetType("Unity.Mathematics.float2");
			_float2InitialVariableFunctionPointer = BurstCompiler.CompileFunctionPointer<Float2InitialVariableFunction>(initialVariableFunction); ;
			_float2DerivativeFunctionPointer = BurstCompiler.CompileFunctionPointer<Float2DerivativeFunction>(derivativeFunction);
			_float2AnalyticalSolutionFunctionPointer = BurstCompiler.CompileFunctionPointer<Float2AnalyticalSolutionFunction>(analyticalSolutionFunction);

			if (_isNondimensionalized)
			{
				_float2VariableDimensionalizationFunctionPointer = BurstCompiler.CompileFunctionPointer<Float2VariableDimensionalizationFunction>(variableDimensionalizationFunction);
			}
		}
		#endregion Constructors

		#region Properties
		/// <summary>
		/// Min parameter of the simulations.
		/// </summary>
		public float MinParameter
		{
			get
			{
				return _minParameter;
			}
			set
			{
				_minParameter = math.min(value, _maxParameter);
			}
		}

		/// <summary>
		/// Max parameter of the simulations.
		/// </summary>
		public float MaxParameter
		{
			get
			{
				return _maxParameter;
			}
			set
			{
				_maxParameter = math.max(_minParameter, value);
			}
		}
		#endregion Properties

		#region Method
		/// <summary>
		/// Initialize.
		/// </summary>
		/// <param name="dataNb">Variable number</param>
		/// <param name="temporaryDataNb">Temporary variable number</param>
		/// <param name="allocator">Allocation type</param>
		private void Init(int dataNb, int temporaryDataNb, Allocator allocator)
		{
			_model = new DEModel(dataNb, temporaryDataNb, allocator);
			_minParameter = 0.0f;
			_maxParameter = 0.0f;
			_parameterSteps = new HashSet<float>();
			_solvingTypes = new HashSet<DESolvingType>();
		}

		/// <summary>
		/// Add a simulation with a new solving type.
		/// </summary>
		/// <param name="solvingType">New solving type</param>
		public void AddSolvingType(DESolvingType solvingType)
		{
			_solvingTypes.Add(solvingType);
		}

		/// <summary>
		/// Remove all selected solving types.
		/// </summary>
		public void ClearSolvingTypes()
		{
			_solvingTypes.Clear();
		}

		/// <summary>
		/// Add a simulation with a new parameter step.
		/// </summary>
		/// <param name="parameterStep">New parameter step</param>
		public void AddParameterStep(float parameterStep)
		{
			_parameterSteps.Add(parameterStep);
		}

		/// <summary>
		/// Remove all selected parameter steps.
		/// </summary>
		public void ClearParameterSteps()
		{
			_parameterSteps.Clear();
		}

		/// <summary>
		/// Launch analysis.
		/// </summary>
		/// <param name="reportName">Name of the report</param>
		/// <param name="reportPath">Report folder path</param>
		/// <param name="isFullReport">Generate a report with all simulation data</param>
		public IEnumerable<AnalysisProgression> Analyse(string reportName, string reportPath, bool isFullReport)
		{
			InitAnalysis();

			float minParameter;
			float maxParameter;
			HashSet<float> parameterSteps;

			if (_isNondimensionalized)
			{
				minParameter = _parameterNondimensionalizationFunction(in _model, _minParameter);
				maxParameter = _parameterNondimensionalizationFunction(in _model, _maxParameter);
				parameterSteps = new HashSet<float>();

				foreach (float parameterStep in _parameterSteps)
				{
					parameterSteps.Add(_parameterNondimensionalizationFunction(in _model, parameterStep));
				}
			}
			else
			{
				minParameter = _minParameter;
				maxParameter = _maxParameter;
				parameterSteps = _parameterSteps;
			}

			if (_variableType == Type.GetType("System.Single"))
			{
				FloatDEAnalyser analyser = new FloatDEAnalyser(_model, 
					_floatInitialVariableFunctionPointer, _floatDerivativeFunctionPointer, 
					_floatAnalyticalSolutionFunctionPointer, minParameter, maxParameter, _isNondimensionalized, 
					_floatVariableDimensionalizationFunctionPointer, _parameterDimensionalizationFunction);

				foreach (float parameterStep in parameterSteps)
				{
					analyser.ParameterSteps.Add(parameterStep);
				}

				foreach (DESolvingType solvingType in _solvingTypes)
				{
					analyser.SolvingTypes.Add(solvingType);
				}

				analyser.StartAnalysis();

				foreach (AnalysisProgression progression in analyser.CheckAnalysisProgression())
				{
					yield return new AnalysisProgression()
					{
						Ratio = progression.Ratio * 0.9f,
						Message = progression.Message
					};
				}

				yield return new AnalysisProgression()
				{
					Ratio = 0.9f,
					Message = "Report generation..."
				};

				FloatDEAnalysisReport report = analyser.GetAnalysisReport(isFullReport);
				report.Name = reportName;
				GenerateDefaultDescriptions(out string shortDescription, out string longDescription);
				report.ShortDescription = shortDescription;
				report.LongDescription = longDescription;
				report.MinParameter = _minParameter;
				report.MaxParameter = _maxParameter;
				AssetDatabase.CreateAsset(report, reportPath + "/" + reportName + ".asset");
				AssetDatabase.SaveAssets();

				yield return new AnalysisProgression()
				{
					Ratio = 1.0f,
					Message = "Finalization..."
				};
			}
			else if (_variableType == Type.GetType("Unity.Mathematics.float2"))
			{
				Float2DEAnalyser analyser = new Float2DEAnalyser(_model, 
					_float2InitialVariableFunctionPointer, _float2DerivativeFunctionPointer, 
					_float2AnalyticalSolutionFunctionPointer, minParameter, maxParameter, _isNondimensionalized, 
					_float2VariableDimensionalizationFunctionPointer, _parameterDimensionalizationFunction);


				foreach (float parameterStep in parameterSteps)
				{
					analyser.ParameterSteps.Add(parameterStep);
				}

				foreach (DESolvingType solvingType in _solvingTypes)
				{
					analyser.SolvingTypes.Add(solvingType);
				}

				analyser.StartAnalysis();

				foreach (AnalysisProgression progression in analyser.CheckAnalysisProgression())
				{
					yield return new AnalysisProgression()
					{
						Ratio = progression.Ratio * 0.9f,
						Message = progression.Message
					};
				}

				yield return new AnalysisProgression()
				{
					Ratio = 0.9f,
					Message = "Report generation..."
				};

				Float2DEAnalysisReport report = analyser.GetAnalysisReport(isFullReport);
				report.Name = reportName;
				GenerateDefaultDescriptions(out string shortDescription, out string longDescription);
				report.ShortDescription = shortDescription;
				report.LongDescription = longDescription;
				report.MinParameter = _minParameter;
				report.MaxParameter = _maxParameter;
				AssetDatabase.CreateAsset(report, reportPath + "/" + reportName + ".asset");
				AssetDatabase.SaveAssets();

				yield return new AnalysisProgression()
				{
					Ratio = 1.0f,
					Message = "Finalization..."
				};
			}
			else
			{
				// TODO : Add error log
				yield return new AnalysisProgression()
				{
					Ratio = float.NaN,
					Message = string.Empty
				};
			}
		}

		/// <summary>
		/// Dispose the data in the unmanaged memory.
		/// </summary>
		public void Dispose()
		{
			_model.Dispose();
		}
		#endregion Methods

		#region Abstract Methods
		/// <summary>
		/// Initialize the analysis.
		/// </summary>
		protected abstract void InitAnalysis();

		/// <summary>
		/// Generate the default report short and long descriptions.
		/// </summary>
		protected abstract void GenerateDefaultDescriptions(out string shortDescription, out string longDescription);
		#endregion Abstract Methods
	}
}
