# dExplorer

## What is it ?

**dExplorer is an ordinary differential equation analysis tool**. It provides interfaces in order to create custom differential equation models and generates reports containing simulation accuracies (depending on several simulation parameters).

> Differential equation is an equation that relates one or more unknown functions and their derivatives.
> Ordinary differential equation (ODE) is a subcategory that involves one variable to its derivatives (as opposed to partial  differential equation).
> **dExplorer can only analyse first order ODE**, corresponding to this form :
> 
> `| y' = f(y(t),t) with  t_min <= t <= t_max`
> 
> `| y(t_min) = y_t_min`
> 
> - `t` is the parameter
> - `y` is the variable
> - `y'` is the derivative variable
> - `f(y(t),t)` is the derivative function

It can be used for :

 - defining the valid scope of the ODE simulation
 - comparing the efficiency of different simulation configurations
 - studying the impact of different numerical error types

Simulation accuracy is calculated from the exact values. **The simulated ODE must therefore have an analytical solution** (implemented in the custom model).

## Where to get it ?

[TODO]

## How to use it ?

[TODO]

## Would you like to know more ?

 - [Simulation configuration](Simulation_Configuration.md)
 - [Custom ODE model](Custom_ODE_Model.md)
 - [Analysis report](Analysis_Report.md)
 - [Numerical errors](Numerical_Errors.md)
 - [Architecture](Architecture.md)
 - [Models solving](Models_Solving.md)
