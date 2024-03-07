using Godot;
using System;
using System.IO;

public partial class text_box : CanvasLayer
{
	// Called when the node enters the scene tree for the first time.
	[Export] public MarginContainer textBoxContainer;
	[Export] public Label label;
	[Export] public Sprite2D spriteButton;
	public Boat boat;
	public bool isAdding;
	private StreamReader sr;
	private string contents;
	private string[] lines;
	private int idx;
	private AudioStreamPlayer adventure;
	public override void _Ready()
	{
		textBoxContainer = GetNode<MarginContainer>("TextBoxContainer");
		label = GetChild(0).GetChild(1).GetNode<Label>("Label");
		spriteButton = GetChild(0).GetChild(1).GetNode<Sprite2D>("XboxButtonColorB");
		boat = GetParent().GetNode<Boat>("Boat");
		adventure = GetParent().GetNode<AudioStreamPlayer>("adventure");

		//i = 0;
		//plainTextArray = new Vector<string>();
		//AddText(plainTextArray[i]);
		//sr = new StreamReader("textDocuments\\dialog_1.txt");
		contents = @"Captain: (groaning, slowly regaining consciousness) What... What happened?
First Mate: Captain! You're awake! Thank the seas, we were worried sick about you.
Captain: (trying to sit up, wincing) The storm... Did we make it through?
First Mate: Aye, Captain. But it was a close call. The Ship took quite a beating.
Captain: (looking around, noticing the crew's worried faces) Why the long faces, lads? What's happened?
First Mate: It's the cargo, Captain. The storm tossed us about like a toy boat. The hold's taking on water, and some of the crates are already slipping.
Captain: (eyes widening) Blast it all! We can't lose our goods. We need to salvage what we can before they sink to the abyss. Sound the alarm! Everyone to their stations! We've got work to do!
First Mate: Aye, Captain! You heard him, lads! Move like the wind and bring up whatever you can salvage!
Crew:(shouting) AYE AYE!!!";
		lines = contents.Split("\n");
		idx=0;
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
		if (Input.IsKeyPressed(Key.F5)) {
			HideTextBox();
		}

		if (!isAdding && (idx < lines.Length)) {
			AddText(lines[idx]);
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
					idx++;
					if (idx < lines.Length) {
						AddText(lines[idx]);
					}
					else 
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
		adventure.Playing = true;
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
