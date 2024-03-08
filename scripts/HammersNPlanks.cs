using Godot;
using System;

public partial class HammersNPlanks : RigidBody3D
{
	[Export]
	public Boat boat;
	public static int amountOfRepairKits = 0;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Spawned repairkit");
		GravityScale = 0;
	}

	public static void ResetModifiers() 
	{
		amountOfRepairKits = 0;
	}
	
	public void OnArea3dTriggerAreaEntered(Area3D area)
	{
		if (area.IsInGroup("ThePlayers"))
		{
			GD.Print("Removed repairkit");
			QueueFree();
			amountOfRepairKits += 1;
			boat.ActivateRepairKit(amountOfRepairKits);
			GameCamera.LabelModifiers.Text ="Repair kit found, press B to use";
			GameCamera.RepairKitModifierLabel.Text = "";
			GameCamera.RepairKitModifierLabel.Text += amountOfRepairKits;
		}
	}
}
