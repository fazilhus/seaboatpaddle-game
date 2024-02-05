using Godot;
using System;

public partial class MenuManager : MarginContainer
{
    public void onExitButtonPressed()
    {

        GetTree().Quit();
    }
}
