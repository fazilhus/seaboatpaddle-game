using Godot;
using System;

public partial class MenuManager : MarginContainer
{
    public override void _Ready()
    {
        GetNode<Button>("HorizontalContainer/MenuContainer/Select").GrabFocus(); 
    }
    public void onSelectButtonPressed()
    {
        GetTree().ChangeSceneToFile("res://scenes/main.tscn"); //Current implementation only.
        //Should be changed to hopefully load scene and disable Current menu to make it easier for managers to keep data
        // We probably wouldnt want to change 
    }
    public void onExitButtonPressed()
    {

        GetTree().Quit();
    }
}
