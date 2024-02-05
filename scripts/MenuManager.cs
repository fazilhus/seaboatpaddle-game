using Godot;
using System;

public partial class MenuManager : MarginContainer
{
    public override void _Ready()
    {
        GetNode<Button>("HorizontalContainer/MenuContainer/Select").GrabFocus();
    }
    
    public void onExitButtonPressed()
    {

        GetTree().Quit();
    }
}
