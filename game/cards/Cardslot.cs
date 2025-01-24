using Godot;
using System.Collections.Generic;

public partial class Cardslot : Area2D
{
	private List<Card> Pile = new List<Card>(); // Stores all references to cards in the hand
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	public void _on_area_entered(Area2D area)
	{
		Card card = (Card) area.GetParent();
		if (card != null)
		{
			card.CurrentSlot = this;
		}		
	}

	public void _on_area_exited(Area2D area)
	{
		Card card = (Card) area.GetParent();
		if (card != null)
		{
			card.CurrentSlot = null;
			if (Pile.Contains(card)){
				Pile.Remove(card);
			}
		}
	}
}
