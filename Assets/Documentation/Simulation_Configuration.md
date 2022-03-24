# Simulation configuration

The behaviour of ODE simulations depends on four parameter types :

- [Model constant](Simulation_Configuration.md#model-constant)
- [Parameter range](Simulation_Configuration.md#parameter-range)
- [Parameter step](Simulation_Configuration.md#parameter-step)
- [Solving type](Simulation_Configuration.md#solving-type)

> [Classic drag model](https://en.wikipedia.org/wiki/Drag_(physics)#The_drag_equation) will be used as example :
> 
> `v' = -0.5 *  Rho * S * Cx * v(t)^2 / m; `
> 
> - `Rho` is the density of the fluid [*kg.m^-3*]
> - `S` is the cross sectional area *[m^2]*
> - `Cx` is the drag coefficient *[N.A]*
> - `m` is the mass *[kg]*
> - `t` is the time parameter *[s]*
> - `v` is the speed relative to the fluid *[m.s^-1]*
> - `v'` is the acceleration relative to the fluid *[m.s^-2]*

## Model constant

ODE models can have constant values influencing the simulation behaviour.

> `Rho`, `S`, `Cx` and `m` are the model constants.
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

[TODO]
