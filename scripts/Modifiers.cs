using Godot;
using System;


public partial class Modifiers : RigidBody3D
{
	
	[Export] private float floatForce = 1.0f;
	[Export] private float waterDrag = 0.005f;
	[Export] private float WaterAngularDrag = 0.01f;
	[Export] private bool isSubmerged = false;
	private float gravity;

	private float initialY;
	private double elapsedTime = 0;
	[Export] public WaterPlane water;
	[Export] public Area3D boatCollider;
	[Export] public Area3D modifiersCollider;
	public double time = 0;
	
	public Godot.Collections.Array<Node> probeContainer;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var parent = GetParent();
		gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
		probeContainer = GetNode<Node3D>("ProbeContainer").GetChildren();
		initialY = GlobalPosition.Y;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		isSubmerged = false;
		foreach(Marker3D p in probeContainer)
		{
			float depth = (float)water.getHeight(GlobalPosition) - p.GlobalPosition.Y;

			 if(depth > 0)
			 {
				isSubmerged = true;
				ApplyForce(Godot.Vector3.Up * floatForce * gravity * depth, p.GlobalPosition - GlobalPosition);
			 }	
		}
	}
	
	public override void _IntegrateForces(PhysicsDirectBodyState3D state) // changing the simulation state of the object
	{
		if(isSubmerged)
		{
			state.LinearVelocity *= 1 - waterDrag;
			state.AngularVelocity *= 1 - WaterAngularDrag;
		}
	}
	
	private Random random;

	public NumberGenerator()
	{
		// Initialize the random number generator
		random = new Random();
	}

	public int GenerateNumber()
	{
		// Generate a random number between 1 and 5 (inclusive)
		return random.Next(1, 6);
	}
	
	public void OnArea3dTriggerAreaEntered(Area3D area)
	{
		if (area.IsInGroup("ThePlayers"))
		{
			GD.Print("ThePlayers are colliding with modifiers");
			QueueFree();
			
			NumberGenerator generator = new NumberGenerator();
			int randomNumber = generator.GenerateNumber();
			if randomNumber == 1
			{
				//Add extra time
				GD.Print("Extra time added");
			}
			if randomNumber == 2
			{
				//Drunken capten, swinging camera
				GD.Print("Drunken Captain mode on");
			}
			if randomNumber == 3
			{
				//Repairing boat
				GD.Print("Repairkit found, boat repaired");
			}
			if randomNumber == 4
			{
				//Control inversion
				GD.Print("The controlls are now inverted");
			}
			if randomNumber == 5
			{
				//Speed boost
				GD.Print("You found a speed boost, press A to use it and get a speedboost straight a head");
			}
		}
	}
}
