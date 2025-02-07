using Godot;
using System;

public partial class CardPlayZone : cardEffectZone
{
	public override void _Ready()
	{
		base._Ready();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
	}

	public override void activeCard(Card card, Vector2 actionPoint)
	{
		GD.Print($"Card {card.cardData.CardName} Played");

		// Apply all effects associated with this card
		card.ActivateEffects(this);
	}

}
