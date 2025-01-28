using Godot;
using System.Collections.Generic;

public partial class DeckVisual : Node2D
{
    [Export] public Texture2D CardBackTexture; // Assign in Inspector
    private int maxStackSize = 40;  // Max number of cards in visual stack
    [Export] public Vector2 offset = new Vector2(-2.0f, 3.0f); // Spacing between stacked cards
    private int currentDeckSize = 0;

    public override void _Ready()
    {
        if (GetParent() is Deck deck)
        {
            deck.Connect(nameof(Deck.DeckUpdated), Callable.From((int size) => UpdateDeckVisual(size)));
            deck.Connect(nameof(Deck.DeckEmpty), Callable.From(() => ClearDeckVisual()));
            currentDeckSize = deck.GetDeckSize();
            DrawDeck();
        }
    }

    private void DrawDeck()
    {
        for (int i = 0; i < Mathf.Min(currentDeckSize, maxStackSize); i++)
        {
            TextureRect cardBack = new TextureRect();
            cardBack.Texture = CardBackTexture;
            cardBack.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
            cardBack.Size = new Vector2(138, 210);
            cardBack.Position = new Vector2(i * offset.X, i * offset.Y);
            AddChild(cardBack);
        }
    }

    public void UpdateDeckVisual(int newSize)
    {
        foreach (Node child in GetChildren())
        {
            child.QueueFree(); // Remove old visual cards
        }

        currentDeckSize = newSize;
        DrawDeck();
    }

    private void ClearDeckVisual()
    {
        foreach (Node child in GetChildren())
        {
            child.QueueFree();
        }
    }
}
