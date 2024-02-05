using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

enum PaddleSide {
    Left = 0,
    Right = 1,
}

public partial class Boat : RigidBody3D
{
    [Export]
    public Godot.Collections.Array<Node3D> paddles;

    private List<Godot.Vector3> _player_inputs;
    private Node3D _left_paddle;
    private Node3D _right_paddle;
    private Node3D _left_force_point;
    private Node3D _right_force_point;
    private Node3D _pivot;

    //private PaddleSide _paddle_side;

    private List<Godot.Vector3> _paddles_rotation_old;
    private Godot.Vector3 _left_paddle_rotation_old;
    private Godot.Vector3 _left_paddle_angular_velocity;
    private Godot.Vector3 _right_paddle_rotation_old;
    private Godot.Vector3 _right_paddle_angular_velocity;

    [Export]
    public float sideways_force_ratio = 0.5f;
    [Export]
    public float forward_force_ratio = 50;
    
    //BoatPhysicsVariables

	[Export] private float floatForce = 10.0f;
	[Export] private float waterDrag = 0.01f;
	[Export] private float WaterAngularDrag = 0.02f;
	[Export] private bool isSubmerged = false;
	private float gravity;

	private float initialY;
	private double elapsedTime = 0;

	//[Export]
	//private float bobbingFactor = 0.1f;
	//[Export]
	//private float bobbingSpeed = 2.0f;

	[Export] public WaterPlane water;
	
	public double time = 0;

	public Godot.Collections.Array<Node> probeContainer;

    public override void _Ready()
    {
        _left_paddle = GetNode<Node3D>("LeftPaddlePivot");
        _right_paddle = GetNode<Node3D>("RightPaddlePivot");
        _left_force_point = GetNode<Node3D>("LeftPaddlePivot/ForcePoint");
        _right_force_point = GetNode<Node3D>("RightPaddlePivot/ForcePoint");
        _pivot = GetNode<Node3D>("Pivot");
        //_paddle_side = PaddleSide.Left;

        //instantiate variables for boat physics
        gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
		water = GetNode<WaterPlane>("/root/Main/WaterPlane");
		probeContainer = GetNode<Node3D>("ProbeContainer").GetChildren();
	
		initialY = GlobalPosition.Y;

        _player_inputs = new List<Godot.Vector3>();
        _paddles_rotation_old = new List<Godot.Vector3>();
        foreach (int device_id in Input.GetConnectedJoypads()) {
            _player_inputs.Add(Godot.Vector3.Zero);
            _paddles_rotation_old.Add(Godot.Vector3.Zero);
        }
        //GD.Print("Player inputs", _player_inputs.Count);
    }

    public override void _Process(double delta)
    {
        // if (Input.IsActionJustPressed("switch_side")) {
        //     if (_paddle_side == PaddleSide.Left) {
        //         _paddle_side = PaddleSide.Right;
        //     }
        //     else {
        //         _paddle_side = PaddleSide.Left;
        //     }
        // }
    }

    public override void _PhysicsProcess(double delta)
    {
        Godot.Vector3 forward = Basis.Z;
        foreach (var it in paddles.Select((paddle, i) => new {Paddle = paddle, Index = i})) {
            if (it.Index >= _player_inputs.Count) {
                continue;
            }
            
            Godot.Vector3 input = Godot.Vector3.Zero;
            switch (it.Index) {
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

            _paddles_rotation_old[it.Index] = it.Paddle.Rotation;
            it.Paddle.Rotation = new Godot.Vector3(input.X * 0.8f, 0, input.Z * 0.5f);
            Godot.Vector3 angular_velocity = (it.Paddle.Rotation - _paddles_rotation_old[it.Index]) / (float)delta;

            Godot.Vector3 force = new Godot.Vector3(angular_velocity.Z, 0, -angular_velocity.X);
            Node3D force_point = it.Paddle.GetNode<Node3D>("ForcePoint");
            if (force_point.GlobalPosition.Y < GlobalPosition.Y) {
                ApplyForce(-sideways_force_ratio * force, it.Paddle.Position);
                ApplyCentralForce(-forward_force_ratio * Curve(force) * force.Sign().Z * forward);
            }
        }

        // Godot.Vector3 input = Godot.Vector3.Zero;
        // input.Z = -Input.GetActionStrength("right") + Input.GetActionStrength("left");
        // input.X = Input.GetActionStrength("backward") - Input.GetActionStrength("forward");

        // _left_paddle_rotation_old = _left_paddle.Rotation;
        // _left_paddle.Rotation = new Godot.Vector3(input.X * 0.8f, 0, input.Z * 0.5f);
        // _left_paddle_angular_velocity = (_left_paddle.Rotation - _left_paddle_rotation_old) / (float)delta;

        // Godot.Vector3 force = new Godot.Vector3(_left_paddle_angular_velocity.Z, 0, -_left_paddle_angular_velocity.X);
        // if (_left_force_point.GlobalPosition.Y < GlobalPosition.Y) {
        //     ApplyForce(-sideways_force_ratio * force, _left_paddle.Position);
        //     ApplyCentralForce(-forward_force_ratio * Curve(force) * force.Sign().Z * forward);
        // }                

        // _right_paddle_rotation_old = _right_paddle.Rotation;
        // _right_paddle.Rotation = new Godot.Vector3(input.X * 0.8f, 0, input.Z * 0.5f);
        // _right_paddle_angular_velocity = (_right_paddle.Rotation - _right_paddle_rotation_old) / (float)delta;
        // Godot.Vector3 force = new Godot.Vector3(-_right_paddle_angular_velocity.Z, 0, -_right_paddle_angular_velocity.X);
        
        // if (_right_force_point.GlobalPosition.Y < GlobalPosition.Y) {
        //     ApplyForce(-sideways_force_ratio * force, _right_paddle.Position);
        //     ApplyCentralForce(-forward_force_ratio * Curve(force) * force.Sign().Z * forward);
        // }                

        // checking marker3d in the probe container for simulating the water physics
        isSubmerged = false;
		foreach(Marker3D p in probeContainer)
		{
			float depth = (float)water.getHeight(GlobalPosition) - p.GlobalPosition.Y;

			 if(depth > 0)
			 {
				isSubmerged = true;
				ApplyForce(Godot.Vector3.Up * floatForce * gravity * depth, p.GlobalPosition - GlobalPosition);
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
    }

    private float Curve(Godot.Vector3 v) {
        return Mathf.Pow(v.Length() / 10, 2);
    }

    // private void HandleInput(int device, Godot.Vector3 input) {
    // }
}
