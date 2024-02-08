using Godot;
using System;

public partial class LevelSelectorMenu : MarginContainer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        GetNode<Button>("LevelContainer/Level/Level1Button").GrabFocus();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void onLevel1ButtonPressed()
	{
        GetParent<LevelManager>().loadLevel(1);
    }
	public override void _UnhandledInput(InputEvent @event)
	{
        if (@event.IsActionPressed("ui_cancel"))
        {
            GetParent<LevelManager>().loadPlayerMenu();
        }
    }
}