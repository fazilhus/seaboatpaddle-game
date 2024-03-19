using System.Diagnostics;
using System.Reflection.Emit;
using Godot;
public partial class seagull : CharacterBody3D
{
	public Boat boat;
	public Goods goods;
	private Quaternion quaternion;
	private AudioStreamPlayer3D seagullSound;

	public bool isSeagull;
	public bool isGoods;
	private  Vector3 yAxelVector = new Vector3(0.0f, 9.0f, 0.0f);
	// Called when the node enters the scene tree for the first time.
	private Vector3 target;
	
	public override void _Ready()
	{
		boat = GetParent<Node3D>().GetNode<Boat>("Boat");
		seagullSound = GetNode<AudioStreamPlayer3D>("SeagullSound");
		target = Vector3.Zero;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
		if(isSeagull)
		{
			Vector3 boatCordinate = boat.GlobalPosition;
			quaternion =  new Quaternion(Vector3.Up,boatCordinate.SignedAngleTo(this.GlobalPosition.DirectionTo(boatCordinate), Vector3.Up));
			float distance = boatCordinate.DistanceTo(this.GlobalPosition);
			GlobalPosition += Transform.Basis.Z * 10 *(float)delta;
			Quaternion += Quaternion.Slerp(quaternion, (float)delta*5);
			//GD.Print("isSealgull", seagullSound.Playing);
			seagullSound.PitchScale = 1.0f;
			/*if(seagullSound.PitchScale <= distance/10)
						{
							seagullSound.PitchScale -= distance/100* (float)delta;
						}
			GD.Print(seagullSound.PitchScale);
			GD.Print(seagullSound.Playing);*/
			

		}

		if(isGoods)
		{
			//seagull
			var seagullArea = boat.GetNode<Area3D>("SeagullArea");
			var min_dist = float.MaxValue;
			//seagullSound.Autoplay = true;
			//ode3D seagull_target = null;
			foreach (var area in seagullArea.GetOverlappingAreas()) {
				if (area.IsInGroup("Goods"))
				{
					//GD.Print(area.GetParent().Name);
					var dist = GlobalPosition.DistanceTo(area.GlobalPosition);
					if (dist < min_dist) 
					{
						
						Vector3 goodsCordinate = area.GlobalPosition + yAxelVector;
						//seagull_target = area.GetParent<Node3D>();
						quaternion =  new Quaternion(Vector3.Up, goodsCordinate.SignedAngleTo(this.GlobalPosition.DirectionTo(goodsCordinate), Vector3.Up));
						float distance = goodsCordinate.DistanceTo(this.GlobalPosition);
						GlobalPosition += Transform.Basis.Z *  5 * (float)delta;
						Quaternion += Quaternion.Slerp(quaternion, (float)delta*15.0f);
						//GD.Print("isGoods", seagullSound.Playing);
						//GD.Print(distance);
						if(seagullSound.PitchScale <= distance/10)
						{
							seagullSound.PitchScale += distance/100* (float)delta;
						}
						
						
						

						//GD.Print(GlobalPosition, "globalposition");
						//GD.Print(Quaternion, "qiatermopm");
						//GD.Print(GlobalPosition, "globalposition");
						
					}
				}
			}

		}
		
		
	}

	/*public void SetTarget(Vector3 v)
	{
		target = v;
	}*/

}


