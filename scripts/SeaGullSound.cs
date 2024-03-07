using Godot;
using System;

public partial class SeaGullSound : AudioStreamPlayer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Start playing the sound
		Play();
	}

	// Function to handle sound restart
	private void OnSeaGullSoundFinished()
	{
		// Stop playing the sound
		Stop();
		GetNode<Timer>("SeaGullPauser").Start(12);
	}

	// Function to restart the sound after the delay
	private void OnTimerTimeout()
	{
		// Restart playing the sound
		Play();
	}
}
