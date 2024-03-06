using Godot;
using System;

public partial class Clock : RigidBody3D
{
	public Boat boat;
	private AudioStreamPlayer clockSound;
	public override void _Ready()
	{
		GravityScale = 0;
		clockSound = GetNode<AudioStreamPlayer>("ClockSound");
	}

	public void OnArea3dTriggerAreaEntered(Area3D area)
	{
		if (area.IsInGroup("ThePlayers"))
		{
			QueueFree();
			GD.Print("Extra time added");
			GameCamera.ActivateExtraTime();
			GameCamera.LabelModifiers.Text ="Extra time added";
			clockSound.Playing = true;
			clockSound.Play();
		}
	}
}
