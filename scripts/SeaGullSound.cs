using Godot;
using System;

public partial class SeaGullSound : AudioStreamPlayer3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	private void OnSeaGullSoundFinished()
	{
		// Stop playing the sound
		Play();
		//GetNode<Timer>("SeaGullPauser").Start(2);
	}

	// Function to restart the sound after the delay
	private void OnTimerTimeout()
	{
		// Restart playing the sound
		Play();
	}
}



