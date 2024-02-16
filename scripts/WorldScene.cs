using Godot;
using System;

public partial class WorldScene : Node3D
{
	public void OnNoBoatHealth() {
		GetParent<LevelManager>().loadMainMenu();
	}

	public void OnCountdownTimerTimeout() {
		GetParent<LevelManager>().loadMainMenu();
	}
}
