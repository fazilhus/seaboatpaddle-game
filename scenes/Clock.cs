using Godot;
using System;

public partial class Clock : RigidBody3D
{
	public Boat boat;

	public override void _Ready()
	{
		GravityScale = 0;
	}

	public void OnArea3dTriggerAreaEntered(Area3D area)
	{
		if (area.IsInGroup("ThePlayers"))
		{
			QueueFree();
			GameCamera.ActivateExtraTime();
			GameCamera.LabelModifiers.Text ="Extra time added";
		}
	}
}
