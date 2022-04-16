# Custom ODE model

Custom [ODE](Introduction.md) models can (must) be added by inheriting from two abstract classes :

 - `AnalysableDEModel`
 - `AnalysableDEModelVisualizer`

> [Classic drag model](https://en.wikipedia.org/wiki/Drag_(physics)#The_drag_equation) without additional force will be used as example :
> 
> `v' = -0.5 *  Rho * S * Cx * v(t)^2 / m`
> 
> - `Rho` is the density of the fluid [*kg.m^-3*]
> - `S` is the cross sectional area *[m^2]*
> - `Cx` is the drag coefficient *[N.A]*
> - `m` is the mass *[kg]*
> - `t` is the time parameter *[s]*
> - `v` is the speed relative to the fluid *[m.s^-1]*, `v >= 0`
> - `v'` is the acceleration relative to the fluid *[m.s^-2]*
> - `t_min = 0` and `v(t_min) = v_init`

## AnalysableDEModel

`AnalysableDEModel` is the core class of the model.

> The empty model class :
> ```
> [BurstCompile]
> public unsafe class DragModel : AnalysableDEModel { }
> ```

It contains all computing functions for the analysis :

 - `[]InitialVariableFunction` : Get the value of the variable at the beginning of the simulation
 - `[]PreSimulationFunction` : Execute the code at the beginning of each parameter iteration 
 - `[]PostSimulationFunction` : Execute the code at the end of each parameter iteration 
 - `[]DerivativeFunction` : Compute the derivative of the variable
 - `[]AnalyticalSolutionFunction` : Compute the analytical solution of the model
 - `[]VariableDimensionalizationFunction` : [Dimensionalize](https://en.wikipedia.org/wiki/Nondimensionalization) the non-dimensionalized variable (if the model is non-dimensionnalized)
 - `ParameterNondimensionalizationFunction` : [Non-dimensionalize](https://en.wikipedia.org/wiki/Nondimensionalization) the parameter (if the model is non-dimensionnalized)
 - `ParameterDimensionalizationFunction` : [Dimensionalize](https://en.wikipedia.org/wiki/Nondimensionalization) the non-dimensionalized parameter (if the model is non-dimensionnalized)

and a few additional methods :

 - `InitAnalysis` : First method called during the analysis
 - `GenerateDefaultDescriptions` : Analysis description in the [analysis report](Analysis_Report.md)

Computing functions are non-generic Delegates and several variable types are available (`float` and `float2`). Delegates names differs therefore according to the variable type of the model.

> The drag model has only the longitudinal speed (`v`) as variable (dimension 1). The selected Delegates are therefore :
> 
> - `FloatInitialVariableFunction`
> - `FloatPreSimulationFunction`
> - `FloatPostSimulationFunction`
> - `FloatDerivativeFunction`
> - `FloatAnalyticalSolutionFunction`
> - `FloatVariableDimensionalizationFunction`

Delegates customize the behaviour of the [simulation job](Architecture.md) :

![Simulation Job](Images/Simulation_Job.png "Simulation Job")

All model data have to be stored in the `_model` attribute containing two static arrays :

 - The constant data array
 - The temporay data array

> Drag model has 5 constant data :
>  - mass `m`
>  - fluid density `Rho`
>  - reference surface `S`
>  - drag coefficient `Cx`
>  - initial speed `v_init`
>  
>   and 1 temporary data :
>  - coefficient A : `-0.5 *  Rho * S * Cx / m`
>  
> ```
> [BurstCompile]
> public unsafe class DragModel : AnalysableDEModel
> {
>    #region Properties
>    public float Mass
>    {
>       get { return _model.GetDataValue(0); }
>       set { _model.SetDataValue(0, value); }
>    }
>   
>    public float FluidDensity
>    {
>       get { return _model.GetDataValue(1); }
>       set { _model.SetDataValue(1, value); }
>    }
>   
>    public float ReferenceSurface
>    {
>       get { return _model.GetDataValue(2); }
>       set { _model.SetDataValue(2, value); }
>    }
>   
>    public float DragCoefficient
>    {
>       get { return _model.GetDataValue(3); }
>       set { _model.SetDataValue(3, value); }
>    }
>   
>    public float InitialSpeed
>    {
>       get { return _model.GetDataValue(4); }
>       set { _model.SetDataValue(4, value); }
>    }
>    
>    // Add t_min = 0 condition
>    public override float MinParameter
>    {
>       get { return _minParameter; }
>       set 
>       { 
>         _minParameter = 0.0f;
>         _maxParameter = math.max(0.0f, _maxParameter);
>       }
>    }
>    #endregion Properties
>    
>    #region Methods
>    protected override void InitAnalysis()
>    {
>      float coefficientA = -0.5f * FluidDensity * ReferenceSurface * DragCoefficient;
>      _model.SetTemporaryDataValue(0, coefficientA);
>    }
>    #endregion Methods
> }
> ```
> 
> The drag equation can be [non-dimensionalized](Simple_Drag_Model_Solving.md). The new variable is `V` and the new parameter is `T` :
> 
> `V = v / v_init`
> 
> `T = v_init * A * t` with `A = 0.5 * Rho * S * Cx`
