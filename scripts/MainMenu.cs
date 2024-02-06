using Godot;
using System;

public partial class MainMenu : MarginContainer
{
    

    public override void _Ready()
    {
        GetNode<Button>("HorizontalContainer/MenuContainer/Start").GrabFocus(); 
    }
    public void onStartButtonPressed()
    {

        GetParent<LevelManager>().loadPlayerMenu();
    }
    public void onExitButtonPressed()
    {

        GetTree().Quit();
    }
}
