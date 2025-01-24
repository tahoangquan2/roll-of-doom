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

    public override void activeCard(Card card)
    {
        GD.Print("Card Played");
        card.GetParent().RemoveChild(card);
        card.QueueFree();
    }
}
