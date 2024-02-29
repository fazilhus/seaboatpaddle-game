using Godot;
using System;

public partial class GameCamera : Node3D
{
	[Signal]
	public delegate void CountdownTimerTimeoutEventHandler();
	
	[Export]
	public Timer countdownTimer;

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
	public static bool ExtraTime { get; private set; } = false;
	
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
			var time_left = countdownTimer.TimeLeft;
			countdownTimer.Stop();
			countdownTimer.WaitTime = time_left + 60;
			countdownTimer.Start();
			ExtraTime = false;
		}
		TimeSpan time = TimeSpan.FromSeconds(countdownTimer.TimeLeft);
		string timeString = time.ToString(@"mm\:ss");

		LabelTime.Text = timeString;
	}
	
	private void OnCountdownTimerTimeout()
	{
		
		EmitSignal(SignalName.CountdownTimerTimeout);
	}
}



