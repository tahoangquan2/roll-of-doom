using Godot;
using System;

public partial class Hand : Area2D // card are in cardmanager this is the hand just for display and interaction cards are not children of hand
{
    private int HandRadius = 800;
    private int cardRadius ;
    private float AngleLimit = 70;
    private float MaxCardSpreadAngle = 10;
    [Export] public int MaxHandSize { get; set; }=10;
     // max cards in hand
    private Godot.Collections.Array<Card> hand = new Godot.Collections.Array<Card>(); // Stores all cards
    private CardManager cardManager; // 
    private Deck deck;
    private Player player;
    private Control selectionFilter;
    private Godot.Collections.Array<Card> selectedCards = new Godot.Collections.Array<Card>();
    public bool isSelecting = false;
    private CollisionShape2D collisionShape;

    [Signal] public delegate void ActionCompletedEventHandler(Godot.Collections.Array<Card> selectedCards);
    [Signal] public delegate void ActionCancelledEventHandler();
    [Signal] public delegate void FinishedDrawingEventHandler(Godot.Collections.Array<Card> drawnCards);

    EnumGlobal.HandSelectionPurpose currentPurpose = EnumGlobal.HandSelectionPurpose.None;
    public override void _Ready() {
        collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        selectionFilter = GetNode<Control>("SelectionFilter");

        RemoveChild(selectionFilter);

        Node Parent = GetParent();
        Parent.CallDeferred("add_child", selectionFilter);
        Parent.CallDeferred("move_child", selectionFilter, 1);

        cardRadius = HandRadius-200;

        cardManager = GetTree().CurrentScene.GetNodeOrNull<CardManager>(GlobalAccessPoint.cardManagerPath);
        deck = GetTree().CurrentScene.GetNodeOrNull<Deck>(GlobalAccessPoint.deckPath);
        ConnectCardMagnagerSignals();

        for (int i=0;i<cardManager.GetChildCount()-1;i++){
            if (cardManager.GetChild(i) is Card card){
                AddCard(card);
            }
        }
    }
    public bool AddCard(Card card)
    {
        if (card == null || hand.Contains(card)) return false;
        if (hand.Count >= MaxHandSize) 
        {
            card.BurnCard();
            return false;
        }
        hand.Add(card);

        RepositionCards();
        return true;
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
            if (card != null  && card != CardState.card)
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
        Vector2 targetPosition = GlobalPosition + GetCardPosition(angleInDegrees);
        float targetRotation = angleInDegrees + 90;
        float tweenDuration = 0.25f; // Duration of the tween

        card.TransformCard( targetPosition, targetRotation, tweenDuration);
    }
    private void AnimateCardHover(Card card)
    {
        float angle = card.Rotation;
        Vector2 targetPosition = GlobalPosition + GetCardPosition(Mathf.RadToDeg(angle)-90) + new Vector2(0,-50).Rotated(angle);

        card.TransformCard(targetPosition, Mathf.RadToDeg(angle), 0.15f);
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
        amount = drawnCards.Count;
        cardManager.Lock();      
        for (int i = 0; i < amount; i++) if (drawnCards[i] != null) {            
            
            if (AddCard(drawnCards[i])){
                cardManager.cardSound();
                await drawnCards[i].FlipCard(true);
            }
        }
        cardManager.Unlock();
        EmitSignal(nameof(FinishedDrawing), drawnCards);
    }
    public void drawFromDeckwithIndex(int index){
        Card drawnCard = deck.DrawCard(index);
        if (drawnCard != null) {AddCard(drawnCard);cardManager.cardSound();}
    }
    public void SelectCard(Card card)    {
        if (!isSelecting) return;

        if (card == null || !hand.Contains(card)) return;

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
        if (currentPurpose == EnumGlobal.HandSelectionPurpose.Discard)
        {
            foreach (Card card in selectedCards)
            {
                RemoveCard(hand.IndexOf(card));
                card.BurnCard();
            }
            selectedCards.Clear();
        } else
        if (currentPurpose == EnumGlobal.HandSelectionPurpose.Duplicate)
        {
            foreach (Card card in selectedCards)
            {
                Card duplicate = cardManager.createCard(card.cardData);
                AddCard(duplicate);
            }
        }
        
        EmitSignal(nameof(ActionCompleted), selectedCards);   
        selectedCards.Clear();
        ExitSelectionMode();
    }
    public void DiscardHand(){
        foreach (Card card in hand)
        {
            card.BurnCard();
        }
        hand.Clear();
    }
    public async void ShuffleFromHandtoDeck(Godot.Collections.Array<int> indexes){ // list of indexes
        if (isSelecting) return;
        foreach (int index in indexes){
            if (index >= 0 && index < hand.Count){
                deck.ShuffleIntoDeck(hand[index].cardData);
            }
        }
        
        Vector2 deckPosition = deck.GlobalPosition + Mathf.Min(GlobalVariables.maxStackSize, deck.GetDeckSize()) * new Vector2(-2.0f, 3.0f);

        for (int i = indexes.Count - 1; i >= 0; i--){
            int index = indexes[i];
            GD.Print(index);
            if (index >= 0 && index < hand.Count){
                Card card = hand[index];
                card.canBeHovered = false;
                
                card.TransformCard(deckPosition, 0, 0.15f);                

                RemoveCard(index);
                await card.FlipCard(false);
                card.obliterateCard();
            }
        }
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
    {   setHandRadius(800);}    
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
        EmitSignal(nameof(ActionCancelled));
    }
    public bool startDiscard(int numToSelect){
        return StartSelectionMode(numToSelect, EnumGlobal.HandSelectionPurpose.Discard);
    }
    int requiredSelectionCount = 0;
    public void _input(InputEvent @event){//action "Action" from input map, this is for testing
    if (@event is InputEventMouseMotion) return;
        if (@event.IsActionPressed("Action"))
        {
            drawFromDeck(3);
        }

        if (@event.IsActionPressed("Action2"))
        {
        }
    }
    public void _on_button_pressed(){
        ExitSelectionMode();
        setHandRadius(750);
    }
    public int GetHandSize(){
        return hand.Count;
    }
    public Godot.Collections.Array<Card> GetHand(){
        return hand;
    }
}