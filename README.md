Poly[n]catenane Chain Simulator
---

Please refer to the following publication for further information.

Wang et al, "Isolated chains of single-crystalline linear poly[n]catenanes exhibit unique mechanical damping" 2025
For correspondence, please contact the corresponding authors listed on the manuscript. For technical details about this simulation, feel free to contact Omid Amiri on github at: github.com/OmidAmiriE or by e-mail: omid.f.amiri@gmail.com

Brief Technical Description
---

This is an interactive real-time mechanical model of the catenated chained behaviour with Langevin dynamics. The 3D models used for collisions are simplified rhombus shaped rigid body approximations of the monomers. Deterministic forces, collisions and damping effects are simulated in real time by the PhysX engine in Unity according to model specifications. The parameter "Damping strength" (arbitrary units) in the app interface modifies the drag coefficient of the rigid bodies. On top of that, the model simulates random Brownian forces applied to the chain, as well as inter-monomer stacking interactions.

Random forces are applied on line 263 of ChainGen.cs to each monomer rigid body (both linear and angular) by function "ApplyRandomForce" which is called once every physics update cycle. This function samples from a standard gaussian distribution using the "Brownian" function on line 273. The parameter "Brownian strength" (arbitrary units) in the app interface modifies the amplitude of the applied random force.

Inter-monomer forces are approximated by a clipped harmonic potential and applied on line 31 and 32 of SatelScript.cs between the anthracene moieties of every second monomer. The "Stacking strength" (arbitrary units) in the app interface modifies the amplitude of the applied stacking force.

The first monomer is attached to a fixed point by a stiff spring joint. Pulling is carried out by moving another stiff spring joint attached to the last monomer at a velocity controlled by "Pulling velocity" (arbitrary units) in the app interface. This is done on line 188 of ChainGen.cs in every physics update cycle during pulling until a threshold force is reached (to avoid separation). Alternatively, the "Constant force" parameter (arbitrary units) in the app interface controls the force applied to the last monomer.

The default values of the parameters in the app allow for ready observation of difference in monomer re-orientation frequency when the chain is pulled at a slow velocity (e.g. 10) as opposed to a high velocity (e.g. 200).

Camera movement for close examination is controlled by keyboard and mouse inputs.
Version 1.0.0


	
