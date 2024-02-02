using Godot;
using System;
using System.Runtime.InteropServices;

public partial class WaterPlane : MeshInstance3D
{
	private ShaderMaterial material;
	private Image noise;
	[Export] private float noiseScale;
	[Export] private float waveSpeed;
	[Export] private float HeightScale;

	private double time; // just to track time
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Image.CreateFromData
		this.MaterialOverride = this.material;
		material = (ShaderMaterial)MaterialOverride;
		noise = GD.Load<NoiseTexture2D>("res://Shades/Water.tres::NoiseTexture2D_cwxk6").GetImage();
		noiseScale = 1000;//(float)material.GetShaderParameter("noiseScale");
		waveSpeed = 0.01f;//(float)material.GetShaderParameter("timeScale");
		HeightScale = 2;//(float)material.GetShaderParameter("heightSale");
		time = 0;
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(noise == null)
			{
				noise = GD.Load<NoiseTexture2D>("res://Shades/Water.tres::NoiseTexture2D_cwxk6").GetImage();
				
			}
		time += delta;
		if (material != null)
		{
			material.SetShaderParameter("waveTime", time);
		}
	}
	  public double WrapValue(double value, double min, double max) // function used to wrap value within a specific range
	{
		double range = max - min;
		double wrappedValue = value;

		while (wrappedValue < min)
		{
			wrappedValue += range;
		};
		while (wrappedValue >= max)
		{
			wrappedValue -= range;
		}

		return wrappedValue;
	}

	public double getHeight(Vector3 worldPosition)
	{
		double UVX = WrapValue((double)worldPosition.X / noiseScale + time * waveSpeed, 0, 1); // UV Cordinates are always between 0 and 1
		double UVY = WrapValue((double)worldPosition.Z / noiseScale + time * waveSpeed, 0, 1);
		Vector2 pixelPos = new Vector2((float)UVX * (float)noise.GetWidth(),(float)UVY * (float)noise.GetHeight());
		GD.Print(UVX);
		return GlobalPosition.Y + noise.GetPixelv((Vector2I)pixelPos).R * HeightScale;
	}
}
