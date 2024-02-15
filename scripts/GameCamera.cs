using Godot;
using System;

public partial class GameCamera : Node3D
{
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
	
	public static void ActivateDrunkenCaptain()
	{
		DrunkenCaptain = true;
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
}
