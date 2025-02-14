using System;
using Godot;

public partial class GlobalAccessPoint : Node
{
    public static GlobalAccessPoint Instance { get; private set; }

    private CardManager cardManager;
    private Hand hand;
    private Deck deck;
    private Player player;

    public static String handPath, deckPath, cardManagerPath, playerPath;

    [Signal] public delegate void ReferencesUpdatedEventHandler();

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

        handPath = "Hand";
        deckPath = "Deck";
        cardManagerPath = "CardManager";
        playerPath = "Player";

        UpdateReferences();
    }

    public void UpdateReferences()
    {
        GD.Print("Updating GlobalAccessPoint references...");

        // Prevent stale references
        cardManager = null;
        hand = null;
        deck = null;
        player = null;

        Node root = GetTree().CurrentScene;
        if (root == null)
        {
            GD.PrintErr("No current scene found! Skipping node update.");
            return;
        }

        // Assign new references
        cardManager = root.GetNodeOrNull<CardManager>(cardManagerPath);
        hand = root.GetNodeOrNull<Hand>(handPath);
        deck = root.GetNodeOrNull<Deck>(deckPath);
        player = root.GetNodeOrNull<Player>(playerPath);

        if (cardManager == null) GD.PrintErr("GlobalAccessPoint: CardManager not found!");
        if (hand == null) GD.PrintErr("GlobalAccessPoint: Hand not found!");
        if (deck == null) GD.PrintErr("GlobalAccessPoint: Deck not found!");
        if (player == null) GD.PrintErr("GlobalAccessPoint: Player not found!");
        EmitSignal(nameof(ReferencesUpdated));
    }
    public static CardManager GetCardManager() => Instance?.cardManager;
    public static Hand GetHand() => Instance?.hand;
    public static Deck GetDeck() => Instance?.deck;
}
