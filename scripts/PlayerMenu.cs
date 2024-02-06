using Godot;
using System;

public partial class PlayerMenu : MarginContainer
{
	
	/*[Export]
	public PackedScene levelSelectorMenu;*/
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}
   
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		
       
    }
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_cancel"))
        {
            GetParent<LevelManager>().loadMainMenu();
        }
    }
}
