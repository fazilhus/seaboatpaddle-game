using Godot;
using System;

public partial class Healthbar : Control
{
    ProgressBar progressBar;
    public override void _Ready()
    {
        progressBar = GetNode<ProgressBar>("ProgressBar");
    }

    public void Sethealth(int health)
    {
        progressBar.Value = health;
    }

}
