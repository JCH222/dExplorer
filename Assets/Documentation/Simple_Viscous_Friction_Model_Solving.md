# Simple viscous friction model solving

Classic viscous friction model of a rotating cylinder in a fluid without additional force :

![Simple Viscous Friction Equation 1](Images/Simple_Viscous_Friction_Equation_1.png "[Simple Viscous Friction Equation 1")

![Simple Viscous Friction Equation 2](Images/Simple_Viscous_Friction_Equation_2.png "[Simple Viscous Friction Equation 2")

- `Alpha` is the viscous friction coefficient of the fluid [*N.s.m^-1*]
- `r` is the radius of the cylinder *[m]*
- `I` is the moment of inertia *[kg.m^2]*
- `t` is the time parameter *[s]*
- `Theta_dot` is the angular speed relative to the fluid *[rad.s^-1]*,
- `dTheta_dot / dt` is the angular acceleration relative to the fluid *[rad.s^-2]*

Angular speed tends towards zero over time. It can therefore be non-dimensionalized with the initial angular speed `Theta_dot_init` :

![Simple Viscous Friction Angular Speed Non Dimensionalization](Images/Simple_Viscous_Friction_Angular_Speed_Non_Dimensionalization.png "Simple Viscous Friction Angular Speed Non Dimensionalization")

`Theta_dot_star` is the non-dimensionalized angular speed. It start with a value of 1 and tends towards zero over time.

![Simple Viscous Friction Non Dimensionalized Equation 1](Images/Simple_Viscous_Friction_Non_Dimensionalized_Equation_1.png "Simple Viscous Friction Non Dimensionalized Equation 1")

![Simple Viscous Friction Non Dimensionalized Equation 2](Images/Simple_Viscous_Friction_Non_Dimensionalized_Equation_2.png "Simple Viscous Friction Non Dimensionalized Equation 2")

![Simple Viscous Friction Non Dimensionalized Equation 3](Images/Simple_Viscous_Friction_Non_Dimensionalized_Equation_3.png "Simple Viscous Friction Non Dimensionalized Equation 3")

`t_star` is the non-dimensionalized time :

![Simple Viscous Friction Time Non Dimensionalization](Images/Simple_Viscous_Friction_Time_Non_Dimensionalization.png "Simple Viscous Friction Time Non Dimensionalization")

![Simple Viscous Friction Non Dimensionalized Equation 4](Images/Simple_Viscous_Friction_Non_Dimensionalized_Equation_4.png "Simple Viscous Friction Non Dimensionalized Equation 4")

The non-dimensionalized viscous friction equation is therefore :

![Simple Viscous Friction Equation Solution 1](Images/Simple_Viscous_Friction_Equation_Solution_1.png "Simple Viscous Friction Equation Solution 1")

![Simple Viscous Friction Equation Solution 2](Images/Simple_Viscous_Friction_Equation_Solution_2.png "Simple Viscous Friction Equation Solution 2")

`C` is a constant.

![Simple Viscous Friction Equation Solution 3](Images/Simple_Viscous_Friction_Equation_Solution_3.png "Simple Viscous Friction Equation Solution 3")

The non-dimensionalized analytical solution is therefore :

![Simple Viscous Friction Equation Solution 4](Images/Simple_Viscous_Friction_Equation_Solution_4.png "Simple Viscous Friction Equation Solution 4")

The position equation can be solved :

![Simple Viscous Friction Position Equation 1](Images/Simple_Viscous_Friction_Position_Equation_1.png "Simple Viscous Friction Position Equation 1")
