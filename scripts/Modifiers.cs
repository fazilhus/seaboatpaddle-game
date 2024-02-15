using Godot;
using System;


public partial class Modifiers : RigidBody3D
{
	[Export]
	public Boat boat;
	[Export] private float floatForce = 0.7f;
	[Export] private float waterDrag = 0.05f;
	[Export] private float WaterAngularDrag = 0.5f;
	[Export] private bool isSubmerged = false;
	private float gravity;

	private float initialY;
	private double elapsedTime = 0;
	[Export] public WaterPlane water;
	public double time = 0;
	
	public Godot.Collections.Array<Node> probeContainer;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
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
				ApplyForce(Vector3.Up * floatForce * gravity * depth, p.GlobalPosition - GlobalPosition);
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
	
	public void OnArea3dTriggerAreaEntered(Area3D area)
	{
		if (area.IsInGroup("ThePlayers"))
		{
			GD.Print("ThePlayers collided with modifiers");
			QueueFree();
			
			NumberGenerator generator = new NumberGenerator();
			int randomNumber = generator.GenerateNumber();
			if (randomNumber == 1)
			{
				//Add extra time
				GD.Print("Extra time added");
			}
			if (randomNumber == 2)
			{
				GameCamera.ActivateDrunkenCaptain();
				GameCamera.swayAmount += 3;
				GD.Print("Rum found");
			}
			if (randomNumber == 3)
			{
				boat.ActivateRepairKit();
				GD.Print("Repair kit found, press A to repair boat");
			}
			if (randomNumber == 4)
			{
				boat.ActivateControlInversion();
				GD.Print("'Control Inversion' mode on");
			}
			if (randomNumber == 5)
			{
				boat.ActivateSpeedBoost();
				GD.Print("Speed Boost found, press A to use it and get a speed boost straight a head");
			}
		}
	}
}

public class NumberGenerator
{
	private Random random;

	public NumberGenerator()
	{
		// Initialize the random number generator
		random = new Random();
	}

	public int GenerateNumber()
	{
		// Generate a random number between 1 and 5 (inclusive)
		return random.Next(3, 6);
	}
}
