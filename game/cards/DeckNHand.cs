using Godot;
using System;

public partial class DeckNHand : Node2D
{
	PackedScene packedScene_card = ResourceLoader.Load("res://game/cards/card.tscn") as PackedScene;

	Node2D spawnPoint;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		spawnPoint = GetNode<Node2D>("CanvasLayer/Spawn");
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButton && mouseButton.ButtonIndex == MouseButton.Left)        {
			if (mouseButton.Pressed)
			{
				GD.Print("Mouse button pressed");
			}
			else
			{
			}
		}

		if (@event.IsActionPressed("Action"))
		{
			GD.Print("Action Key Pressed");
			var card = packedScene_card.Instantiate() as Card;
			AddChild(card);
			
			card.Position = spawnPoint.Position;
		}

		if (@event.IsActionPressed("Action2"))
		{
			GD.Print("W Key Pressed");
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
