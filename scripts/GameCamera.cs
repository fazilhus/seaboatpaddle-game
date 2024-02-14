using Godot;
using System;

public partial class GameCamera : Node3D
{
    [Export]
    public Node3D Boat;
   
    [Export]
    public Vector3 rotation_offset = new Vector3(-120, 0, 180);
    [Export]
    float followSpeed=1f;
    public override void _Process(double delta)
    {
         Vector3 tempPos = Position.Lerp(Boat.GlobalPosition, (float)delta * followSpeed);
        Position = new Vector3(tempPos.X, 10, tempPos.Z);
        Rotation= new Vector3(55.5f,Boat.GlobalRotation.Y,0);
        /*Vector3 boatPos = Boat.GetParentNode3D().Position;
        Vector3 boatToAnchorDiff = boatPos - Boat.GlobalPosition;
        Vector3 positionOffest= new Vector3();
        positionOffest.X= boatToAnchorDiff.X*(5.5f/1.5f);
        positionOffest.Z = boatToAnchorDiff.Z * (5.5f / 1.5f);
        positionOffest.Y = 10;
        GD.Print(boatToAnchorDiff);
        GlobalPosition = positionOffest;
        //RotationDegrees = new Vector3(0, Boat.Rotation.Y, 0) + rotation_offset;
        RotationDegrees = new Vector3(-55.5f, -180, 0)+Boat.GetParentNode3D().RotationDegrees; // correct rotation*/
    }
}
