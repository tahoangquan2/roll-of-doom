using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class Hand : Area2D // card are in cardmanager this is the hand just for display and interaction cards are not children of hand
{
    private int HandRadius = 750;
    private int cardRadius ;
    private float AngleLimit = 65;
    private float MaxCardSpreadAngle = 10;
    [Export] public int MaxHandSize { get; set; }=10;
     // max cards in hand
    private Godot.Collections.Array<Card> hand = new Godot.Collections.Array<Card>(); // Stores all cards
    private CardManager cardManager; // 
    private Deck deck;
    private Control selectionFilter;
    private Godot.Collections.Array<Card> selectedCards = new Godot.Collections.Array<Card>();
    public bool isSelecting = false;
    private CollisionShape2D collisionShape;
    private Dictionary<Card, Tween> activeTweens = new Dictionary<Card, Tween>(); 

    [Signal] public delegate void SelectionCompletedEventHandler(Godot.Collections.Array<Card> selectedCards);
    [Signal] public delegate void SelectionCancelledEventHandler();
    public override void _Ready()
    {
        collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        selectionFilter = GetNode<Control>("SelectionFilter");

        // Get CardManager and its child cards
        cardManager = GlobalAccessPoint.GetCardManager();
        deck = GlobalAccessPoint.GetDeck();
        ConnectCardMagnagerSignals();
        cardRadius = HandRadius-200;
    }
    public void AddCard(Card card)
    {
        if (card == null || hand.Contains(card)) return;
        if (hand.Count >= MaxHandSize) 
        {
            card.KillCard();
            return;
        }
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
        float tweenDuration = 0.25f; // Duration of the tween

        TransformCard(card, targetPosition, targetRotation, tweenDuration);
    }
    private void AnimateCardHover(Card card)
    {
        float angle = card.Rotation;
        Vector2 targetPosition = Position + GetCardPosition(Mathf.RadToDeg(angle)-90) + new Vector2(0,-50).Rotated(angle);

        TransformCard(card, targetPosition, angle, 0.15f);
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
        Godot.Collections.Array<Card> drawnCards = deck.DrawCards(amount);        
        // slow down beetwen each card
        for (int i = 0; i < amount; i++) if (drawnCards[i] != null) {
            AddCard(drawnCards[i]);
            await ToSignal(GetTree().CreateTimer(0.7f), "timeout");
        }
    }
    public void SelectCard(Card card)    {
        if (!isSelecting) return;

        if (selectedCards.Contains(card))
        {
            selectedCards.Remove(card);
            _on_card_pushup(card,false);
        }
        else if (selectedCards.Count < requiredSelectionCount)
        {
            selectedCards.Add(card);
            _on_card_pushup(card,true);
        }

        if (selectedCards.Count == requiredSelectionCount)
        {
            ApplySelectionEffect();
        }
    }
    private void ApplySelectionEffect()    {
        foreach (Card card in selectedCards)
        {
            if (currentPurpose == EnumGlobal.HandSelectionPurpose.Discard)
            {
                RemoveCard(hand.IndexOf(card));
                card.KillCard();
            }
        }
        selectedCards.Clear();
        EmitSignal(nameof(SelectionCompleted), selectedCards);        
        ExitSelectionMode();
    }
    public void DiscardHand(){
        foreach (Card card in hand)
        {
            card.KillCard();
        }
        hand.Clear();
    }
    public async void ShuffleFromHandtoDeck(Godot.Collections.Array<int> indexes){ // list of indexes
        if (isSelecting) return;
        cardManager.islocked = true;
        foreach (int index in indexes){
            if (index >= 0 && index < hand.Count){
                deck.ShuffleIntoDeck(hand[index].cardData);
            }
        }
        for (int i = indexes.Count - 1; i >= 0; i--){
            int index = indexes[i];
            GD.Print(index);
            if (index >= 0 && index < hand.Count){
                Card card = hand[index];
                card.canBeHovered = false;
                
                TransformCard(card, deck.Position, 0, 0.15f);
                await card.FlipCard(false);

                RemoveCard(index);
                await ToSignal(GetTree().CreateTimer(0.15f), "timeout");
                card.obliterateCard();
            }
        }
        cardManager.islocked = false;
    }
    private void TransformCard(Card card, Vector2 targetPosition, float targetRotation, float duration){
        if (activeTweens.TryGetValue(card, out Tween existingTween) && existingTween.IsRunning()) existingTween.Kill();

        Tween tween = GetTree().CreateTween();
        activeTweens[card] = tween;

        if (!card.Position.IsEqualApprox(targetPosition)) 
            tween.TweenProperty(card, "position", targetPosition, duration).SetEase(Tween.EaseType.Out);

        if (card.Rotation != targetRotation)
            tween.TweenProperty(card, "rotation_degrees", targetRotation, duration).SetEase(Tween.EaseType.Out);
    }
    public void ShuffleHandtoDeck(){ 
        if (isSelecting) return;

        Godot.Collections.Array<int> indexes = new Godot.Collections.Array<int>();
        for (int i = 0;i<hand.Count;i++) indexes.Add(i);
        ShuffleFromHandtoDeck(new Godot.Collections.Array<int>(indexes));
    }
    public override void _Process(double delta)    {
        if (collisionShape.Shape is CircleShape2D circle){ // this for the hand circle shape
            if (circle.Radius != HandRadius){
                circle.Radius = HandRadius;
            }
        }
    }
    private void _on_card_pushup(Card card,bool isHovered)    {
        if (card == null) return;

        int index = hand.IndexOf(card);
        if (index != -1){
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
    {   setHandRadius(900);}
    public void _on_mouse_exited()
    {   setHandRadius(750);}    
    private void setHandRadius(int radius){
        if (cardManager.selected_card != null || isSelecting) return;
        HandRadius = radius;cardRadius = HandRadius-200;
        RepositionCards();
    }
    private bool StartSelectionMode(int numToSelect, EnumGlobal.HandSelectionPurpose purpose){
        if (hand.Count < numToSelect) return false;
        selectionFilter.Visible = true;

        cardManager.selected_card = null;
        setHandRadius(900);

        isSelecting= true;
        requiredSelectionCount = numToSelect;
        currentPurpose = purpose;
        selectedCards.Clear();
        return true;
    }
    private void ExitSelectionMode(){
        selectionFilter.Visible = false;
        isSelecting = false;
        selectedCards.Clear();
        currentPurpose = EnumGlobal.HandSelectionPurpose.None;
        EmitSignal(nameof(SelectionCancelled));
    }
    public bool startDiscard(int numToSelect){
        return StartSelectionMode(numToSelect, EnumGlobal.HandSelectionPurpose.Discard);
    }
    int requiredSelectionCount = 0;
    EnumGlobal.HandSelectionPurpose currentPurpose = EnumGlobal.HandSelectionPurpose.None;
    public void _input(InputEvent @event){//action "Action" from input map, this is for testing
        if (@event.IsActionPressed("Action"))
        {
            drawFromDeck(3);
        }

        if (@event.IsActionPressed("Action2"))
        {
            ShuffleHandtoDeck();
        }
    }
    public void _on_button_pressed(){
        ExitSelectionMode();
        setHandRadius(750);
    }
}