using Godot;
using System;

public partial class Boat : CharacterBody3D
{
    private Node3D _left_paddle;
    private Node3D _right_paddle;
    private Node3D _pivot;
    private Timer _left_paddle_timer;
    private Timer _right_paddle_timer;

    private Vector3 lpf = new Vector3(0.5f, 0.0f, 1.0f).Normalized();
    private float lpa = 15;
    private Vector3 rpf = new Vector3(-0.5f, 0.0f, 1.0f).Normalized();
    private float rpa = -15;

    private float power = 2;

    public override void _Ready()
    {
        _left_paddle = GetNode<Node3D>("LeftPaddlePivot");
        _right_paddle = GetNode<Node3D>("RightPaddlePivot");
        _left_paddle_timer = GetNode<Timer>("LeftPaddleTimer");
        _right_paddle_timer = GetNode<Timer>("RightPaddleTimer");
        _pivot = GetNode<Node3D>("Pivot");
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("left_paddle") && _left_paddle_timer.IsStopped()) {
            _left_paddle_timer.Start();
        }
        if (Input.IsActionJustPressed("right_paddle") && _right_paddle_timer.IsStopped()) {
            _right_paddle_timer.Start();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 velocity = Velocity;
        Vector3 rotation = _pivot.Rotation;
        
        Vector3 axis = new Vector3(0, 1, 0);
        if (!_left_paddle_timer.IsStopped()) {
            float delta_angle = Mathf.DegToRad(lpa) * (float)delta;
            rotation = rotation + new Vector3(0, delta_angle, 0);
            velocity += lpf.Rotated(axis, Mathf.DegToRad(lpa)) * power * (float)delta;
            velocity = velocity.Rotated(axis, delta_angle);
        }

        if (!_right_paddle_timer.IsStopped()) {
            float delta_angle = Mathf.DegToRad(rpa) * (float)delta;
            rotation = rotation + new Vector3(0, delta_angle, 0);
            velocity += rpf.Rotated(axis, Mathf.DegToRad(lpa)) * power * (float)delta;
            velocity = velocity.Rotated(axis, rotation.Y * (float)delta);
        }

        _pivot.Rotation = rotation;
        Velocity = velocity - velocity * 0.95f * (float)delta;
        DebugDraw3D.DrawLine(Position + Vector3.Up, Position + Velocity + Vector3.Up, Colors.Red);
        DebugDraw3D.DrawLine(Position + Vector3.Up, Position + Vector3.Forward.Rotated(axis, rotation.Y) + Vector3.Up, Colors.Blue);
        MoveAndSlide();
    }

    public void _on_left_paddle_timer_timeout() {

    }

    public void _on_right_paddle_timer_timeout() {

    }
}
