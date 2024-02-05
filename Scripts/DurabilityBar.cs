using Godot;
using System;

public partial class DurabilityBar : ProgressBar
{
	
	[Export] private Timer timer;
	[Export] private int Durability;
	private  ProgressBar damageBar {get; set; }
	private ProgressBar durabilityBar {get; set; }
	// Called when the node enters the scene tree for the first time.


	public override void _Ready()
	{
		durabilityBar = GetNode<ProgressBar>("DurabilityBar");
		damageBar = GetNode<ProgressBar>("DamageBar");
		timer = GetNode<Timer>("Timer");

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void setHealth(int newDurability)
	{
		int previousDurability = newDurability;
		Durability = Math.Min((int)MaxValue, newDurability);
		Value = Durability;
			if( Durability <= 0)
		{
			Durability = 0;
		}
		if(Durability < previousDurability)
		{
			timer.Start();
		}
		else
		{
			durabilityBar.Value = Durability;
		}
	}
	public void InitHealthDurability(int healthAmount)
	{
		durabilityBar.MaxValue = healthAmount;
		durabilityBar.Value = healthAmount;
		
		damageBar.MaxValue = healthAmount;
		damageBar.Value = healthAmount;
	}
	public void OnTimerTimeout()
	{
		damageBar.Value = Durability;
	}
}
