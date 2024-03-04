using Godot;
using System;

public partial class StartingLocation : StaticBody3D
{
    private Godot.Collections.Array<Node3D> _unloading_markers;
    private int _unloading_idx;

    public override void _Ready()
    {
        _unloading_markers = misc.GetChildren<Node3D>(GetNode<Node3D>("UnloadPositions"));
        _unloading_idx = 0;
    }

    public void StartUnloading(Node3D stack) {
        //GD.Print($"Unloaded {stack.GetChildCount()} crates");
        if (stack.GetChildCount() == 0) {
            return;
        }

        while (stack.GetChildCount() != 0) {
            var child = stack.GetChildOrNull<Goods>(0);
            
            if (child == null) {
                return;
            }

            child.Disable();
            var marker = _unloading_markers[_unloading_idx];
            child.Rotation = marker.Rotation;
            child.GlobalPosition = marker.GlobalPosition;
            var pos = child.Position;
            pos.Y += marker.GetChildCount();
            child.Position = pos;
            child.Reparent(marker);
            _unloading_idx = (_unloading_idx + 1) % _unloading_markers.Count;
        }
    }
}
