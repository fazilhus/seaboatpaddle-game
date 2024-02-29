using Godot;
using System;
using System.IO;
using System.Numerics;

public partial class text_box : CanvasLayer
{
	// Called when the node enters the scene tree for the first time.
	[Export] public MarginContainer textBoxContainer;
	[Export] public Label label;
	[Export] public Sprite2D spriteButton;
	public Boat boat;
	public bool isAdding;
	//public int i;
	//public Vector<string> plainTextArray;
	private StreamReader sr;
	public override void _Ready()
	{
		textBoxContainer = GetNode<MarginContainer>("TextBoxContainer");
		label = GetChild(0).GetChild(1).GetNode<Label>("Label");
		spriteButton = GetChild(0).GetChild(1).GetNode<Sprite2D>("XboxButtonColorB");
		boat = GetParent().GetNode<Boat>("Boat");
		//i = 0;
		//plainTextArray = new Vector<string>();
		//AddText(plainTextArray[i]);
		sr = new StreamReader("textDocuments\\dialog_1.txt");
		spriteButton.Hide();
		label.VisibleRatio = 0.0f;
		boat.isReadyPaddle = false;

		GetParent().GetNode<CanvasLayer>("GameCamera/CanvasLayer").Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	// 	if(isAdding && label.VisibleRatio <= 1.0f)
	// 	{
			
	// 		label.VisibleRatio += 0.5f*(float)delta;
	// 		if( label.VisibleRatio >= 1.0f)
	// 		{
	// 			spriteButton.Show();
	// 			if(Input.IsActionJustPressed("ui_accept"))
	// 			{
	// 				label.VisibleRatio = 0.0f;
	// 				AddText(plainTextArray[++i]);
	// 				GD.Print(i);
	// 				if(plainTextArray[i] == plainTextArray[plainTextArray.Length-1])
	// 				{
	// 					isAdding = false;
	// 					HideTextBox();
	// 				}
	// 			}
	// 		}
	// 	}

		if (!isAdding && !sr.EndOfStream) {
			AddText(sr.ReadLine());
		}

		if(isAdding && label.VisibleRatio <= 1.0f)
		{
			
			label.VisibleRatio += 0.5f*(float)delta;
			if( label.VisibleRatio >= 1.0f)
			{
				spriteButton.Show();
				if(Input.IsActionJustPressed("ui_accept"))
				{
					label.VisibleRatio = 0.0f;
					if (!sr.EndOfStream) {
						AddText(sr.ReadLine());
					}
					//GD.Print(i);
					if(sr.EndOfStream)
					{
						isAdding = false;
						HideTextBox();
					}
				}
			}
		}
		/*if(Input.IsActionPressed("ui_accept")) 
		{
			if( label.VisibleRatio >= 1.0f)
			{
				spriteButton.Show();
				if(Input.IsActionJustPressed("ui_accept"))
				{
					label.VisibleRatio = 0.0f;
					AddText(plainTextArray[++i]);
					GD.Print(i);
					if(plainTextArray[i] == plainTextArray[plainTextArray.Length-1])
					{
						HideTextBox();
					}
				}
				

			}
			label.VisibleRatio = 1.0f;
		}*/
	}

	public void HideTextBox()
	{
		GD.Print("I've been called");
		label.Text = "";
		textBoxContainer.Hide();
		spriteButton.Hide();
		boat.isReadyPaddle = true;
		GetParent().GetNode<GameCamera>("GameCamera").StartCountdownTimer();
		GetParent().GetNode<CanvasLayer>("GameCamera/CanvasLayer").Show();
	}

	public void ShowTextBox()
	{
		textBoxContainer.Show();
	}
	public void AddText(string newText)
	{
		spriteButton.Hide();
		label.Text = newText;
		label.VisibleRatio = 0;
		isAdding = true;
		
	}

	/*public void scanf()
	{
		try
		{
			var scan = new StreamReader("textDocuments\\dialog_1.txt");
			string line;
			while((line = scan.ReadLine()) != null)
			{
				Console.WriteLine(line);
				
				GD.Print(line);
				
			}
			scan.Close();
			Console.ReadLine();
		}
		catch(Exception e)
		{
			GD.Print("exception: " + e.Message);
		}
		
	}*/
}
