using Godot;

public partial class DeckVisual : Node2D
{
    [Export] public Texture2D CardBackTexture; // Assign in Inspector
    [Export] public Vector2 offset = new Vector2(-1.0f, 2.0f); // Spacing between stacked cards
    private int currentDeckSize = 0;

    private Label CardCount => GetNode<Label>("Circle-64/CardCount");

    private Node2D Cardstack => GetNode<Node2D>("CardStack");

    public override void _Ready()
    {
        Position = new Vector2(-69, -105); // Set position
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
        for (int i = 0; i < Mathf.Min(currentDeckSize, GlobalVariables.maxStackSize); i++)
        {
            TextureRect cardBack = new TextureRect();
            cardBack.Texture = CardBackTexture;
            cardBack.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
            cardBack.Size = new Vector2(138, 210);
            cardBack.Position = new Vector2(i * offset.X, i * offset.Y);
            Cardstack.AddChild(cardBack);
        }
    }

    public void UpdateDeckVisual(int newSize)
    {
        foreach (Node child in Cardstack.GetChildren())
        {
            child.QueueFree(); // Remove old visual cards
        }

        currentDeckSize = newSize;
        CardCount.Text = currentDeckSize.ToString();
        DrawDeck();
    }

    private void ClearDeckVisual()
    {
        foreach (Node child in Cardstack.GetChildren())
        {
            child.QueueFree();
        }
    }
    public Vector2 getTopCardPosition()
    {
        int topCardIndex = Mathf.Min(currentDeckSize, GlobalVariables.maxStackSize) - 1;
        return new Vector2(topCardIndex * offset.X, topCardIndex * offset.Y)+GlobalPosition+ new Vector2(69, 105);
    }
}
