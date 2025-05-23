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
    private DiscardPile discardPile;
    private CardPlayZone cardActiveZone;
    private CardPileView cardPileView;

    public static string handPath, deckPath, cardManagerPath, playerPath,discardPilePath, cardActiveZonePath, cardPileViewPath;

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
        discardPilePath = "DiscardPile";
        cardActiveZonePath = "CardActiveZone";
        cardPileViewPath = "Player/CardPileView";

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
        discardPile = null;
        cardActiveZone = null;
        cardPileView = null;        

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
        discardPile = root.GetNodeOrNull<DiscardPile>(discardPilePath);
        cardActiveZone = root.GetNodeOrNull<CardPlayZone>(cardActiveZonePath);
        cardPileView = root.GetNodeOrNull<CardPileView>(cardPileViewPath);
        
        EmitSignal(nameof(ReferencesUpdated));
    }
    public static CardManager GetCardManager() => Instance?.cardManager;
    public static Hand GetHand() => Instance?.hand;
    public static Deck GetDeck() => Instance?.deck;
    public static DiscardPile GetDiscardPile() => Instance?.discardPile;
    public static CardPlayZone GetCardActiveZone() => Instance?.cardActiveZone;
    public static CardPileView GetCardPileView() => Instance?.cardPileView;
    public static Player GetPlayer() => Instance?.player;
}
