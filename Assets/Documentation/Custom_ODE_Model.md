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
> - `v` is the speed relative to the fluid *[m.s^-1]*, `v > 0`
> - `v'` is the acceleration relative to the fluid *[m.s^-2]*

## AnalysableDEModel

`AnalysableDEModel` is the core class of the model.

It contains all computing functions for the analysis :

 - `InitialVariableFunction` : Get the value of the variable at the beginning of the simulation
 - `PreSimulationFunction` : Execute the code at the beginning of each parameter iteration 
 - `PostSimulationFunction` : Execute the code at the end of each parameter iteration 
 - `DerivativeFunction` : Compute the derivative of the variable
 - `AnalyticalSolutionFunction` : Compute the analytical solution of the model
 - `VariableDimensionalizationFunction` : [Dimensionalize](https://en.wikipedia.org/wiki/Nondimensionalization) the non-dimensionalized variable (if the model is non-dimensionnalized)
 - `ParameterNondimensionalizationFunction` : [Non-dimensionalize](https://en.wikipedia.org/wiki/Nondimensionalization) the parameter (if the model is non-dimensionnalized)
 - `ParameterDimensionalizationFunction` : [Dimensionalize](https://en.wikipedia.org/wiki/Nondimensionalization) the non-dimensionalized parameter (if the model is non-dimensionnalized)

and a few additional methods :

 - `InitAnalysis` : First method called during the analysis
 - `GenerateDefaultDescriptions` : Analysis description in the [analysis report](Analysis_Report.md)

Computing functions are non-generic Delegates and several variable types are available (`float`, `float2`). Delegates names differs therefore according to the variable type of the model.
