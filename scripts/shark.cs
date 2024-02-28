using Godot;
using System;
using System.Collections.Generic;
public partial class shark : CharacterBody3D
{
	[Export]
	public Boat boat;
	[Export]
	public Node3D path;

	enum Behavior 
	{
		Patrol = 0,
		Chase = 1,
	}

	private Behavior _behavior;

	private List<Marker3D> _path_nodes;
	private int _next_path_node_idx;
	private Marker3D _next_path_node;
	private Area3D _trigger_area;

	[Export]
	public float velocity = 5;
	[Export]
	public float chasing_mul = 1.5f;
	[Export]
	public float rotation_follow_speed = Mathf.Tau / 6;


	public override void _Ready()
	{
		_path_nodes = new List<Marker3D>(); // Create an empty list to hold Marker3D nodes
		var children = path.GetChildren();

		foreach (var child in children)
		{
			if (child.GetClass() == "Marker3D")
			{
				var marker = child as Marker3D; // Attempt to cast the child to Marker3D
				_path_nodes.Add(marker); // Add the valid Marker3D node to the list
			}
			else if (child.GetClass() == "Area3D") 
			{
				var area = child as Area3D;
				_trigger_area = area;
			}
		}
		(_next_path_node_idx, _next_path_node) = (-1, null);
		_behavior = Behavior.Patrol;
	}

	public override void _Process(double delta)
	{
		// Movement along the path
		if (_trigger_area.OverlapsBody(boat)) 
		{
			_behavior = Behavior.Chase;
		}
		else 
		{
			_behavior = Behavior.Patrol;
		}

		switch (_behavior) 
		{
			case Behavior.Patrol: 
				{
				GetNode<AnimationPlayer>("Body/sharkswim/AnimationPlayer").SpeedScale = 1;
				_PatrolMovement((float)delta);
				break;
			}
			case Behavior.Chase: 
				{
				GetNode<AnimationPlayer>("Body/sharkswim/AnimationPlayer").SpeedScale = 1.5f;
				_ChaseMovement((float)delta);
				break;
			}
		}
		MoveAndSlide();
	}

	private void _PatrolMovement(float delta) 
	{
		if (_next_path_node == null) 
		{
			(_next_path_node_idx, _next_path_node) = _GetClosestPathNode();
		}

		var rot = Rotation;
		var target_dir = (_next_path_node.GlobalPosition - GlobalPosition).Normalized();
		var target_angle = Basis.Z.SignedAngleTo(target_dir, Vector3.Up);
		var angle_diff = Mathf.Sign(target_angle);
				
		rot.Y += Mathf.Clamp(delta * rotation_follow_speed * chasing_mul, 0, Mathf.Abs(angle_diff)) * Mathf.Sign(angle_diff);
		Rotation = rot;

		var vel = Basis.Z * velocity * delta;
		Velocity = vel;

		var dist = _next_path_node.GlobalPosition.DistanceTo(GlobalPosition);
		if (dist < 2) {
			(_next_path_node_idx, _next_path_node) = _GetNextPathNode(_next_path_node_idx);
		}
	}

	private void _ChaseMovement(float delta) 
	{
		(_next_path_node_idx, _next_path_node) = (-1, null);
		var rot = Rotation;
		var target_dir = (boat.GlobalPosition - GlobalPosition).Normalized();
		var target_angle = Basis.Z.SignedAngleTo(target_dir, Vector3.Up);
		var angle_diff = Mathf.Sign(target_angle);
				
		rot.Y += Mathf.Clamp(delta * rotation_follow_speed, 0, Mathf.Abs(angle_diff)) * Mathf.Sign(angle_diff);
		Rotation = rot;

		var vel = Basis.Z * velocity * chasing_mul * delta;
		Velocity = vel;
	}

	private (int, Marker3D) _GetClosestPathNode() 
	{
		float min_dist = float.MaxValue;
		int min_idx = 0;
		int idx = 0;
		Marker3D closest_marker = null;
		foreach (var path_node in _path_nodes) 
		{
			var dist = path_node.GlobalPosition.DistanceTo(GlobalPosition);
			if (dist < min_dist) 
			{
				min_dist = dist;
				closest_marker = path_node;
				min_idx = idx;
			}
			idx++;
		}
		return (min_idx, closest_marker);
	}

	private (int, Marker3D) _GetNextPathNode(int idx) 
	{
		idx = (idx + 1) % _path_nodes.Count;
		return (idx, _path_nodes[idx]);
	}

	public void OnBiteAreaEntered(Area3D area)
	{
		if (area.IsInGroup("ThePlayers"))
		{
			GD.Print("Chomp");
			boat.AttackedByShark((area.GlobalPosition - GlobalPosition).Normalized());
		}
	}
}
