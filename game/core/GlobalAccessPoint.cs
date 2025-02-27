using System;
using Godot;

public partial class GlobalAccessPoint : Node
{
    public static GlobalAccessPoint Instance { get; private set; }

    private EnumGlobal.State gameState = EnumGlobal.State.GamePlay;

    private CardManager cardManager;
    private Hand hand;
    private Deck deck;
    private Player player;

    public static string handPath, deckPath, cardManagerPath, playerPath, gamePlayPath;

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

        handPath = "Player/MainControl/HandControl/Hand";
        deckPath = "Player/MainControl/DeckControl/Deck";
        cardManagerPath = "CardManager";
        playerPath = "Player";
        gamePlayPath = "GamePlay";

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
        player = root.GetNodeOrNull<Player>(playerPath);
        deck = root.GetNodeOrNull<Deck>(deckPath);
        
        EmitSignal(nameof(ReferencesUpdated));
    }
    public static CardManager GetCardManager() => Instance?.cardManager;
    public static Hand GetHand() => Instance?.hand;
    public static Deck GetDeck() => Instance?.deck;
}
