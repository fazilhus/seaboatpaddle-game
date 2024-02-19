using Godot;
using System;
using System.Collections.Generic;
public partial class shark : CharacterBody3D
{
	[Export]
	public Node3D path;

	private List<Marker3D> _path_nodes;
	private int _next_path_node_idx;
	private Marker3D _next_path_node;

	[Export]
	public float velocity = 5;
	[Export]
	public float chasing_mul = 1.5f;
	[Export]
	public float rotation_follow_speed = Mathf.Tau / 6;
	private double time;


    public override void _Ready()
    {
		_path_nodes = new List<Marker3D>(); // Create an empty list to hold Marker3D nodes
		var children = path.GetChildren();

		foreach (var child in children)
		{
			var marker = child as Marker3D; // Attempt to cast the child to Marker3D
			if (marker != null)
			{
				_path_nodes.Add(marker); // Add the valid Marker3D node to the list
			}
		}
		
		(_next_path_node_idx, _next_path_node) = _GetClosestPathNode();
		GD.Print(_next_path_node.Name);
		time = 0;
    }

    public override void _Process(double delta)
    {
		DebugDraw3D.DrawLine(Position, Position + 3 * Basis.X, Colors.Red);
		DebugDraw3D.DrawLine(Position, Position + 3 * Basis.Y, Colors.Green);
		DebugDraw3D.DrawLine(Position, Position + 3 * Basis.Z, Colors.Blue);
		foreach (var path_node in _path_nodes) {
			DebugDraw3D.DrawSphere(path_node.GlobalPosition, 0.5f, Colors.Green);
		}
		time += delta;

		// Movement along the path
		var rot = Rotation;
		var target_dir = (_next_path_node.GlobalPosition - GlobalPosition).Normalized();
		DebugDraw3D.DrawLine(Position, Position + 3 * target_dir, Colors.Yellow);
		var target_angle = Basis.Z.SignedAngleTo(target_dir, Vector3.Up);
		//var angle_diff = Mathf.Wrap(target_angle - rot.Y, -Mathf.Pi, Mathf.Pi);
		var angle_diff = Mathf.AngleDifference(0, target_angle);
				
		rot.Y += Mathf.Clamp((float)delta * rotation_follow_speed, 0, Mathf.Abs(angle_diff)) * Mathf.Sign(angle_diff);
		Rotation = rot;

		var vel = Basis.Z * velocity * (float)delta;
		Velocity = vel;

		var dist = _next_path_node.GlobalPosition.DistanceTo(GlobalPosition);
		if (dist < 2) {
			(_next_path_node_idx, _next_path_node) = _GetNextPathNode(_next_path_node_idx);
			GD.Print(_next_path_node.Name);
		}

		if (time > 0.05) {
			time = 0;
			DebugDraw2D.SetText("Angle To Next PathNode ", target_angle);
			DebugDraw2D.SetText("Rot ", RotationDegrees);
			DebugDraw2D.SetText("Dist ", dist);
		}

		MoveAndSlide();
    }

	private (int, Marker3D) _GetClosestPathNode() {
		float min_dist = float.MaxValue;
		int min_idx = 0;
		int idx = 0;
		Marker3D closest_marker = null;
		foreach (var path_node in _path_nodes) {
			var dist = path_node.GlobalPosition.DistanceTo(GlobalPosition);
			if (dist < min_dist) {
				min_dist = dist;
				closest_marker = path_node;
				min_idx = idx;
			}
			idx++;
		}
		return (min_idx, closest_marker);
	}

	private (int, Marker3D) _GetNextPathNode(int idx) {
		idx = (idx + 1) % _path_nodes.Count;
		return (idx, _path_nodes[idx]);
	}
}
