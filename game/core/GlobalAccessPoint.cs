using Godot;
using System;

public partial class GlobalAccessPoint : Node
{
    public static GlobalAccessPoint Instance { get; private set; }

    private CardManager cardManager;
    private Hand hand;
    private Deck deck;

    public override void _Ready()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            QueueFree();
            return;
        }

        // Get root scene
        Node root = GetTree().CurrentScene;

        // Ensure nodes exist before assignment
        cardManager = root.GetNodeOrNull<CardManager>("CardManager");
        hand = root.GetNodeOrNull<Hand>("Hand");
        deck = root.GetNodeOrNull<Deck>("Deck");

        if (cardManager == null) GD.PrintErr(" GlobalAccessPoint: CardManager not found!");
        if (hand == null) GD.PrintErr(" GlobalAccessPoint: Hand not found!");
        if (deck == null) GD.PrintErr(" GlobalAccessPoint: Deck not found!");
    }

    // Public getters for GDScript
    public static CardManager GetCardManager() => Instance?.cardManager;
    public static Hand GetHand() => Instance?.hand;
    public static Deck GetDeck() => Instance?.deck;
}
