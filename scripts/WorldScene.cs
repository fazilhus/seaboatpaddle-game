using Godot;
using System;
using System.Xml.Schema;

public partial class WorldScene : Node3D
{
	public int level=0;
	public void OnNoBoatHealth() {

		GameOverFunction();
	}

	public void OnCountdownTimerTimeout() {


		GameOverFunction();
	}
	public void GameOverFunction()
	{
		GetNode<Panel>("GameCamera/CanvasLayer/GameOverScreen").Visible = true;
		GetNode<Button>("GameCamera/CanvasLayer/GameOverScreen/RestartButton").GrabFocus();
		GD.Print(level);
		GetTree().Paused = true;
		
	}
	public void QuitButtonPressed()
	{
		GetTree().Paused = false;
		GetParent<LevelManager>().loadMainMenu();
    }
	public void RetryButtonPressed()
	{
		GetTree().Paused = false;
		GetParent<LevelManager>().loadLevel(level);
    }
}
