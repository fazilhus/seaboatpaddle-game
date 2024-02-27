using Godot;
using System;
using System.Xml.Schema;

public partial class WorldScene : Node3D
{
	public int level=0;
	public int objectiveScore = 0;
	string objectiveString;
	int maxObjectiveStackAmount = 4;
	[Export]
	Label objectiveTracker;

    public override void _Ready()
    {
		objectiveString = objectiveTracker.Text;
		
		objectiveTracker.Text = objectiveString + objectiveScore+"/"+maxObjectiveStackAmount;
    }
    public void OnNoBoatHealth() {

		GameOverFunction();
	}
	public void OnObjectivePickup()
	{
		objectiveScore++;
        objectiveTracker.Text = objectiveString + objectiveScore + "/" + maxObjectiveStackAmount;
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
