using Godot;
using System;

public partial class GameCamera : Node3D
{
    [Export]
    public Node3D Boat;
    [Export]
    public Vector3 position_offset = new Vector3(0, 10, -7);
    [Export]
    public Vector3 rotation_offset = new Vector3(-120, 0, 180);

    public override void _Process(double delta)
    {
        Position = Boat.Position + position_offset;
        RotationDegrees = new Vector3(0, Boat.Rotation.Y, 0) + rotation_offset;
    }
}
