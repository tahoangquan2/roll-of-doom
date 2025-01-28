using Godot;
using System;
using System.Collections.Generic;

public partial class Hand : Area2D // card are in cardmanager this is the hand just for display and interaction cards are not children of hand
{
    private int HandRadius = 750;
    private int cardRadius ;
    [Export] public float AngleLimit { get; set; } = 65;
    [Export] public float MaxCardSpreadAngle { get; set; } = 15;

    private List<Card> hand = new List<Card>(); // Stores all cards
    private CardManager cardManager; // 

    private Deck deck;
    private CollisionShape2D collisionShape;
    private int currentSelectedCardIndex = -1;
    private Dictionary<Card, Tween> activeTweens = new Dictionary<Card, Tween>(); // Track active tweens

    public override void _Ready()
    {
        collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");

        // Get CardManager and its child cards
        cardManager = GetParent().GetNode<CardManager>("CardManager");
        deck = GetParent().GetNode<Deck>("Deck");
        ConnectCardMagnagerSignals();
        cardRadius = HandRadius-200;
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

        for (int i = 0; i < hand.Count; i++)
        {
            Card card = hand[i];
            if (card != null && card != cardManager.card_being_dragged)
            {
                AnimateCardTransform(card, GetCardAngle(i, hand.Count));
                card.ZIndex = i + 1;
            }
        }
    }
    private void RepositionCard(Card card, int index)
    {
        if (hand.Count == 0) return;

        AnimateCardTransform(card, GetCardAngle(index, hand.Count));
        card.ZIndex = index + 1; 
    }

    private void AnimateCardTransform(Card card, float angleInDegrees)
    {
        Vector2 targetPosition = Position + GetCardPosition(angleInDegrees);
        float targetRotation = angleInDegrees + 90;
        float tweenDuration = 0.2f; // Duration of the tween

        if (activeTweens.TryGetValue(card, out Tween existingTween) && existingTween.IsRunning())
        {
            existingTween.Kill();
        }

        Tween tween = GetTree().CreateTween();
        activeTweens[card] = tween; // Store it in the dictionary

        // Animate movement & rotation
        tween.TweenProperty(card, "position", targetPosition, tweenDuration).SetEase(Tween.EaseType.Out);
        tween.TweenProperty(card, "rotation_degrees", targetRotation, tweenDuration).SetEase(Tween.EaseType.Out);
    }
    private void AnimateCardHover(Card card)
    {
        float angle = card.Rotation;
        Vector2 targetPosition = Position + GetCardPosition(Mathf.RadToDeg(angle)-90) + new Vector2(0,-50).Rotated(angle);

        if (card.Position.IsEqualApprox(targetPosition)) return;

        if (activeTweens.ContainsKey(card) && activeTweens[card].IsRunning()) activeTweens[card].Kill();

        Tween tween = GetTree().CreateTween();
        activeTweens[card] = tween;

        tween.TweenProperty(card, "position", targetPosition, 0.15f).SetEase(Tween.EaseType.Out);
    }

    private Vector2 GetCardPosition(float angleInDegrees){
        return new Vector2(0,-cardRadius).Rotated(Mathf.DegToRad(angleInDegrees+90));
    }
    private float GetCardAngle(int index, int totalCards)
    {
        if (totalCards == 0) return -90;
        float cardSpread = Math.Min(AngleLimit / totalCards, MaxCardSpreadAngle);
        return -(cardSpread * (totalCards - 1)) / 2 - 90 + (cardSpread * index);
    }

    public async void drawFromDeck(int amount){
        if (deck == null) return;
        Godot.Collections.Array<Card> drawnCards = deck.DrawCards(amount);

        GD.Print(drawnCards);
        
        // slow down beetwen each card
        for (int i = 0; i < amount; i++) if (drawnCards[i] != null) 
        {
            GD.Print("Drawing card");
            if (hand.Count >= 10) break;
            AddCard(drawnCards[i]);
            await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
        }

    }

    public override void _Process(double delta)
    {
        if (collisionShape.Shape is CircleShape2D circle){
            if (circle.Radius != HandRadius){
                circle.Radius = HandRadius;
            }
        }
    }
    private void _on_card_pushup(Card card,bool isHovered)
    {
        if (card == null) return;

        int index = hand.IndexOf(card);
        if (index != -1)
        {
            if (isHovered)
            {
                AnimateCardHover(card);
                card.ZIndex = hand.Count + 1;
            }else{
                RepositionCard(card, index);
            }
        }
    }
    private void _on_card_unhand(Card card){
        int index = hand.IndexOf(card);
        if (index != -1) RemoveCard(index);
    }
    public void ConnectCardMagnagerSignals()
    {   cardManager.CardPushup += _on_card_pushup;cardManager.CardUnhand += _on_card_unhand;}
    public void _on_mouse_entered()
    {   if (cardManager.selected_card != null) return;
        HandRadius = 900;cardRadius = HandRadius-200;RepositionCards();
    }
    public void _on_mouse_exited()
    {   if (cardManager.selected_card != null) return;
        HandRadius = 750;cardRadius = HandRadius-200;RepositionCards();
    }    

    public void _input(InputEvent @event)
    {//action "Action" from input map
        if (@event.IsActionPressed("Action"))
        {
            drawFromDeck(3);
        }

        if (@event.IsActionPressed("Action2"))
        {
            Card card = RemoveCard(0);
            if (card != null) card.QueueFree();
        }
    }
}