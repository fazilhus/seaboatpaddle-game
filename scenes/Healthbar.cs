using Godot;
using System;

public partial class Healthbar : Control
{
	// Called when the node enters the scene tree for the first time.
	ProgressBar healthBar;

	public override void _Ready()
	{
		healthBar = GetNode<ProgressBar>("ProgressBar");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetHealth(int health)
	{
		healthBar.Value = health;
	}
}
