namespace dExplorer.Editor.Mathematics
{
	using dExplorer.Runtime.Mathematics;
	using System.Collections.Generic;
	using Unity.Burst;
	using Unity.Collections;
	using Unity.Mathematics;
	using UnityEditor;

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

		private bool _isFloat1Activated;
		private FunctionPointer<FloatInitialVariableFunction> _floatInitialVariableFunction;
		private FunctionPointer<FloatDerivativeFunction> _floatDerivativeFunction;
		private FunctionPointer<FloatAnalyticalSolutionFunction> _floatAnalyticalSolutionFunction;

		private bool _isFloat2Activated;
		private FunctionPointer<Float2InitialVariableFunction> _float2InitialVariableFunction;
		private FunctionPointer<Float2DerivativeFunction> _float2DerivativeFunction;
		private FunctionPointer<Float2AnalyticalSolutionFunction> _float2AnalyticalSolutionFunction;
		#endregion Fields

		#region Constructors
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="dataNb">Variable number</param>
		/// <param name="allocator">Allocation type</param>
		public AnalysableDEModel(int dataNb, Allocator allocator)
		{
			_model = new DEModel(dataNb, allocator);
			_minParameter = 0.0f;
			_maxParameter = 0.0f;
			_parameterSteps = new HashSet<float>();
			_solvingTypes = new HashSet<DESolvingType>();

			_isFloat1Activated = false;
			_isFloat2Activated = false;
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
		#endregion Proeperties

		#region Methods
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
		/// Use the dimension 1 differential equation as simulation model.
		/// </summary>
		/// <param name="initialVariableFunction">Initial state function</param>
		/// <param name="derivativeFunction">Derivative computation function</param>
		/// <param name="analyticalSolutionFunction">Analytical solution computation function</param>
		protected void ActivateFloat1Dimension(
			FloatInitialVariableFunction initialVariableFunction,
			FloatDerivativeFunction derivativeFunction,
			FloatAnalyticalSolutionFunction analyticalSolutionFunction)
		{
			_isFloat1Activated = true;
			_isFloat2Activated = false;

			_floatInitialVariableFunction = BurstCompiler.CompileFunctionPointer<FloatInitialVariableFunction>(initialVariableFunction);
			_floatDerivativeFunction = BurstCompiler.CompileFunctionPointer<FloatDerivativeFunction>(derivativeFunction);
			_floatAnalyticalSolutionFunction = BurstCompiler.CompileFunctionPointer<FloatAnalyticalSolutionFunction>(analyticalSolutionFunction);
		}

		/// <summary>
		/// Use the dimension 2 differential equation as simulation model.
		/// </summary>
		/// <param name="initialVariableFunction">Initial state function</param>
		/// <param name="derivativeFunction">Derivative computation function</param>
		/// <param name="analyticalSolutionFunction">Analytical solution computation function</param>
		protected void ActivateFloat2Dimension(
			Float2InitialVariableFunction initialVariableFunction,
			Float2DerivativeFunction derivativeFunction,
			Float2AnalyticalSolutionFunction analyticalSolutionFunction)
		{
			_isFloat1Activated = false;
			_isFloat2Activated = true;

			_float2InitialVariableFunction = BurstCompiler.CompileFunctionPointer<Float2InitialVariableFunction>(initialVariableFunction);
			_float2DerivativeFunction = BurstCompiler.CompileFunctionPointer<Float2DerivativeFunction>(derivativeFunction);
			_float2AnalyticalSolutionFunction = BurstCompiler.CompileFunctionPointer<Float2AnalyticalSolutionFunction>(analyticalSolutionFunction);
		}

		/// <summary>
		/// Launch analysis.
		/// </summary>
		/// <param name="reportName">Name of the report</param>
		/// <param name="reportPath">Report folder path</param>
		public void Analyse(string reportName, string reportPath)
		{
			Init();

			if (_isFloat1Activated)
			{
				FloatDEAnalyser analyser = new FloatDEAnalyser(_model, _floatInitialVariableFunction, 
					_floatDerivativeFunction, _floatAnalyticalSolutionFunction, _minParameter, _maxParameter);

				foreach (float parameterStep in _parameterSteps)
				{
					analyser.ParameterSteps.Add(parameterStep);
				}

				foreach (DESolvingType solvingType in _solvingTypes)
				{
					analyser.SolvingTypes.Add(solvingType);
				}

				FloatDEAnalysisReport report = analyser.Analyse();
				report.Name = reportName;
				GenerateDefaultDescriptions(out string shortDescription, out string longDescription);
				report.ShortDescription = shortDescription;
				report.LongDescription = longDescription;
				AssetDatabase.CreateAsset(report, reportPath + "/" + reportName + ".asset");
				AssetDatabase.SaveAssets();
			}
			else if (_isFloat2Activated)
			{
				Float2DEAnalyser analyser = new Float2DEAnalyser(_model, _float2InitialVariableFunction,
					_float2DerivativeFunction, _float2AnalyticalSolutionFunction, _minParameter, _maxParameter);

				foreach (float parameterStep in _parameterSteps)
				{
					analyser.ParameterSteps.Add(parameterStep);
				}

				foreach (DESolvingType solvingType in _solvingTypes)
				{
					analyser.SolvingTypes.Add(solvingType);
				}

				Float2DEAnalysisReport report = analyser.Analyse();
				report.Name = reportName;
				GenerateDefaultDescriptions(out string shortDescription, out string longDescription);
				report.ShortDescription = shortDescription;
				report.LongDescription = longDescription;
				AssetDatabase.CreateAsset(report, reportPath + "/" + reportName + ".asset");
				AssetDatabase.SaveAssets();
			}
			else
			{
				// TODO : Add error log
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
		/// Initialize.
		/// </summary>
		protected abstract void Init();

		/// <summary>
		/// Generate the default report short and long descriptions.
		/// </summary>
		protected abstract void GenerateDefaultDescriptions(out string shortDescription, out string longDescription);
		#endregion Abstract Methods
	}
}
