using Godot;
using System;

public partial class HealthComponent : Node3D
{
    [Signal]
    public delegate void NoHealthEventHandler();

    [Export]
    public int max_health = 100;
    public int health {get;set;}

    public override void _Ready()
    {
        health = max_health;
    }

    public void AddHealth(int n) {
        if (health + n > max_health) {
            health = max_health;
            return;
        }
        health += n;
    }

    public void SubtractHealth(int n) {
        if (health - n <= 0) {
            EmitSignal(SignalName.NoHealth);
            health = 0;
            return;
        }
        health -= n;
    }
}
