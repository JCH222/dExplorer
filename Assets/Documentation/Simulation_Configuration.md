# Simulation configuration

The behaviour of [ODE](Introduction.md) simulations depends on four parameter types :

- [Model constant](Simulation_Configuration.md#model-constant)
- [Parameter range](Simulation_Configuration.md#parameter-range)
- [Parameter step](Simulation_Configuration.md#parameter-step)
- [Solving type](Simulation_Configuration.md#solving-type)

> [Classic drag model](https://en.wikipedia.org/wiki/Drag_(physics)#The_drag_equation) will be used as example :
> 
> `v' = -0.5 *  Rho * S * Cx * v(t)^2 / m + F / m`
> 
> - `Rho` is the density of the fluid [*kg.m^-3*]
> - `S` is the cross sectional area *[m^2]*
> - `Cx` is the drag coefficient *[N.A]*
> - `m` is the mass *[kg]*
> - `t` is the time parameter *[s]*
> - `v` is the speed relative to the fluid *[m.s^-1]* and `v >= 0`
> - `v'` is the acceleration relative to the fluid *[m.s^-2]*
> - `F` is the additional force *[N]* 

## Model constant

ODE models can have constant values influencing the simulation behaviour.

> `Rho`, `S`, `Cx` `m` and `F` are the model constants.
>
> [ADD SIMULATIONS RESULTS WITH DIFFERENT VALUES]

## Parameter range

Simulations must have starting and stopping conditions via the min/max parameters.

> - `t_min` is the min parameter *[s]*
> - `t_max` is the max parameter *[s]*
> - `t_min <= t <= t_max`

## Parameter step

Numerical simulations are discretized. It is therefore necessary to define the simulation frequency through the parameter step value.

NB : dExplorer doesn't handle non-constant paramater steps.

> [ADD SIMULATIONS RESULTS WITH DIFFERENT VALUES]



## Solving type

Numerical simulations are discretized. It is therefore necessary to define the simulation solving type.

If the ODE form is :

> `| v' = f(v(t),t) with  t_min <= t <= t_max`
>
> `| v' = dv / dt`

The analytical solution of `v(t_max)` is :

> `| v(t_max) = v(t_min) + INTEGRATION[t_min->t_max] (dv / dt)`
>
> `| v(t_max) = v(t_min) + INTEGRATION[t_min->t_max] (f(v(t),t))`

But the numerical solution is :

> `| v(t_max) = v(t_min) + SUM[INTEGRATION[t->t+h] (f(v(t),t)] with t_min <= t <= t_max`

`h` is the parameter step used to define the range of each integration in the sum.

It can be viewed in a graphic :

> [ADD GRAPHIC]

Contrary to the anlalytical solution, the numerical solution is an approximation. In fact, each integration of the sum is computed with a specific solving type.
The selected solving type has an impact on :
 - The accuracy
 - The computational load
 - The edge conditions

### Explicit Euler

[TODO]

### Explicit Runge-Kutta

[TODO]

#### Second order

[TODO]

#### Fourth order

[TODO]
