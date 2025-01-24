using Godot;
using System.Collections.Generic;

public partial class cardEffectZone : Area2D
{
	private List<Card> Pile = new List<Card>(); // Stores all references to cards in the hand
	private Tween tween = null;
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	public virtual void activeCard(Card card)
	{
		if (card != null)
		{
			Pile.Add(card);
		}
	}

	public void _on_area_entered(Area2D area)
	{
		Card card = (Card) area.GetParent();
		if (card != null)
		{
		}		
	}

	public void _on_area_exited(Area2D area)
	{
		Card card = (Card) area.GetParent();
		if (card != null)
		{
		}
	}
}
