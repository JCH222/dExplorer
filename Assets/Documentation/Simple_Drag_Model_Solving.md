# Simple drag model solving

[Classic drag model](https://en.wikipedia.org/wiki/Drag_(physics)#The_drag_equation) without additional force :

![Simple Drag Equation 1](Images/Simple_Drag_Equation_1.png "Simple Drag Equation 1")

- `Rho` is the density of the fluid [*kg.m^-3*]
- `S` is the cross sectional area *[m^2]*
- `Cx` is the drag coefficient *[N.A]*
- `m` is the mass *[kg]*
- `t` is the time parameter *[s]*
- `v` is the speed relative to the fluid *[m.s^-1]*,
- `dv / dt` is the acceleration relative to the fluid *[m.s^-2]*

if `A = 0.5 * Rho * S * Cx` :

![Simple Drag Equation 2](Images/Simple_Drag_Equation_2.png "Simple Drag Equation 2")

Speed tends towards zero over time. It can therefore be non-dimensionalized with the initial speed `v_init` :

![Simple Drag Speed Non Dimensionalization](Images/Simple_Drag_Speed_Non_Dimensionalization.png "Simple Drag Speed Non Dimensionalization")

`V` is the non-dimensionalized speed. It start with a value of 1 and tends towards zero over time.

![Simple Drag Non Dimensionalized Equation 1](Images/Simple_Drag_Non_Dimensionalized_Equation_1.png "Simple Drag Non Dimensionalized Equation 1")

![Simple Drag Non Dimensionalized Equation 2](Images/Simple_Drag_Non_Dimensionalized_Equation_2.png "Simple Drag Non Dimensionalized Equation 2")

`T` is the non-dimensionalized time :

![Simple Drag Time Non Dimensionalization](Images/Simple_Drag_Time_Non_Dimensionalization.png "Simple Drag Time Non Dimensionalization")

The non-dimensionalized drag equation is therefore :

![Simple Drag Non Dimensionalized Equation 3](Images/Simple_Drag_Non_Dimensionalized_Equation_3.png "Simple Drag Non Dimensionalized Equation 3")

![Simple Drag Equation Solution 1](Images/Simple_Drag_Equation_Solution_1.png "Simple Drag Equation Solution 1")

![Simple Drag Equation Solution 2](Images/Simple_Drag_Equation_Solution_2.png "Simple Drag Equation Solution 2")

`C` is a constant.

![Simple Drag Equation Solution 3](Images/Simple_Drag_Equation_Solution_3.png "Simple Drag Equation Solution 3")

The non-dimensionalized analytical solution is therefore :

![Simple Drag Equation Solution 4](Images/Simple_Drag_Equation_Solution_4.png "Simple Drag Equation Solution 4")
