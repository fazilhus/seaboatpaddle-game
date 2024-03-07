using Godot;
using System;

public partial class SuperFan : RigidBody3D
{
	[Export]
	public Boat boat;
	public static int amountOfSpeedBoosts = 0;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GravityScale = 0;
	}

	public static void ResetModifiers() 
	{
		amountOfSpeedBoosts = 0;
	}
	
	public void OnArea3dTriggerAreaEntered(Area3D area)
	{
		if (area.IsInGroup("ThePlayers"))
		{
			QueueFree();
			amountOfSpeedBoosts += 1;
			boat.ActivateSpeedBoost(amountOfSpeedBoosts);
			GameCamera.LabelModifiers.Text ="Speed Boost found, press A to use";
			GameCamera.SpeedBoostModifierLabel.Text = "";
			GameCamera.SpeedBoostModifierLabel.Text += amountOfSpeedBoosts;
		}
	}
}
