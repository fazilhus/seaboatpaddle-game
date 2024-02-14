using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

enum PaddleSide {
    Neutral = 0,
    Left = 1,
    Right = 2,
}

public partial class Boat : RigidBody3D
{
    [Export]
    public Godot.Collections.Array<Node3D> rows;

    private List<Vector3> _player_inputs;
    private List<Vector3> _paddles_rotation_old;

    [Export]
    public float sideways_force_ratio = 0.5f;
    [Export]
    public float forward_force_ratio = 50;
    
    //BoatPhysicsVariables

	[Export] private float floatForce = 1.0f;
	[Export] private float waterDrag = 0.005f;
	[Export] private float WaterAngularDrag = 0.01f;
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
    
    [Export] Survivors survivors;

    PaddleSide[] rowPadddleChoice = new PaddleSide[4];
    public override void _Ready()
    {
        //instantiate variables for boat physics
        var parent = GetParent();
        gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
		//water = parent.GetNode<WaterPlane>("WaterPlane");
		probeContainer = GetNode<Node3D>("ProbeContainer").GetChildren();
        survivors = parent.GetNode<Survivors>("Survivors");
        
	
		initialY = GlobalPosition.Y;

        _player_inputs = new List<Vector3>();
        _paddles_rotation_old = new List<Vector3>();
        foreach (int device_id in Input.GetConnectedJoypads()) {
            _player_inputs.Add(Vector3.Zero);
            rowPadddleChoice[device_id] = PaddleSide.Neutral;
            _paddles_rotation_old.Add(Vector3.Zero);
            _paddles_rotation_old.Add(Vector3.Zero);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 forward = Basis.Z;
        foreach (var row in rows.Select((row,i) => new {Row=row, Index = i})) {
            if (row.Index >= _player_inputs.Count) {
                continue;
            }
            
            // Grab a row, make tuple of paddles from children 
            //assign index of row to deviceId frontROw(index 0 = deviceID)
            //check for inputs from controller
            // returns a tuple (leftSTick,rightStick)
            //Angular velocity depending on which stick
            //
            int playerId = GetTree().CurrentScene.GetChild<PlayerController>(0).playerIds[row.Index];

            (Vector3,Vector3) inputs = GetPlayerInput(row.Index); // Change to state and vector. State to track which paddle
            GD.Print(inputs.Item1);
            //Vector3 input = _player_inputs[it.Index];
            if(row.Index==0)
            {
                
                if(Input.IsJoyButtonPressed(playerId,JoyButton.LeftShoulder))
                {
                    rowPadddleChoice[row.Index] = PaddleSide.Left;
                }
                else if(Input.IsJoyButtonPressed(playerId, JoyButton.RightShoulder))
                {
                    rowPadddleChoice[row.Index] = PaddleSide.Right;
                }

                if (rowPadddleChoice[row.Index] == PaddleSide.Left)
                {
                    Node3D leftPaddle = row.Row.GetChild<Node3D>(0);
                    _paddles_rotation_old[0] = leftPaddle.Rotation;
                    row.Row.GetChild<Node3D>(0).Rotation = new Vector3(inputs.Item1.X * 0.8f, 0, inputs.Item1.Z * 0.5f);
                    Vector3 angular_velocity_left = (row.Row.GetChild<Node3D>(0).Rotation - _paddles_rotation_old[0]) / (float)delta;
                    Vector3 leftForce = new Vector3(angular_velocity_left.Z, 0, -angular_velocity_left.X);
                    Node3D force_point_left = row.Row.GetChild<Node3D>(0).GetNode<Node3D>("ForcePoint");
                    if (force_point_left.GlobalPosition.Y < GlobalPosition.Y)
                    {
                        var origin = row.Row.GetChild<Node3D>(0).Position; // This is the PaddlePivot node
                        var pos = new Vector3(origin.X, origin.Y - 0.3f, origin.Z);
                        ApplyForce(-sideways_force_ratio * leftForce, pos);
                        ApplyCentralForce(-forward_force_ratio * Curve(leftForce) * leftForce.Sign().Z * forward);
                    }
                }
                else if(rowPadddleChoice[row.Index] == PaddleSide.Right)
                {
                    Node3D rightPaddle = row.Row.GetChild<Node3D>(1);

                    _paddles_rotation_old[1] = rightPaddle.Rotation;


                    row.Row.GetChild<Node3D>(1).Rotation = new Vector3(inputs.Item2.X * 0.8f, 0, inputs.Item2.Z * 0.5f);

                    Vector3 angular_velocity_right = (row.Row.GetChild<Node3D>(1).Rotation - _paddles_rotation_old[1]) / (float)delta;

                    Vector3 rightForce = new Vector3(angular_velocity_right.Z, 0, -angular_velocity_right.X);

                    Node3D force_point_right = row.Row.GetChild<Node3D>(1).GetNode<Node3D>("ForcePoint");

                    if (force_point_right.GlobalPosition.Y < GlobalPosition.Y)
                    {
                        var origin = row.Row.GetChild<Node3D>(1).Position;
                        var pos = new Vector3(origin.X, origin.Y - 0.3f, origin.Z);
                        ApplyForce(-sideways_force_ratio * rightForce, pos);
                        ApplyCentralForce(-forward_force_ratio * Curve(rightForce) * rightForce.Sign().Z * forward);
                    }

                }
                
                
            }
            else if(row.Index==1)
            {
               
                if (Input.IsJoyButtonPressed(playerId, JoyButton.LeftShoulder))
                {
                    rowPadddleChoice[row.Index] = PaddleSide.Left;
                }
                else if (Input.IsJoyButtonPressed(playerId, JoyButton.RightShoulder))
                {
                    rowPadddleChoice[row.Index] = PaddleSide.Right;
                }

                if (rowPadddleChoice[row.Index] == PaddleSide.Left)
                {
                    Node3D leftPaddle = row.Row.GetChild<Node3D>(0);
                    _paddles_rotation_old[2] = leftPaddle.Rotation;
                    row.Row.GetChild<Node3D>(0).Rotation = new Vector3(inputs.Item1.X * 0.8f, 0, inputs.Item1.Z * 0.5f);
                    Vector3 angular_velocity_left = (row.Row.GetChild<Node3D>(0).Rotation - _paddles_rotation_old[2]) / (float)delta;
                    Vector3 leftForce = new Vector3(angular_velocity_left.Z, 0, -angular_velocity_left.X);
                    Node3D force_point_left = row.Row.GetChild<Node3D>(0).GetNode<Node3D>("ForcePoint");
                    if (force_point_left.GlobalPosition.Y < GlobalPosition.Y)
                    {
                        var origin = row.Row.GetChild<Node3D>(0).Position; // This is the PaddlePivot node
                        var pos = new Vector3(origin.X, origin.Y - 0.3f, origin.Z);
                        ApplyForce(-sideways_force_ratio * leftForce, pos);
                        ApplyCentralForce(-forward_force_ratio * Curve(leftForce) * leftForce.Sign().Z * forward);
                    }
                }
                else if (rowPadddleChoice[row.Index] == PaddleSide.Right)
                {
                    Node3D rightPaddle = row.Row.GetChild<Node3D>(1);

                    _paddles_rotation_old[3] = rightPaddle.Rotation;


                    row.Row.GetChild<Node3D>(1).Rotation = new Vector3(inputs.Item2.X * 0.8f, 0, inputs.Item2.Z * 0.5f);

                    Vector3 angular_velocity_right = (row.Row.GetChild<Node3D>(1).Rotation - _paddles_rotation_old[3]) / (float)delta;

                    Vector3 rightForce = new Vector3(angular_velocity_right.Z, 0, -angular_velocity_right.X);

                    Node3D force_point_right = row.Row.GetChild<Node3D>(1).GetNode<Node3D>("ForcePoint");

                    if (force_point_right.GlobalPosition.Y < GlobalPosition.Y)
                    {
                        var origin = row.Row.GetChild<Node3D>(1).Position;
                        var pos = new Vector3(origin.X, origin.Y - 0.3f, origin.Z);
                        ApplyForce(-sideways_force_ratio * rightForce, pos);
                        ApplyCentralForce(-forward_force_ratio * Curve(rightForce) * rightForce.Sign().Z * forward);
                    }

                }
            }
            /*_paddles_rotation_old[it.Index] = it.Paddle.Rotation;//Previous frame rotation
            it.Paddle.Rotation = new Vector3(input.X * 0.8f, 0, input.Z * 0.5f); //Visual rotation
            
            
            Vector3 angular_velocity = (it.Paddle.Rotation - _paddles_rotation_old[it.Index]) / (float)delta;
            Vector3 force = new Vector3(angular_velocity.Z, 0, -angular_velocity.X);
            
            Node3D force_point = it.Paddle.GetNode<Node3D>("ForcePoint");
            if (force_point.GlobalPosition.Y < GlobalPosition.Y) {
                ApplyForce(-sideways_force_ratio * force, it.Paddle.Position);  
                ApplyCentralForce(-forward_force_ratio * Curve(force) * force.Sign().Z * forward);
            }*/
        }              

        // checking marker3d in the probe container for simulating the water physics
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

        if (state.LinearVelocity.Length() >= 8) {
            state.LinearVelocity = 8 * state.LinearVelocity.Normalized();
        }
    }

    private float Curve(Vector3 v) {
        return Mathf.Pow(v.Length() / 10, 2);
    }

    private (Vector3,Vector3) GetPlayerInput(int device_id) {
        Vector3 rightInput = Vector3.Zero;
        Vector3 leftInput = Vector3.Zero;
        rightInput.Z = -1 * Input.GetJoyAxis(device_id, JoyAxis.RightX);
        rightInput.X = Input.GetJoyAxis(device_id, JoyAxis.RightY);
        leftInput.X=  Input.GetJoyAxis(device_id,JoyAxis.LeftY);
        leftInput.Z = -1*Input.GetJoyAxis(device_id, JoyAxis.LeftX);
        
       /* switch (device_id) {
            case 0: {
                //input.Z = Input.GetAxis("right_p1", "left_p1");
                
                    
                // input.X = -Input.GetAxis("backward_p1", "forward_p1");
                
                break;
            }
            case 1: {
                input.Z = Input.GetAxis("right_p2", "left_p2");
                input.X = -Input.GetAxis("backward_p2", "forward_p2");
                break;
            }
        }*/

        return (leftInput,rightInput);
    }
    public void OnArea3dTriggerBoatAreaEntered(Area3D area)
    {
        if(area.IsInGroup("Survivors"))
        {
              GD.Print("boat is colliding with survivors!");
        }
      
    }
}
