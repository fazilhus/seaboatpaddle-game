using Godot;
using System;

public partial class streams : Node3D
{
	[Export] public Boat boat;
	private bool isStreamCollided;
	[Export] public int streamSpeed = 7;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		boat = GetParent().GetNode<Boat>("Boat");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if(isStreamCollided && boat.isSubmerged)
		{
			Vector3 ForceVector = Transform.Basis.Z;
			boat.ApplyCentralForce(ForceVector * streamSpeed);
			//GD.Print(ForceVector);
		}

	}
	public void OnArea3DEntered(Area3D area)
	{

		if(area.IsInGroup("ThePlayers"))
		{
			GD.Print("you have entered doom");
			isStreamCollided = true;
		}
	}
	public void OnArea3DExited(Area3D area)
	{
		if(area.IsInGroup("ThePlayers"))
		{
			isStreamCollided = false;
			GD.Print("has exited collider");
		}
	}
}
