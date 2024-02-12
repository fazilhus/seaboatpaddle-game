using Godot;
using System;

public partial class PlayerController : Node3D
{
    public int[] playerIds = new int[4];
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void AddPlayers(int[] players)
	{
		playerIds[0] = players[0];
		playerIds[1] = players[1];
		playerIds[2]= players[2];
		playerIds[3]= players[3];
	}
}
