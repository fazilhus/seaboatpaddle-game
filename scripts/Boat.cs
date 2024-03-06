using Godot;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

enum PaddleSide 
{
	Left = 0,
	Right = 1,
}

public partial class Boat : RigidBody3D
{	
	[Signal]
	public delegate void NoBoatHealthEventHandler();
	[Signal]
	public delegate void ObjectivePickupEventHandler(int stack_size);

	[Export]
	public Godot.Collections.Array<Node3D> paddles;

	private List<Vector3> _player_inputs;
	private List<Vector3> _paddles_rotation_old;
	[Export]
	Material[] paddleMaterial;
	private List<bool> _is_paddle_moving;

	[Export]
	public float sideways_force_ratio = 0.5f;
	[Export]
	public float forward_force_ratio = 50;
	
	//BoatPhysicsVariables

	[Export] private float floatForce = 1.0f;
	[Export] private float waterDrag = 0.005f;
	[Export] private float WaterAngularDrag = 0.01f;
	[Export] public bool isSubmerged = false;
	private float gravity;
	private float initialY;
	private double elapsedTime = 0;
	[Export] public WaterPlane water;
	
	public float time = 1;

	public Godot.Collections.Array<Node> probeContainer;
	
	[Export] private bool isVortexCollided;
	[Export] private bool isStreamCollided;
	[Export] private float maxDistance = 20.0f; // Example maximum distance from the center

	 // Define the vortex center
	private Vector3 vortexCenter = new Vector3(10, 5, 0); // Example vortex center position
	private Vector3 streamGlobalPosition;

	// Define the base force magnitude
	[Export]
	public float baseForceMagnitude = 100.0f; // Example base force magnitude
	[Export]
	public float rotationalForceMagnitude = 10.0f;
	[Export]
	public float rotationalVelocity = 5;

	public bool RepairKit { get; private set; } = false;
	public bool SpeedBoost { get; private set; } = false;
	public bool UsingSpeedBoost = false;
	private int amountOfRepairKits = 0;
	private int amountOfSpeedBoosts = 0;
	public bool isBombed;
	[Export] public int bombAcceleration = 40;
	public void ActivateRepairKit(int repairKitAmount)
	{
		RepairKit = true;
		amountOfRepaitKits = repairKitAmount;
		GetNode<AudioStreamPlayer>("RepairKitFound").Play();
	}
	
	public void ActivateSpeedBoost(int speedBoostAmount)
	{
		amountOfSpeedBoosts = speedBoostAmount;
		SpeedBoost = true;
	}

	private float strengthFactor; 
	public bool isReadyPaddle = false;
	private HealthComponent healthComp;
	
	//[Export] Survivors survivors;
	public GpuParticles3D WaterSplashRatio;
	public GpuParticles3D watersplashLeft;
	public GpuParticles3D watersplashRight;

	[Export]
	public PackedScene goods_scene;
	[Export]
	public Node3D goods_node;
	private Node3D _goods_stack;
	[Export]
	public int StackSize = 0;

	private AudioStreamPlayer3D crashSounds;


	public override void _Ready()
	{
		GetNode<AnimationPlayer>("FanOn/AnimationPlayer").Stop();
		var fanOn = GetNode<Node3D>("FanOn");
		fanOn.Visible = false;
		crashSounds = GetNode<AudioStreamPlayer3D>("BoatCrash");
		//instantiate variables for boat physics
		var parent = GetParent();
		gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
		probeContainer = GetNode<Node3D>("ProbeContainer").GetChildren();
		//survivors = parent.GetNode<Survivors>("Survivors");
		WaterSplashRatio = GetNode<GpuParticles3D>("GPUParticles3D");
		watersplashLeft = GetNode<Node3D>("LeftPaddlePivot").GetNode<GpuParticles3D>("GPUsplashEffect");
		watersplashRight = GetNode<Node3D>("RightPaddlePivot").GetNode<GpuParticles3D>("GPUsplashEffect2");
	
		initialY = GlobalPosition.Y;
		StackSize = GetParent<WorldScene>().maxObjectiveStackAmount;
		_player_inputs = new List<Vector3>();
		_paddles_rotation_old = new List<Vector3>();
		_is_paddle_moving = new List<bool>();
		foreach (var _ in paddles) 
		{
			_player_inputs.Add(Vector3.Zero);
			_paddles_rotation_old.Add(Vector3.Zero);
			_is_paddle_moving.Add(false);
		}

		healthComp = GetNode<HealthComponent>("HealthComponent");

		MeshInstance3D paddle1 = paddles[0].GetChild(1).GetChild<MeshInstance3D>(0); //Supposed to access the mesh inside each individual paddle
		MeshInstance3D paddle2 = paddles[1].GetChild(1).GetChild<MeshInstance3D>(0);
		paddle1.SetSurfaceOverrideMaterial(0, paddleMaterial[0]);
		paddle2.SetSurfaceOverrideMaterial(0, paddleMaterial[1]);
		paddle1.GetSurfaceOverrideMaterial(0).Set("albedo_color", PlayerManager.instance.playerColors[0]);
		paddle2.GetSurfaceOverrideMaterial(0).Set("albedo_color", PlayerManager.instance.playerColors[1]);
	
		_goods_stack = GetNode<Node3D>("GoodsStack");
	}
 
	public override void _Process(double delta)
	{
		if(amountOfSpeedBoosts > 0)
		{
			var fanOn = GetNode<Node3D>("FanOn");
			fanOn.Visible = true;
		}
		if (Input.IsKeyPressed(Key.F2)) 
		{
			GetNode<HealthComponent>("HealthComponent").SubtractHealth(100);
		}
		if (Input.IsKeyPressed(Key.F3)) 
		{
			var rot = Rotation;
			rot.X = 0;
			rot.Z = 0;
			Rotation = rot;
		}
		GetParent<Node3D>().GetNode<Label>("GameCamera/CanvasLayer/LabelHealth").Text = "Health: "+ GetNode<HealthComponent>("HealthComponent").health;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 forward = Basis.Z;
		foreach (var it in paddles.Select((paddle, i) => new {Paddle = paddle, Index = i})) 
		{
			if (!isReadyPaddle) 
			{
				continue;
			}
			if (it.Index >= _player_inputs.Count) 
			{
				continue;
			}
			
			Vector3 input = GetPlayerInput(it.Index);

			_paddles_rotation_old[it.Index] = it.Paddle.Rotation;
			it.Paddle.Rotation = new Vector3(input.X * 0.8f, 0, input.Z * 0.5f);
			Vector3 angular_velocity = (it.Paddle.Rotation - _paddles_rotation_old[it.Index]) / (float)delta;
			Vector3 force = new Vector3(angular_velocity.Z, 0, -angular_velocity.X);

			_is_paddle_moving[it.Index] = force.Length() > 0.01f;

			Node3D force_point = it.Paddle.GetNode<Node3D>("ForcePoint");
			if (isSubmerged && force_point.GlobalPosition.Y < GlobalPosition.Y) 
			{
				if (!_is_paddle_moving[(it.Index + 1) % 2]) 
				{
					ApplyForce(-sideways_force_ratio * 1.25f * force, it.Paddle.Position);
					ApplyCentralForce(0.25f * -forward_force_ratio * Curve(force) * force.Sign().Z * forward);
				}
				else 
				{
					ApplyForce(-sideways_force_ratio * force, it.Paddle.Position);
					ApplyCentralForce(-forward_force_ratio * Curve(force) * force.Sign().Z * forward);
				}

				float speedRotation = angular_velocity.Length();
				if (speedRotation <= 30.0f && _paddles_rotation_old[it.Index] == _paddles_rotation_old[0])
				{
					watersplashLeft.Emitting = true;
					watersplashLeft.AmountRatio = speedRotation;
					watersplashLeft.Rotate(Vector3.Up, Mathf.Pi * angular_velocity.Sign().Z);
				}
				else if (speedRotation <= 30.0f && _paddles_rotation_old[it.Index] == _paddles_rotation_old[1])
				{
					watersplashRight.Emitting = true;
					watersplashRight.AmountRatio = speedRotation;
					watersplashRight.Rotate(Vector3.Up, Mathf.Pi * angular_velocity.Sign().Z);
				}
			}
			else 
			{
				watersplashLeft.AmountRatio = 0;
				watersplashRight.AmountRatio = 0;
			}

			if (UsingSpeedBoost) 
			{
				ApplyCentralForce(1000 * forward * (float)delta);
				GetNode<AnimationPlayer>("FanOn/AnimationPlayer").Play();
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
					Vector3 relativePosition = -Position + vortexCenter;
					relativePosition.Y = 0;

					// Calculate the distance from the boat to the center of the whirlpool
					float distanceToCenter = relativePosition.Length();

					// Define the base force toward the center of the whirlpool
					Vector3 baseForce = relativePosition.Normalized() * baseForceMagnitude;

					// Adjust the force based on the distance from the center
					float strengthFactor = (maxDistance - distanceToCenter) / maxDistance;
					Vector3 whirlpoolForce = baseForce * strengthFactor;

					// Apply the whirlpool force to the boat
					ApplyCentralForce(whirlpoolForce * (float)delta);

					// Calculate the rotational force to simulate rotation around the whirlpool
					Vector3 tangentialVelocity = relativePosition.Rotated(Vector3.Left, Mathf.DegToRad(-90)) * rotationalForceMagnitude * (float)delta;

					// Apply the rotational force to the boat
					var v = Vector3.Down.Cross(-relativePosition) * rotationalVelocity * (float)delta;
					
					ApplyCentralForce(v);
					ApplyTorque(tangentialVelocity);
					depth += -2.5f * strengthFactor;
				} 

				ApplyForce(Vector3.Up * floatForce * gravity * depth, p.GlobalPosition - GlobalPosition);
			}
			else 
			{
				isSubmerged = false;
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

		if (state.LinearVelocity.Length() >= 8) 
		{
			state.LinearVelocity = 8 * state.LinearVelocity.Normalized();
		}
	}

	private float Curve(Vector3 v) 
	{
		return Mathf.Pow(v.Length() / 10, 2);
	}

	private void LooseStackedGoods() 
	{
		SetMonitoringGoods(false);
		SetMonitorableGoods(false);

		var nr = new NumberGenerator();
		for (int i = 0; i < _goods_stack.GetChildCount(); i++) {
			var goods = _goods_stack.GetChild<Goods>(0);
			goods.Reparent(goods_node);
			var pos = goods.GlobalPosition;
			pos.Y += 2;
			goods.GlobalPosition = pos;
			var angle = nr.GenerateFloat(-Mathf.Pi, Mathf.Pi);
			var vector = Basis.Z.Rotated(Vector3.Up, angle);
			goods.Enable();
			goods.ApplyCentralImpulse(5 * vector);

		}

		GetNode<Timer>("CrashCooldown").Start();
	}
	public void EmptyCargo()
	{
		var count = _goods_stack.GetChildCount();
	   
		for (int i = 0; i < count; i++)
		{
			var child = _goods_stack.GetChild<Node3D>(i);
		   
			child.CallDeferred("free");
		}
		

	}
	public void OnBoatArea3dBodyExited(Area3D area)
	{
		if(area.IsInGroup("Vortex"))
		{
			isVortexCollided = false;
		}

		if (area.IsInGroup("VortexDamage")) 
		{
			GetNode<Timer>("VortexDamageTimer").Stop();
		}
	}

	private Vector3 GetPlayerInput(int device_id) 
	{
		Vector3 input = Vector3.Zero;
		input.Z = -Input.GetJoyAxis(device_id, JoyAxis.LeftX);
		input.X = Input.GetJoyAxis(device_id, JoyAxis.LeftY);
		return input;
	}

	public void SetMonitoringGoods(bool val) 
	{
		GetNode<Area3D>("Area3DTriggerGoods").SetDeferred("monitoring", val);
	}

	public void SetMonitorableGoods(bool val) 
	{
		GetNode<Area3D>("Area3DTriggerGoods").SetDeferred("monitorable", val);
	}

	private void SeaMineImpact(Vector3 pos) 
	{
		Vector3 BoomVector = -pos + GlobalPosition;
		ApplyCentralImpulse(BoomVector.Normalized() * bombAcceleration);
	}

	public void OnArea3DTriggerGoodsEntered(Area3D area) 
	{
		if (area.IsInGroup("Goods"))
		{
			GD.Print("boat is colliding with goods!");
			var count = _goods_stack.GetChildCount();
			if (count < StackSize) {
				var goods = area.GetParent<Goods>();
				goods.Disable();
				goods.Reparent(_goods_stack, false);
				goods.Position = new Vector3(0, count, 0);
			}

			count = _goods_stack.GetChildCount();
			if (count == StackSize) 
			{
				var boat_area = GetNode<Area3D>("Area3DTriggerGoods");
				boat_area.SetDeferred("monitorable", false);
			}
			EmitSignal(SignalName.ObjectivePickup, count);
		}

		if (area.IsInGroup("Unloading")) 
		{
			var loc = area.GetParent<StartingLocation>();
			loc.StartUnloading(_goods_stack);
		}
	}

	public void OnArea3dTriggerBoatAreaEntered(Area3D area)
	{	
		if (area.IsInGroup("SeaMine")) 
		{
			GD.Print("Boom!!!");
			LooseStackedGoods();
			SeaMineImpact(area.GetParent<Node3D>().GlobalPosition);
			healthComp.SubtractHealth(50);
		}

		if(area.IsInGroup("Modifiers")) 
		{
			GD.Print("boat is colliding with modifiers!");
		}

		if (area.IsInGroup("Vortex"))
		{
			isVortexCollided = true;
			vortexCenter = area.GlobalPosition;
			GD.Print("Collided with vortex!", area.GlobalPosition);
		
		}

		if (area.IsInGroup("VortexDamage")) 
		{
			LooseStackedGoods();
			GetNode<Timer>("VortexDamageTimer").Start();
		}
	}

	public void OnHealthComponentNoHealthEvent() 
	{
		GD.Print("Boat lost all durability: You Lose");
		EmitSignal(SignalName.NoBoatHealth);
	}

	public void OnBodyEntered(Node node) 
	{
		GD.Print(node.Name);
		if (node.IsInGroup("Rock")) 
		{
			var speed = LinearVelocity.Length();
			if (speed < 5) 
			{
				return;
			}
			ApplyCentralImpulse(-10 * LinearVelocity);
			healthComp.SubtractHealth(3 * (int)speed);
			crashSounds.Play();
			crashSounds.VolumeDb = speed;

			LooseStackedGoods();
		}
	}

	public void OnSpeedBoostTimerTimeout() 
	{
		GD.Print("'Speed Boost' modifier has ended");
		UsingSpeedBoost = false;
		var fanOn = GetNode<Node3D>("FanOn");
		GetNode<AnimationPlayer>("FanOn/AnimationPlayer").Stop();
		fanOn.Visible = false;
	}

	public void OnVortexDamageTimerTimeout() 
	{
		healthComp.SubtractHealth(10);
	}

	private void OnCrashCooldownTimerTimeout() {
		SetMonitoringGoods(true);
		SetMonitorableGoods(true);
	}
	
	public override void _UnhandledInput(InputEvent @event)
	{
		if(@event.IsActionPressed("ui_cancel")) //Press B
		{
			if (RepairKit && amountOfRepairKits > 0)
			{
				amountOfRepairKits--;
				GameCamera.RepairKitModifierLabel.Text = "";
				GameCamera.RepairKitModifierLabel.Text += amountOfRepairKits;
				healthComp.AddHealth(25);
				GetNode<AudioStreamPlayer>("Repearing").Play();
				if(amountOfRepaitKits <= 0)
				{
					RepairKit = false;
				}
				Modifiers.amountOfRepairKits = amountOfRepairKits;
			}
		}
		
		if(@event.IsActionPressed("ui_accept")) //Press A
		{
			if(SpeedBoost && amountOfSpeedBoosts > 0)
			{
				amountOfSpeedBoosts--;
				GameCamera.SpeedBoostModifierLabel.Text = "";
				GameCamera.SpeedBoostModifierLabel.Text += amountOfSpeedBoosts;
				GetNode<Timer>("SpeedBoostTimer").Start();
				UsingSpeedBoost = true;
				if(amountOfSpeedBoosts <= 0)
				{
					SpeedBoost = false;
				}
				Modifiers.amountOfSpeedBoosts = amountOfSpeedBoosts;
			}
		}
	}
	
	public void AttackedByShark(Vector3 attack_dir)
	{
		healthComp.SubtractHealth(17);
		LooseStackedGoods();
		ApplyCentralImpulse(50 * attack_dir);
	}
}
