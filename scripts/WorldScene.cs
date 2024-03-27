using Godot;
using System;
using System.Xml.Schema;

public partial class WorldScene : Node3D
{
    public int level = 0;
    public int objectiveScore = 0; //current held cargo
    int deliveredCargo = 0;
    string cargoString;
    string objectiveString;
    int maxObjectiveAmount = 0;
    public int maxObjectiveStackAmount = 4; //max hold amount
    bool GameOver = false;
    [Export]
    Label cargoTracker;
    [Export]
    Label objectiveTracker;
    [Export]
    StyleBoxFlat deathStyleBox;
    [Export]
    GpuParticles3D boatDeathParticle;
    byte styleBoxTransparency = 0;
    bool transperancyReached = false;
    int transperancyTimer = 0;

    int damageEffectTimer = 0;
    public override void _Ready()
    {
        boatDeathParticle = GetNode<Boat>("Boat").GetNode<Node3D>("ParticleEffectDeath").GetChild<GpuParticles3D>(0);
        cargoString = cargoTracker.Text;
        objectiveString = objectiveTracker.Text;
        maxObjectiveAmount = GetNode("Goods").GetChildCount();
        objectiveTracker.Text = objectiveString + deliveredCargo + "/" + maxObjectiveAmount;
        cargoTracker.Text = cargoString + objectiveScore + "/" + maxObjectiveStackAmount;
    }
    public override void _Process(double delta)
    {
        if (Input.IsKeyPressed(Key.F4))
        {
            OnWholeDelivery();
        }
        if (!GameOver)
        {
            if (deliveredCargo == maxObjectiveAmount)
            {
                GameOver = true;
                GameOverFunction(true);

            }
        }
        if (GetNode<TextureRect>("GameCamera/CanvasLayer/DamageFlash").Visible == true)
        {
            damageEffectTimer++;
            if (damageEffectTimer == 20)
            {
                damageEffectTimer = 0;
                GetNode<TextureRect>("GameCamera/CanvasLayer/DamageFlash").Visible = false;

            }
        }
        if (GetNode<Panel>("GameCamera/CanvasLayer/GameOverScreen").Visible == true && !transperancyReached)
        {
            transperancyTimer++;

            if (transperancyTimer % 2 == 0)
            {
                styleBoxTransparency++; ;
            }
            deathStyleBox.BgColor = Color.Color8(0, 0, 0, styleBoxTransparency);
            if (styleBoxTransparency == 120)
            {

                transperancyReached = true;
                GameOverFunction(false);
                return;
            }
        }
    }
    public void OnDamageEffect()
    {
        GD.Print("Damage Taken");
        GetNode<TextureRect>("GameCamera/CanvasLayer/DamageFlash").Visible = true;
    }
    public void OnNoBoatHealth(string cause)
    {
        GetNode<Panel>("GameCamera/CanvasLayer/GameOverScreen").Visible = true;
        string deathText = GetNode<Label>("GameCamera/CanvasLayer/GameOverScreen/Container/DeathExplenation").Text;
        GetNode<Label>("GameCamera/CanvasLayer/GameOverScreen/Container/DeathExplenation").Text = deathText + " " + cause;
        boatDeathParticle.Emitting = true;
        GetTree().Paused = true;
        GameCamera.ResetSway();
        Modifiers.ResetModifiers();
    }
    public void OnObjectivePickup(int stack_size)
    {
        objectiveScore = stack_size;
        cargoTracker.Text = cargoString + objectiveScore + "/" + maxObjectiveStackAmount;
    }
    public void OnWholeDelivery()
    {
        deliveredCargo += objectiveScore;
        GetNode<Boat>("Boat").EmptyCargo();
        objectiveScore = 0;
        cargoTracker.Text = cargoString + objectiveScore + "/" + maxObjectiveStackAmount;
        objectiveTracker.Text = objectiveString + deliveredCargo + "/" + maxObjectiveAmount;
    }
    public void OnDelivery()
    {
        objectiveScore--;
        deliveredCargo++;
        cargoTracker.Text = cargoString + objectiveScore + "/" + maxObjectiveStackAmount;
        objectiveTracker.Text = objectiveString + deliveredCargo + "/" + maxObjectiveAmount;
    }
    public void OnCountdownTimerTimeout()
    {


        GameOverFunction(false);
    }
    public void GameOverFunction(bool win)
    {
        if (win)
        {
            GetNode<Label>("GameCamera/CanvasLayer/GameOverScreen/Container/Label").Text = "You Win";
        }
        GetNode<Label>("GameCamera/CanvasLayer/GameOverScreen/Container/Label").Text = "GAME OVER";
        GetNode<Button>("GameCamera/CanvasLayer/GameOverScreen/Container/RestartButton").Visible = true;
        GetNode<Button>("GameCamera/CanvasLayer/GameOverScreen/Container/QuitButton").Visible = true;
        GetNode<Button>("GameCamera/CanvasLayer/GameOverScreen/Container/RestartButton").GrabFocus();



    }
    public void QuitButtonPressed()
    {
        GetTree().Paused = false;
        GetParent<LevelManager>().loadMainMenu();
    }
    public void RetryButtonPressed()
    {
        GetTree().Paused = false;
        GetParent<LevelManager>().loadLevel(level);
    }
    public void ResumeButtonPressed()
    {
        GetTree().Paused = false;
        GetNode<Panel>("GameCamera/CanvasLayer/Pausescreen").Visible = false;
        
    }
    public void Pause()
    {
        GetTree().Paused = true;
        
        GetNode<Panel>("GameCamera/CanvasLayer/Pausescreen").Visible = true;
        GetNode<Button>("GameCamera/CanvasLayer/Pausescreen/Container/ResumeButton").GrabFocus();
       
    }
    public override void _UnhandledInput(InputEvent @event)
    {
        if(@event.IsActionPressed("ui_continue"))
        {
            Pause();
        }
    }
}

