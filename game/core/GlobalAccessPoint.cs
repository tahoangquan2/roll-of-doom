using Godot;
using System;

public partial class GlobalAccessPoint : Node
{
    // hold the instance of the global access point Deck, Hand, CardManager
    
    public static CardManager cardManager;
    public static Hand hand;
    public static Deck deck;

    public override void _Ready()
    {
        Node root = GetTree().CurrentScene; 

        cardManager = root.GetNode<CardManager>("CardManager");
        hand = root.GetNode<Hand>("Hand");
        deck = root.GetNode<Deck>("Deck");
    }
}
