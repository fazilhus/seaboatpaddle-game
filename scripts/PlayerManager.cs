using Godot;
using System;

public partial class PlayerManager : Node3D
{


	public static PlayerManager instance;




	public int playerAmount = 0;
	public int[] playerIds = new int[2];
	public Color[] playerColors = new Color[2];
	[Export]
	public OptionButton[] colorPickers;

	[Export]
	Color red = Color.FromHtml("eb0033");
	[Export]
	Color blue = Color.FromHtml("0033ff");
	[Export]
	Color green = Color.FromHtml("00ad72");
	// Called when the node enters the scene tree for the first time.


	public override void _EnterTree()
	{
		instance = this;
	}


}
