using Godot;
using System;

public partial class sea_mine : RigidBody3D
{
	[Export] private float floatForce = 1.0f;
	[Export] private float waterDrag = 0.005f;
	[Export] private float WaterAngularDrag = 0.01f;
	[Export] private bool isSubmerged = false;

	private Boat boat;
	private float gravity;

	private float initialY;
	private double elapsedTime = 0;
	private GpuParticles3D particleExplosion;
	[Export] public WaterPlane water;
	private MeshInstance3D mineMaterial;
	private AudioStreamPlayer3D boomSound;

	public Godot.Collections.Array<Node> probeContainer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var parent = GetParent();
		gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
		probeContainer = GetNode<Node3D>("ProbeContainer").GetChildren();
		particleExplosion = GetNode<GpuParticles3D>("Node3D/GPUParticles3D");
		mineMaterial = GetNode<MeshInstance3D>("MeshInstance3D");
		boomSound = GetNode<AudioStreamPlayer3D>("Boom");


		

	
		initialY = GlobalPosition.Y;
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
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
    public override void _IntegrateForces(PhysicsDirectBodyState3D state) // changing the simulation state of the object
    {
		if(isSubmerged)
		{
			state.LinearVelocity *= 1 - waterDrag;
			state.AngularVelocity *= 1 - WaterAngularDrag;
		}
    }
	public void OnArea3DTriggerAreaEntered(Area3D area)
	{
		if (area.IsInGroup("ThePlayers"))
		{
			GD.Print("ThePlayers are colliding with the mine");
			mineMaterial.Hide();
			DisableCollisions();
			particleExplosion.Emitting = true;
			boomSound.Playing = true;
			
		}
		
	}

	private void DisableCollisions() {
		GetNode<CollisionShape3D>("CollisionShape3D").SetDeferred("disabled", true);
		var area = GetNode<Area3D>("CollisionTrigger");
		area.SetDeferred("monitoring", false);
		area.SetDeferred("monitorable", false);
	}

	private void OnEmittingFinshed() {
		GD.Print("Finished emitting, ready to die");
		QueueFree();
	}
}
