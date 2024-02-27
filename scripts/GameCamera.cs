using Godot;
using System;

public partial class GameCamera : Node3D
{
	[Signal]
	public delegate void CountdownTimerTimeoutEventHandler();
	
	[Export]
	public Timer countdownTimer;
	private int remainingTime = 60;
	private int remainingMinutes = 1;
	private int currentSeconds = 60;
	private int currentTime = 60;

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
	public static bool ExtraTime { get; private set; } = true;
	
	public static Label LabelPlayers;
	public static Label LabelTime;
	public static Label LabelModifiers;
	public static Panel RepairKitModifierPanel;
    public static Panel SpeedBoostModifierPanel;
	public static Label SpeedBoostModifierLabel;
    public static Label RepaitKitModifierLabel;

    public static void ActivateDrunkenCaptain()
	{
		DrunkenCaptain = true;
	}
	
	public static void ActivateExtraTime()
	{
		ExtraTime = true;
	}
	
	public override void _Ready()
	{
		countdownTimer.Start();
		
		LabelPlayers = GetNodeOrNull<Label>("CanvasLayer/LabelPlayers");
		LabelTime = GetNodeOrNull<Label>("CanvasLayer/LabelTime");
		LabelModifiers = GetNodeOrNull<Label>("CanvasLayer/LabelModifiers");
        SpeedBoostModifierPanel = GetNodeOrNull<Panel>("CanvasLayer/SpeedBoostModifier");
        RepairKitModifierPanel = GetNodeOrNull<Panel>("CanvasLayer/RepairKitModifier");
        SpeedBoostModifierLabel = GetNodeOrNull<Label>("CanvasLayer/SpeedBoostModifier/LabelAmount");
        RepaitKitModifierLabel = GetNodeOrNull<Label>("CanvasLayer/RepairKitModifier/LabelAmount");
		RepaitKitModifierLabel.Text = "0";
        SpeedBoostModifierLabel.Text = "0";
		//for(int i = 0; i < PlayerMenu.playerAmount; i++){
		//	LabelPlayers.Text += "\nPlayer " + PlayerMenu.playerIds[i];
		//}
		
		remainingMinutes--;
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
		if(ExtraTime)
		{
			countdownTimer.WaitTime += 60;
			ExtraTime = false;
		}
		if((int)countdownTimer.TimeLeft != currentTime)
		{
            if (currentSeconds <= 0)
            {
				currentSeconds = 60;
				if(remainingMinutes != 0)
				{
                    remainingMinutes--;
                }
				
            }
            if (remainingMinutes != 0 || currentSeconds != 0)
            {
                currentSeconds--;
            }
			currentTime = (int)countdownTimer.TimeLeft;
			GD.Print((int)countdownTimer.TimeLeft);

        }

		LabelTime.Text = "Time Left: " + remainingMinutes + " : " + currentSeconds;
	}
	
	private void OnCountdownTimerTimeout()
	{
		// Decrement remaining time
		// if (remainingTime > 0)
		// {
		// 	remainingTime--;
		// }
		
		// LabelTime.Text = "Time Left: " + remainingTime.ToString();

		// if (remainingTime <= 0)
		// {
		// 	GD.Print("Time's up!");
		// 	countdownTimer.Stop(); // Stop the timer if needed
		// }
		EmitSignal(SignalName.CountdownTimerTimeout);
	}
}



