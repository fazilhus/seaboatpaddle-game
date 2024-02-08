using Godot;
using System;
using System.Linq;

public partial class PlayerMenu : MarginContainer
{

   
    // Called when the node enters the scene tree for the first time.

    public int playerAmount=0;
    public int[] playerIds=new int[4];
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
            GD.Print(playerIds[playerAmount]);
            GetNode("VerticalContainer/HorizontalContainer").GetChild(playerAmount).Set("visible", true);
            playerAmount++;

             GD.Print(playerId);
            GD.Print(playerAmount);

           
           
        }
        
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
            
            GetParent<LevelManager>().loadLevelSelector();
        }
        
    }
}
