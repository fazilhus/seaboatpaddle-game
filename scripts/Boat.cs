using Godot;
using System;

enum PaddleSide {
    Left = 0,
    Right = 1,
}

public partial class Boat : RigidBody3D
{
    private Node3D _left_paddle;
    private Node3D _right_paddle;
    private Node3D _left_force_point;
    private Node3D _right_force_point;
    private Node3D _pivot;

    private PaddleSide _paddle_side;

    private Vector3 _left_paddle_rotation_old;
    private Vector3 _left_paddle_angular_velocity;
    private Vector3 _right_paddle_rotation_old;
    private Vector3 _right_paddle_angular_velocity;

    //BoatPhysicsVariables

	[Export] private float floatForce = 10.0f;
	[Export] private float waterDrag = 0.05f;
	[Export] private float WaterAngularDrag = 0.05f;
	[Export] private bool isSubmerged = false;
	private float gravity;

	private float initialY;
	private double elapsedTime = 0;

	[Export]
	private float bobbingFactor = 0.1f;
	[Export]
	private float bobbingSpeed = 2.0f;

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
        _paddle_side = PaddleSide.Left;

        //instanciate variables for boat physics
        gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
		water = GetNode<WaterPlane>("/root/Main/WaterPlane");
		probeContainer = GetNode<Node3D>("ProbeContainer").GetChildren();
	
		initialY = GlobalPosition.Y;
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("switch_side")) {
            if (_paddle_side == PaddleSide.Left) {
                _paddle_side = PaddleSide.Right;
            }
            else {
                _paddle_side = PaddleSide.Left;
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 forward = _pivot.Basis.Z;
        Vector3 left_paddle_forward = _left_paddle.Basis.Z;
        Vector3 right_paddle_forward = _right_paddle.Basis.Z;

        Vector3 input = Vector3.Zero;
        input.Z = -Input.GetActionStrength("right") + Input.GetActionStrength("left");
        input.X = -Input.GetActionStrength("backward") + Input.GetActionStrength("forward");

        switch (_paddle_side) {
            case PaddleSide.Left: {
                _left_paddle_rotation_old = _left_paddle.Rotation;
                _left_paddle.Rotation = new Vector3(input.X * 0.8f, 0, input.Z * 0.5f);
                _left_paddle_angular_velocity = (_left_paddle.Rotation - _left_paddle_rotation_old) / (float)delta;

                Vector3 force = new Vector3(_left_paddle_angular_velocity.Z, 0, -_left_paddle_angular_velocity.X);
                DebugDraw2D.SetText("Left Y", _left_force_point.GlobalPosition.Y);
                if (_left_force_point.GlobalPosition.Y > 0.5f) {
                    force = Vector3.Zero;
                }

                ApplyForce(-0.125f * force * forward, _left_paddle.Position);
                ApplyCentralForce(-4 * _left_paddle_angular_velocity.Sign().X * forward);
                break;
            }
            case PaddleSide.Right: {
                _right_paddle_rotation_old = _right_paddle.Rotation;
                _right_paddle.Rotation = new Vector3(input.X * 0.8f, 0, input.Z * 0.5f);
                _right_paddle_angular_velocity = (_right_paddle.Rotation - _right_paddle_rotation_old) / (float)delta;
                Vector3 force = new Vector3(-_right_paddle_angular_velocity.Z, 0, -_right_paddle_angular_velocity.X);
                
                DebugDraw2D.SetText("Right Y", _right_force_point.GlobalPosition.Y);
                if (_right_force_point.GlobalPosition.Y > 0.5f) {
                    force = Vector3.Zero;
                }

                ApplyForce(-0.125f * force * forward, _right_paddle.Position);
                ApplyCentralForce(-4 * _right_paddle_angular_velocity.Sign().X * forward);
                break;
            }
        }
        
        // checking marker3d in the phrobecontainer for simulating the water physics
        isSubmerged = false;
		foreach(Marker3D p in probeContainer)
		{
			float depth = (float)water.getHeight(GlobalPosition) - p.GlobalPosition.Y;

			 if(depth > 0)
			 {
				isSubmerged = true;
				ApplyForce(Vector3.Up * floatForce * gravity * depth, p.GlobalPosition - GlobalPosition);
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
}
