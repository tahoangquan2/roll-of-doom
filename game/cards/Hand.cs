using Godot;
using System;
using System.Collections.Generic;

public partial class Hand : Node2D // card are in cardmanager this is the hand just for display and interaction cards are not children of hand
{
    [Export] public int HandRadius { get; set; } = 500;
    [Export] public float CardAngle { get; set; } = -90;
    [Export] public float AngleLimit { get; set; } = 65;
    [Export] public float MaxCardSpreadAngle { get; set; } = 15;

    private List<Card> hand = new List<Card>(); // Stores all cards
    private CardManager cardManager;

    private CollisionShape2D collisionShape;
    private PackedScene packedSceneCard = ResourceLoader.Load("res://game/cards/card.tscn") as PackedScene;

    private int currentSelectedCardIndex = -1;

    public override void _Ready()
    {
        collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");

        // Get CardManager and its child cards
        cardManager = GetParent().GetNode<CardManager>("CardManager");
        ConnectCardMagnagerSignals();

        foreach (Node child in cardManager.GetChildren())
        {
            if (child is Card card)
            {
                AddCard(card); // Add existing cards to Hand
            }
        }

        // print hand
        foreach (var card in hand)
        {
            GD.Print(card.CardData.CardName);
        }

    }

    public void AddCard(Card card)
    {
        hand.Add(card);

        RepositionCards();
    }

    public Card RemoveCard(int index)
    {
        if (index < 0 || index >= hand.Count)
            return null;

        Card removingCard = hand[index];
        hand.RemoveAt(index);

        RepositionCards();
        return removingCard;
    }

    private void RepositionCards()
    {
        if (hand.Count == 0) return;

        float cardSpread = Math.Min(AngleLimit / hand.Count, MaxCardSpreadAngle);
        float currentAngle = -(cardSpread * (hand.Count - 1)) / 2 - 90;

        foreach (var card in hand)
        {
            if (card != null )
            {
                AnimateCardTransform(card, currentAngle);
            }
            currentAngle += cardSpread;
        }
    }

    private void AnimateCardTransform(Card card, float angleInDegrees)
    {
        Vector2 targetPosition = Position + GetCardPosition(angleInDegrees);
        float targetRotation = angleInDegrees + 90;

        var tween = GetTree().CreateTween();
        tween.TweenProperty(card, "position", targetPosition, 0.2f).SetEase(Tween.EaseType.Out);
        tween.TweenProperty(card, "rotation_degrees", targetRotation, 0.2f).SetEase(Tween.EaseType.Out);
    }


    private Vector2 GetCardPosition(float angleInDegrees)
    {
        float x = HandRadius * Mathf.Cos(Mathf.DegToRad(angleInDegrees));
        float y = HandRadius * Mathf.Sin(Mathf.DegToRad(angleInDegrees));
        return new Vector2(x, y);
    }

    private void UpdateCardTransform(Card card, float angleInDegrees)
    {
        card.Position = Position+GetCardPosition(angleInDegrees);
        card.RotationDegrees = angleInDegrees + 90;
    }

    public override void _Process(double delta)
    {
        if (collisionShape.Shape is CircleShape2D circle)
        {
            if (circle.Radius != HandRadius)
                circle.Radius = HandRadius;
        }

        if (Input.IsActionJustPressed("Action"))
        {
            GD.Print("Action input detected via Process!");
            RemoveCard(0);
        }

        if (Input.IsActionJustPressed("Action2"))
        {
            GD.Print("Action2 input detected via Process!");
            if (cardManager.card_being_dragged != null)
            {
                RemoveCard(hand.IndexOf(cardManager.card_being_dragged));
                
            }
        }
    }

    private Card hoveredCard = null;  // Track the currently hovered card
    private Card card_being_dragged = null;  // Track the card being dragged


    private void AnimateCardHover(Card card, Vector2 offset)
    {
        Vector2 targetPosition = card.Position + offset;

        if (card.Position.IsEqualApprox(targetPosition)) return;  // Skip unnecessary tweens

        var tween = GetTree().CreateTween();
        tween.TweenProperty(card, "position", targetPosition, 0.15f).SetEase(Tween.EaseType.Out);
    }

    //input event 
    public override void _Input(InputEvent @event)
    {
        // when mouse released reposotion the cards
        if (@event is InputEventMouseButton mouseButton && mouseButton.ButtonIndex == MouseButton.Left)        {
			if (mouseButton.IsReleased())
            {
                //RepositionCards();
            }
        }

        // when action input is pressed

        if (@event is InputEventAction action && action.Action == "Action" && action.Pressed)
        {
            Card card = RemoveCard(0);
            if (card != null)
            {
                cardManager.RemoveChild(card);
            }
        }        
    }

    //catch the signal from the cardManager[Signal] public delegate void CardPushupEventHandler(Card card, int targetZIndex);

    public void _on_card_pushup(Card card, bool isHovered)
    {
        if (card == null) return;

        //get the index of the card
        int index = hand.IndexOf(card);

        if (index != -1)
        {
            Vector2 offset = new Vector2(0, -50).Rotated(Mathf.DegToRad(card.RotationDegrees));
            
            AnimateCardHover(card, isHovered ? offset : -offset);

            RepositionCards(); 
        }
    }

    //connect the signal from the cardManager [Signal] public delegate void CardPushupEventHandler(Card card, int targetZIndex);
    
    public void ConnectCardMagnagerSignals()
    {   
        cardManager.CardPushup += _on_card_pushup;
    }
}