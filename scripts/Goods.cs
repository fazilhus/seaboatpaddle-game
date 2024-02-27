using Godot;
using System;

public partial class Goods : RigidBody3D
{	
	[Export] private float floatForce = 1.0f;
	[Export] private float waterDrag = 0.005f;
	[Export] private float WaterAngularDrag = 0.01f;
	[Export] private bool isSubmerged = false;
	private float gravity;

	private float initialY;
	private double elapsedTime = 0;
	[Export] public WaterPlane water;
	public double time = 0;

	[Export]
	public bool isActive = true;

	public Godot.Collections.Array<Node> probeContainer;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var parent = GetParent();
		gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
		//water = GetNode<WaterPlane>("/root/Main/WaterPlane");
		probeContainer = GetNode<Node3D>("ProbeContainer").GetChildren();

		initialY = GlobalPosition.Y;

		if (!isActive) {
			ProcessMode = ProcessModeEnum.Disabled;
			GetNode<Area3D>("Area3DTrigger").ProcessMode = ProcessModeEnum.Disabled;
		}
	}
	
	public override void _PhysicsProcess(double delta)
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
	
	public override void _IntegrateForces(PhysicsDirectBodyState3D state)
	{
		if(isSubmerged)
		{
			state.LinearVelocity *= 1 - waterDrag;
			state.AngularVelocity *= 1 - WaterAngularDrag;
		}
	}
	
	private void OnArea3dTriggerAreaEntered(Area3D area)
	{
		if (area.IsInGroup("ThePlayersGoods"))
		{
			GD.Print("ThePlayers are colliding with goods");
			QueueFree();
		}
	}

	private void OnCrashCooldownTimerTimeout() {
		GetNode<Area3D>("Area3DTrigger").SetDeferred("monitoring", true);
	}
}
