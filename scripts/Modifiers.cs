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
	public static int amountOfRepairKits = 0;
	public static int amountOfSpeedBoosts = 0;

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

	public static void ResetModifiers() {
		amountOfRepairKits = 0;
		amountOfSpeedBoosts = 0;
	}
	
	public void OnArea3dTriggerAreaEntered(Area3D area)
	{
		if (area.IsInGroup("ThePlayers"))
		{
			QueueFree();
			
			NumberGenerator generator = new NumberGenerator();
			int randomNumber = generator.GenerateNumber(1, 5);
			if (randomNumber == 1)
			{
				GameCamera.ActivateExtraTime();
				GameCamera.LabelModifiers.Text ="Extra time added";
			}
			if (randomNumber == 2)
			{
				GameCamera.ActivateDrunkenCaptain();
				GameCamera.swayAmount += 3;
				GameCamera.LabelModifiers.Text ="Rum found";
			}
			if (randomNumber == 3)
			{
				amountOfRepairKits += 1;
				boat.ActivateRepairKit(amountOfRepairKits);
				GameCamera.LabelModifiers.Text ="Repair kit found, press B to use";
				GameCamera.RepairKitModifierLabel.Text = "";
				GameCamera.RepairKitModifierLabel.Text += amountOfRepairKits;
			}

			if (randomNumber == 4)
			{
				amountOfSpeedBoosts += 1;
				boat.ActivateSpeedBoost(amountOfSpeedBoosts);
				GameCamera.LabelModifiers.Text ="Speed Boost found, press A to use";
				GameCamera.SpeedBoostModifierLabel.Text = "";
				GameCamera.SpeedBoostModifierLabel.Text += amountOfSpeedBoosts;

			}
		}
	}

}
