using Godot;
using System;

public partial class LevelManager : Node3D
{
	[Export]
	public PackedScene mainMenu;
    [Export]
    public PackedScene playerMenu;
	[Export]
	public PackedScene levelSelectorMenu;
	[Export]
	public PackedScene[] levels;
    
	// Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		loadMainMenu();
		
	}
	public void loadMainMenu()
	{
		var children = GetChildren();
		foreach (var child in children)
		{
			RemoveChild(child);
		}
        var instance = mainMenu.Instantiate();
        MainMenu menu = (MainMenu)instance;

        AddChild(menu);
    }
	public void loadPlayerMenu()
	{
        var children = GetChildren();
        foreach (var child in children)
        {
            RemoveChild(child);
        }
        var instance = playerMenu.Instantiate();
        PlayerMenu menu = (PlayerMenu)instance;

        AddChild(menu);
    }
	public void loadLevelSelector()
	{
        var children = GetChildren();
        foreach (var child in children)
        {
            RemoveChild(child);
        }
		var instance = levelSelectorMenu.Instantiate();
		LevelSelectorMenu menu = (LevelSelectorMenu)instance;
		AddChild(menu);
    }
	public void loadLevel(int levelId)
	{
        var children = GetChildren();
        foreach (var child in children)
        {
            RemoveChild(child);
        }
        var instance = levels[levelId-1].Instantiate();
        Node3D level = (Node3D)instance;
        AddChild(level);
    }
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
