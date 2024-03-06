using Godot;
using System;

public partial class StartingLocation : StaticBody3D
{
	[Signal]
	public delegate void OnCrateUnloadEventHandler();

	private Godot.Collections.Array<Node3D> _unloading_markers;
	private int _unloading_idx;
	private Node3D _stack;
	private Timer _unloadDelayTimer;

	public override void _Ready()
	{
		_unloading_markers = misc.GetChildren<Node3D>(GetNode<Node3D>("UnloadPositions"));
		_unloading_idx = 0;
		_unloadDelayTimer = GetNode<Timer>("UnloadDelayTimer");
	}

	public void StartUnloading(Node3D stack) {
		//GD.Print($"Unloaded {stack.GetChildCount()} crates");
		if (stack.GetChildCount() == 0) {
			return;
		}
		_stack = stack;
		_unloadDelayTimer.Start();
	}

	private void OnUnloadDelayTimerTimeout() {
		var count = _stack.GetChildCount();
		if (count == 0) {
			_unloadDelayTimer.Stop();
			return;
		}

		var child = _stack.GetChildOrNull<Goods>(count - 1);
		
		if (child == null) {
			return;
		}

		child.Disable();
		var marker = _unloading_markers[_unloading_idx];
		child.GlobalPosition = marker.GlobalPosition;
		var pos = child.Position;
		pos.Y += marker.GetChildCount();
		child.Position = pos;
		child.Reparent(marker);
		var rot = Vector3.Zero;
		var ng = new NumberGenerator();
		var angle = Mathf.DegToRad(ng.GenerateFloat(-10, 10));
		rot.Y = angle;
		child.Rotation = rot;
		_unloading_idx = (_unloading_idx + 1) % _unloading_markers.Count;

		EmitSignal(SignalName.OnCrateUnload);
	}
}
