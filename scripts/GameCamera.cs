using Godot;
using System;

public partial class GameCamera : Node3D
{
	
	private Timer countdownTimer;
	private int remainingTime = 800;
	
	[Export]
	public Node3D Boat;
	[Export]
	public Vector3 position_offset = new Vector3(0, 10, -7);
	[Export]
	public Vector3 rotation_offset = new Vector3(-120, 0, 180);
	[Export]
	public static float swayAmount = 0;
	[Export]
	public float swaySpeed = 2f;
	private float swayTimer = 0f;
	public static bool DrunkenCaptain { get; private set; } = false;
	
	public static Label LabelPlayers;
	public static Label LabelTime;
	public static Label LabelModifiers;
	
	public static void ActivateDrunkenCaptain()
	{
		DrunkenCaptain = true;
	}
	
	public override void _Ready()
	{
		GetNode<Timer>("countdownTimer").Start();
		
		LabelPlayers = GetNodeOrNull<Label>("CanvasLayer/LabelPlayers");
		LabelTime = GetNodeOrNull<Label>("CanvasLayer/LabelTime");
		LabelModifiers = GetNodeOrNull<Label>("CanvasLayer/LabelModifiers");
		
		//for(int i = 0; i < PlayerMenu.playerAmount; i++){
		//	LabelPlayers.Text += "\nPlayer " + PlayerMenu.playerIds[i];
		//}
				
	}
	
	public override void _Process(double delta)
	{
		Position = Boat.Position + position_offset;
		RotationDegrees = new Vector3(0, Boat.Rotation.Y, 0) + rotation_offset;
		
		if(DrunkenCaptain)
		{
			float swayAngle = Mathf.Sin(swayTimer) * swayAmount;
			RotationDegrees += new Vector3(0, swayAngle, 0);

			// Update the timer for the next frame
			swayTimer += (float)delta * swaySpeed;
		}
	}
	private void OnCountdownTimerTimeout()
	{
		// Decrement remaining time
		if (remainingTime > 0)
		{
			remainingTime--;
		}
		

		// Update the display to show the remaining time
		LabelTime.Text = "Time: " + remainingTime.ToString();

		// Check if the countdown has reached zero
		if (remainingTime <= 0)
		{
			// Handle timer expiration (e.g., game over)
			GD.Print("Time's up!");
			countdownTimer.Stop(); // Stop the timer if needed
		}
	}
}



