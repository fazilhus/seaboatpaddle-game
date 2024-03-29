using Godot;
using System;

public partial class HealthComponent : Node3D
{
    [Signal]
    public delegate void NoHealthEventHandler();
    [Signal]
    public delegate void DamageTakenEventHandler();

    Healthbar healthBar;

    [Export]
    public int max_health = 100;
    public int health { get; set; }

    public override void _Ready()
    {
        health = max_health;
        healthBar = GetTree().Root.GetNode<Healthbar>("Main/LevelController/Level/GameCamera/CanvasLayer/HealthBarControl");
        healthBar.Sethealth(health);
    }

    public void AddHealth(int n)
    {
        if (health + n > max_health)
        {
            health = max_health;
            healthBar.Sethealth(health);
            return;
        }
        health += n;
        healthBar.Sethealth(health);
    }

    public void SubtractHealth(int n, string cause)
    {
        if (health - n <= 0)
        {
            EmitSignal(SignalName.NoHealth, cause);
            health = 0;
            healthBar.Sethealth(health);
            return;
        }
        EmitSignal(SignalName.DamageTaken);
        health -= n;
        healthBar.Sethealth(health);
    }
}
