using Godot;
using System;
using System.Linq;

public partial class PlayerMenu : MarginContainer
{

   
	// Called when the node enters the scene tree for the first time.

	public  int playerAmount=0;
	public  int[] playerIds=new int[2];
	public Color[] playerColors=new Color[2];
	[Export]
	public OptionButton[] colorPickers;

	[Export]
	Color red = Color.FromHtml("eb0033");
	[Export]
	Color blue = Color.FromHtml("0033ff");
	[Export]
	Color green = Color.FromHtml("00ad72");

	public override void _Ready()
	{
		
	}
   
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	   
	}
	public void Addplayer(int playerId)
	{
		bool playerAlreadyIn = false;
		GD.Print(playerIds.Length);
	   
		for(int i = 0; i < playerAmount; i++) 
		{
			if (playerIds[i] == playerId)
			{
				playerAlreadyIn = true;
			}
			if(playerAmount==0)
			{
				playerAlreadyIn = false;
			}
		}
		if (!playerAlreadyIn)
		{

			playerIds[playerAmount] = playerId;
			//GD.Print(playerIds[playerAmount]);
			GetNode("VerticalContainer/HorizontalContainer").GetChild(playerAmount).Set("visible", true);
			/*if(playerAmount==1)
			{
				GetNode<OptionButton>("VerticalContainer/HorizontalContainer/Player1/Colors").GrabFocus();
			}*/
			if (playerAmount == 0 && playerId == 0)
			{
				GetNode<Label>("VerticalContainer/HorizontalContainer/Player1/Label").AddThemeColorOverride("font_color", red);
			}
			else if(playerAmount == 1 && playerId ==0)
			{
				GetNode<Label>("VerticalContainer/HorizontalContainer/Player2/Label").AddThemeColorOverride("font_color", red);

			}
			else if (playerAmount == 0 && playerId == 1)
			{
				GetNode<Label>("VerticalContainer/HorizontalContainer/Player1/Label").AddThemeColorOverride("font_color", blue);
			}
			else
			{
				GetNode<Label>("VerticalContainer/HorizontalContainer/Player2/Label").AddThemeColorOverride("font_color", blue);

			}
			playerAmount++;
			
			
			//GD.Print(playerId);
			//GD.Print(playerAmount);

			string playerAmountLabel = "";
			if (playerAmount != 2) {
				playerAmountLabel = playerAmount + " / 2 players";
			}
			else {
				playerAmountLabel = "Press 'Start' to play";
			}
			GetNode<Label>("VerticalContainer/ContinueNotice").Text = playerAmountLabel;
		   
		}
		
	}
	public void OnColor1Select(int index)
	{
		GD.Print(index);
	
		if (index == 0)
		{
			playerColors[0] = red;
			colorPickers[1].SetItemDisabled(1, false);
			colorPickers[1].SetItemDisabled(2, false);
		}
		else if (index == 1)
		{
			playerColors[0] = green;
			colorPickers[1].SetItemDisabled(0, false);
			colorPickers[1].SetItemDisabled(2, false);
		}
		else if (index == 2)
		{
			playerColors[0] = blue;
			colorPickers[1].SetItemDisabled(0, false);
			colorPickers[1].SetItemDisabled(1, false);
		}
		colorPickers[1].SetItemDisabled(index, true);
		GD.Print(playerColors[0]);
	}
	public void OnColor2Select(int index)
	{
		if (index == 0)
		{
			playerColors[1] = red;
			colorPickers[0].SetItemDisabled(1, false);
			colorPickers[0].SetItemDisabled(2, false);
			
		}
		else if (index == 1)
		{
			colorPickers[0].SetItemDisabled(0, false);
			colorPickers[0].SetItemDisabled(2, false);
			playerColors[1] = green;
		}
		else if(index == 2)
		{
			colorPickers[0].SetItemDisabled(0, false);
			colorPickers[0].SetItemDisabled(1, false);
			playerColors[1] = blue;
		}
		colorPickers[0].SetItemDisabled(index, true);
		GD.Print(playerColors[1]);
	}
	public override void _UnhandledInput(InputEvent @event)
	{
	   
		if(@event.IsActionPressed("ui_accept"))
		{
			int playerId = @event.Device;
			Addplayer(playerId);
			

		}
		if (@event.IsActionPressed("ui_cancel"))
		{
			GetParent<LevelManager>().loadMainMenu();
		}
		if(@event.IsActionPressed("ui_continue")) //Need to add this to inputmap if not there already
		{
			if (playerAmount == 2) {

				transferDataToPlayerManager();
				GetParent<LevelManager>().loadLevel(1);
			}
			
		}
	   if(Input.IsKeyPressed(Key.F1)) 
		{
			transferDataToPlayerManager();
			GetParent<LevelManager>().loadLevel(1);
		} 
	}
	void transferDataToPlayerManager()
	{
		//PlayerManager.instance.playerColors[0]= playerColors[0];
		//PlayerManager.instance.playerColors[1] = playerColors[1];
		PlayerManager.instance.playerColors[0] = red;
		PlayerManager.instance.playerColors[1] = blue;
	}
}
