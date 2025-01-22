using Godot;
using System;

public partial class Cardslot : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Cardslot Ready");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GD.Print("Cardslot Process");
	}

	//_on_area_entered and _on_area_exited are the signals emitted by the Area2D node

	// Called when the Card enters the Area2D
	public void _on_area_entered(Area2D area)
	{
		GD.Print("Card entered slot");
		Card card = (Card) area.GetParent();
		if (card != null)
		{
			card.CurrentSlot = this;
		}		
	}

	// Called when the Card exits the Area2D
	public void _on_area_exited(Area2D area)
	{
		GD.Print("Card exited slot");
		Card card = (Card) area.GetParent();
		if (card != null)
		{
			card.CurrentSlot = null;
		}
	}
}
