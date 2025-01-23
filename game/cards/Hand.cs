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
        if (card == null || hand.Contains(card)) return;
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

        for (int i = 0; i < hand.Count; i++)
        {
            Card card = hand[i];
            if (card != null)
            {
                AnimateCardTransform(card, currentAngle);
                
                // 🟢 Set Base ZIndex only if not hovered
                card.ZIndex = i + 1; // Ensure correct stacking order

                currentAngle += cardSpread;
            }
        }
    }

    private void RepositionCard(Card card,int index){
        if (hand.Count == 0) return;

        float cardSpread = Math.Min(AngleLimit / hand.Count, MaxCardSpreadAngle);
        float currentAngle = -(cardSpread * (hand.Count - 1)) / 2 - 90;

        currentAngle += cardSpread * index;

        AnimateCardTransform(card, currentAngle);

        card.ZIndex = index + 1; // Ensure correct stacking order
    }

    private Dictionary<Card, Tween> activeTweens = new Dictionary<Card, Tween>(); // Track active tweens

    private void AnimateCardTransform(Card card, float angleInDegrees)
    {
        Vector2 targetPosition = Position + GetCardPosition(angleInDegrees);
        float targetRotation = angleInDegrees + 90;

        // 🟢 Cancel existing tween if it exists
        if (activeTweens.ContainsKey(card) && activeTweens[card].IsRunning())
        {
            activeTweens[card].Kill();
        }

        // 🟢 Create a new tween
        Tween tween = GetTree().CreateTween();
        activeTweens[card] = tween; // Store it in the dictionary

        tween.TweenProperty(card, "position", targetPosition, 0.2f).SetEase(Tween.EaseType.Out);
        tween.TweenProperty(card, "rotation_degrees", targetRotation, 0.2f).SetEase(Tween.EaseType.Out);
    }



    private Vector2 GetCardPosition(float angleInDegrees)
    {
        float x = HandRadius * Mathf.Cos(Mathf.DegToRad(angleInDegrees));
        float y = HandRadius * Mathf.Sin(Mathf.DegToRad(angleInDegrees));
        return new Vector2(x, y);
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
            if (cardManager.card_being_dragged != null)
            {
                AddCard(cardManager.card_being_dragged);                
            }
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

        // 🟢 Prevent redundant animations
        if (card.Position.IsEqualApprox(targetPosition)) return;

        // 🟢 Cancel existing tween if needed
        if (activeTweens.ContainsKey(card) && activeTweens[card].IsRunning())
        {
            activeTweens[card].Kill();
        }

        // 🟢 Create a new tween and store it
        Tween tween = GetTree().CreateTween();
        activeTweens[card] = tween;

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

        // when right mouse button is pressed
        if (@event is InputEventMouseButton mouseButton2 && mouseButton2.ButtonIndex == MouseButton.Right)        {
        if (mouseButton2.Pressed)
            {
                // print all zindex
                foreach (var card in hand)
                {
                    GD.Print(card.ZIndex);
                }
            }
        }   
    }

    //catch the signal from the cardManager[Signal] public delegate void CardPushupEventHandler(Card card, int targetZIndex);

    private void _on_card_pushup(Card card,bool isHovered)
    {
        if (card == null) return;

        int index = hand.IndexOf(card);
        if (index != -1)
        {
            if (isHovered)
            {
                AnimateCardHover(card, new Vector2(0, -50).Rotated(Mathf.DegToRad(card.RotationDegrees)));
                card.ZIndex = hand.Count + 1;
            }else{
                RepositionCard(card, index);
            }
        }
    }


    //connect the signal from the cardManager [Signal] public delegate void CardPushupEventHandler(Card card, int targetZIndex);
    
    public void ConnectCardMagnagerSignals()
    {   
        cardManager.CardPushup += _on_card_pushup;
    }
}