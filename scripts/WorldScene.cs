using Godot;
using System;
using System.Xml.Schema;

public partial class WorldScene : Node3D
{
	public int level=0;
	public int objectiveScore = 0; //current held cargo
	int deliveredCargo = 0;
	string cargoString;
	string objectiveString;
	int maxObjectiveAmount = 0;
	public int maxObjectiveStackAmount = 4; //max hold amount
	bool GameOver = false;
	[Export]
	Label cargoTracker;
	[Export]
	Label objectiveTracker;
	[Export]
	StyleBoxFlat deathStyleBox;
	byte styleBoxTransparency = 0;
	bool transperancyReached = false;
	int i = 0;
    public override void _Ready()
    {
		cargoString = cargoTracker.Text;
		objectiveString = objectiveTracker.Text;
		maxObjectiveAmount= GetNode("Goods").GetChildCount();
		objectiveTracker.Text = objectiveString + deliveredCargo + "/" + maxObjectiveAmount;
		cargoTracker.Text = cargoString + objectiveScore+"/"+maxObjectiveStackAmount;
	}
	public override void _Process(double delta)
	{
		if(Input.IsKeyPressed(Key.F4))
		{
			OnWholeDelivery();
		}
		if(!GameOver)
		{
			if (deliveredCargo == maxObjectiveAmount)
			{
				GameOver = true;
				GameOverFunction(true);

            }
        }
        if(GetNode<Panel>("GameCamera/CanvasLayer/GameOverScreen").Visible == true  && !transperancyReached)
		{
			i++;
			
			if(i%2==0)
			{
				styleBoxTransparency++; ;
			}
			deathStyleBox.BgColor = Color.Color8(0, 0, 0, styleBoxTransparency);
			if(styleBoxTransparency ==120)
			{
                
				transperancyReached = true;
                GameOverFunction(false);
				return;
			}
		}
    }
	
    public void OnNoBoatHealth() 
	{
        GetNode<Panel>("GameCamera/CanvasLayer/GameOverScreen").Visible = true;
		GameCamera.ResetSway();
		Modifiers.ResetModifiers();
    }
	public void OnObjectivePickup()
	{
		objectiveScore++;
        cargoTracker.Text = cargoString + objectiveScore + "/" + maxObjectiveStackAmount;
    }
	public void OnWholeDelivery()
	{
		deliveredCargo += objectiveScore;
		GetNode<Boat>("Boat").EmptyCargo();
		objectiveScore = 0;
        cargoTracker.Text = cargoString + objectiveScore + "/" + maxObjectiveStackAmount;
        objectiveTracker.Text = objectiveString + deliveredCargo + "/" + maxObjectiveAmount;
    }
	public void OnDelivery() {
		objectiveScore--;
		deliveredCargo++;
		cargoTracker.Text = cargoString + objectiveScore + "/" + maxObjectiveStackAmount;
        objectiveTracker.Text = objectiveString + deliveredCargo + "/" + maxObjectiveAmount;
	}
	public void OnCountdownTimerTimeout() {

		
		GameOverFunction(false);
	}
	public void GameOverFunction(bool win)
	{
		if(win)
		{
			GetNode<Label>("GameCamera/CanvasLayer/GameOverScreen/Container/Label").Text = "You Win";
		}
        GetNode<Label>("GameCamera/CanvasLayer/GameOverScreen/Container/Label").Text = "GAME OVER";
        GetNode<Button>("GameCamera/CanvasLayer/GameOverScreen/Container/RestartButton").Visible = true;
        GetNode<Button>("GameCamera/CanvasLayer/GameOverScreen/Container/QuitButton").Visible = true;
        GetNode<Button>("GameCamera/CanvasLayer/GameOverScreen/Container/RestartButton").GrabFocus();
		
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
