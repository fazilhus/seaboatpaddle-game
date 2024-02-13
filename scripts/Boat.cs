using Godot;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

enum PaddleSide {
	Left = 0,
	Right = 1,
}

public partial class Boat : RigidBody3D
{
    [Signal]
    public delegate void NoBoatHealthEventHandler();

    [Export]
    public Godot.Collections.Array<Node3D> paddles;

	private List<Vector3> _player_inputs;
	private List<Vector3> _paddles_rotation_old;

	[Export]
	public float sideways_force_ratio = 0.5f;
	[Export]
	public float forward_force_ratio = 50;
	
	//BoatPhysicsVariables

	[Export] private float floatForce = 1.0f;
	[Export] private float waterDrag = 0.005f;
	[Export] private float WaterAngularDrag = 0.01f;
	[Export] private bool isSubmerged = false;
	private float gravity;

	private float initialY;
	private double elapsedTime = 0;
	
	public bool SpeedBoost = true;
	public bool ControlInversion = false;
	public bool RepairKit = false;
	
	//[Export]
	//private float bobbingFactor = 0.1f;
	//[Export]
	//private float bobbingSpeed = 2.0f;

	[Export] public WaterPlane water;
	
	public double time = 0;

	public Godot.Collections.Array<Node> probeContainer;
	
	[Export] Survivors survivors;
	[Export] private bool isVortexCollided;
    [Export] private float maxDistance = 20.0f; // Example maximum distance from the center

	 // Define the vortex center
    private Vector3 vortexCenter = new Vector3(10, 5, 0); // Example vortex center position

    // Define the base force magnitude
    [Export]
	public float baseForceMagnitude = 100.0f; // Example base force magnitude
	[Export]
	public float rotationalForceMagnitude = 10.0f;
	[Export]
	public float rotationalVelocity = 5;

	private float strengthFactor; 
    private HealthComponent healthComp;
    
    //[Export] Survivors survivors;

	public override void _Ready()
	{
		//instantiate variables for boat physics
		var parent = GetParent();
		gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
		//water = parent.GetNode<WaterPlane>("WaterPlane");
		probeContainer = GetNode<Node3D>("ProbeContainer").GetChildren();
        //survivors = parent.GetNode<Survivors>("Survivors");
        
	
		initialY = GlobalPosition.Y;

        _player_inputs = new List<Vector3>();
        _paddles_rotation_old = new List<Vector3>();
        foreach (int device_id in Input.GetConnectedJoypads()) {
            _player_inputs.Add(Vector3.Zero);
            _paddles_rotation_old.Add(Vector3.Zero);
        }

        healthComp = GetNode<HealthComponent>("HealthComponent");
    }

    public override void _Process(double delta)
    {
        if (Input.IsKeyPressed(Key.F2)) {
            GetNode<HealthComponent>("HealthComponent").SubtractHealth(100);
        }
        
        DebugDraw2D.SetText("Health: ", GetNode<HealthComponent>("HealthComponent").health);
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 forward = Basis.Z;
        foreach (var it in paddles.Select((paddle, i) => new {Paddle = paddle, Index = i})) {
            if (it.Index >= _player_inputs.Count) {
                continue;
            }
            
            Vector3 input = GetPlayerInput(it.Index);
            //Vector3 input = _player_inputs[it.Index];

			_paddles_rotation_old[it.Index] = it.Paddle.Rotation;
			it.Paddle.Rotation = new Vector3(input.X * 0.8f, 0, input.Z * 0.5f);
			Vector3 angular_velocity = (it.Paddle.Rotation - _paddles_rotation_old[it.Index]) / (float)delta;
			Vector3 force = new Vector3(angular_velocity.Z, 0, -angular_velocity.X);

			Node3D force_point = it.Paddle.GetNode<Node3D>("ForcePoint");
			if (isSubmerged && force_point.GlobalPosition.Y < GlobalPosition.Y) {
				ApplyForce(-sideways_force_ratio * force, it.Paddle.Position);
				ApplyCentralForce(-forward_force_ratio * Curve(force) * force.Sign().Z * forward);
			}
		}              
		
		// checking marker3d in the probe container for simulating the water physics
		isSubmerged = false;
		foreach(Marker3D p in probeContainer)
		{
			float depth = (float)water.getHeight(GlobalPosition) - p.GlobalPosition.Y;

			if(depth > 0)
			{
				isSubmerged = true;

				if(isVortexCollided)
				{
					// Calculate the position of the boat relative to the center of the whirlpool
					//DebugDraw3D.DrawSphere(vortexCenter, 1, Colors.Green);
					Vector3 relativePosition = -Position + vortexCenter;
					relativePosition.Y = 0;

					// Calculate the distance from the boat to the center of the whirlpool
					float distanceToCenter = relativePosition.Length();

					// Define the base force toward the center of the whirlpool
					Vector3 baseForce = relativePosition.Normalized() * baseForceMagnitude;

					// Adjust the force based on the distance from the center
					float strengthFactor = (maxDistance - distanceToCenter) / maxDistance;
					//DebugDraw2D.SetText("StrengthFactor", strengthFactor);
					//GD.Print("StrengthFactor", strengthFactor);
					Vector3 whirlpoolForce = baseForce * strengthFactor;

					// Apply the whirlpool force to the boat
					//DebugDraw3D.DrawLine(Position, Position + relativePosition, Colors.Red);
					ApplyCentralForce(whirlpoolForce * (float)delta);

					// Calculate the rotational force to simulate rotation around the whirlpool
					Vector3 tangentialVelocity = relativePosition.Rotated(Vector3.Left, Mathf.DegToRad(-90)) * rotationalForceMagnitude * (float)delta;
					//Vector3 tangentialVelocity = AngularVelocity.Cross(relativePosition.Normalized());
					//Vector3 rotationalForce = relativePosition.Normalized().Cross(tangentialVelocity) * rotationalForceMagnitude * (float)delta;

					// Apply the rotational force to the boat
					//ApplyCentralForce(tangentialVelocity * (float)delta);
					var v = Vector3.Down.Cross(-relativePosition) * rotationalVelocity * (float)delta;
					
					//GD.Print(v);
					//DebugDraw3D.DrawLine(Position, Position + v, Colors.Yellow);
					ApplyCentralForce(v);
					//DebugDraw3D.DrawLine(Position, Position + tangentialVelocity.Normalized(), Colors.Green);
					ApplyTorque(tangentialVelocity);
					depth += -2.5f * strengthFactor;
					//GD.Print("depth: ",depth);
					//GD.Print("strengthF: ",strengthFactor);
				} 
				ApplyForce(Vector3.Up * floatForce * gravity * depth, p.GlobalPosition - GlobalPosition);
			}
			else {
                isSubmerged = false;
            }
		}
		
	}
	
	public override void _UnhandledInput(InputEvent @event)
	{
		if(@event.IsActionPressed("ui_accept"))
		{
			if (SpeedBoost)
			{
				ApplySpeedBoost(7000, 2);
				SpeedBoost = true;
			}
			if (RepairKit)
			{
				RepairBoat(5);
				RepairKit = false;
			}
		}
	}
	
	public override void _IntegrateForces(PhysicsDirectBodyState3D state) // changing the simulation state of the object
	{
		if(isSubmerged)
		{
			state.LinearVelocity *= 1 - waterDrag;
			state.AngularVelocity *= 1 - WaterAngularDrag;
		}

		if (state.LinearVelocity.Length() >= 8) {
			state.LinearVelocity = 8 * state.LinearVelocity.Normalized();
		}
	}

	private float Curve(Vector3 v) {
		return Mathf.Pow(v.Length() / 10, 2);
	}

	public void OnBoatArea3dBodyExited(Area3D area)
	{
		if(area.IsInGroup("Vortex"))
		{
			isVortexCollided = false;
		}

        if (area.IsInGroup("VortexDamage")) {
            GetNode<Timer>("VortexDamageTimer").Stop();
        }
		{
			  GD.Print("boat is colliding with modifiers!");
		}
	}

    private Vector3 GetPlayerInput(int device_id) {
        Vector3 input = Vector3.Zero;
        switch (device_id) {
            case 0: {
                input.Z = Input.GetAxis("right_p1", "left_p1");
                input.X = -Input.GetAxis("backward_p1", "forward_p1");
                break;
            }
            case 1: {
                input.Z = Input.GetAxis("right_p2", "left_p2");
                input.X = -Input.GetAxis("backward_p2", "forward_p2");
                break;
            }
        }
        return input;
    }
    public void OnArea3dTriggerBoatAreaEntered(Area3D area)
    {
        if (area.IsInGroup("Survivors"))
        {
              GD.Print("boat is colliding with survivors!");
        }
        
        if (area.IsInGroup("SeaMine")) {
            GD.Print("Boom!!!");
            healthComp.SubtractHealth(100);
        }

        if(area.IsInGroup("Modifiers"))

		if (area.IsInGroup("Vortex"))
		{
			isVortexCollided = true;
			vortexCenter = area.GlobalPosition;
			GD.Print("Collided with vortex!", area.GlobalPosition);
		
		}

        if (area.IsInGroup("VortexDamage")) {
            GetNode<Timer>("VortexDamageTimer").Start();
        }
    }

    public void OnHealthComponentNoHealthEvent() {
        GD.Print("Boat lost all durability: You Lose");
        EmitSignal(SignalName.NoBoatHealth);
    }

    public void OnBodyEntered(Node node) {
        GD.Print(node.Name);
        if (node.IsInGroup("Rock")) {
            GD.Print("Crashed a rock!!!");
            var speed = LinearVelocity.Length();
            if (speed < 5) {
                return;
            }
            GD.Print("Lost ", 3 * (int)speed, " health");
            healthComp.SubtractHealth(3 * (int)speed);
        }
    }

    public void OnVortexDamageTimerTimeout() {
        healthComp.SubtractHealth(10);
    }
}
