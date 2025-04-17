using Godot;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

public partial class Hand : Area2D // card are in cardmanager this is the hand just for display and interaction cards are not children of hand
{
    private int HandRadius = 800;
    private int cardRadius ;
    private float AngleLimit = 70;
    private float MaxCardSpreadAngle = 10;
    [Export] public int MaxHandSize { get; set; }=15;
     // max cards in hand
    private Godot.Collections.Array<Card> hand = new Godot.Collections.Array<Card>(); // Stores all cards
    private CardManager cardManager; // 
    private Deck deck;
    private DiscardPile discardPile;
    private PlayerStat playerStat;
    private Player player;
    private Control selectionFilter;
    private Godot.Collections.Array<Card> selectedCards = new Godot.Collections.Array<Card>();
    private CollisionShape2D collisionShape;
    private TaskCompletionSource<bool> discardCompletionSource;

    [Signal] public delegate void ActionCompletedEventHandler(Godot.Collections.Array<Card> selectedCards);
    [Signal] public delegate void ActionCancelledEventHandler();
    [Signal] public delegate void FinishedDrawingEventHandler(Godot.Collections.Array<Card> drawnCards);
    public override void _Ready() {
        collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");

        cardRadius = HandRadius-200;

        cardManager = GetTree().CurrentScene.GetNodeOrNull<CardManager>(GlobalAccessPoint.cardManagerPath);
        deck = GetTree().CurrentScene.GetNodeOrNull<Deck>(GlobalAccessPoint.deckPath);
        discardPile = GetTree().CurrentScene.GetNodeOrNull<DiscardPile>(GlobalAccessPoint.discardPilePath);
        playerStat = GlobalVariables.playerStat;
        ConnectCardMagnagerSignals();

        DrawFromDeckSimple(playerStat.cardDrawPerTurn);
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
    public Card RemoveCard(int index)    {
        if (index < 0 || index >= hand.Count)
            return null;

        Card removingCard = hand[index];
        hand.RemoveAt(index);

        RepositionCards();
        return removingCard;
    }
    public Card RemoveCard(Card card)    {
        int index = hand.IndexOf(card);
        if (index != -1)
            return RemoveCard(index);

        return null;
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
    public async Task drawFromDeck(int amount){
        Godot.Collections.Array<Card> drawnCards = await deck.DrawCards(amount);  
        amount = drawnCards.Count;
        cardManager.Lock();      

        for (int i = 0; i < amount; i++) if (drawnCards[i] != null) {     
            Card drawnCard = drawnCards[i];    
            
            if (AddCard(drawnCard)){
                cardManager.cardSound();
                await drawnCard.FlipCard(true);

                CardKeywordSystem.OnDraw(drawnCard, this);
            }
        }
        cardManager.Unlock();
        EmitSignal(nameof(FinishedDrawing), drawnCards);
    }
    public void drawFromDeckwithIndex(int index){
        Card drawnCard = deck.DrawCard(index);
        if (drawnCard != null) {AddCard(drawnCard);cardManager.cardSound();}
    }

    private async Task<bool> HandleCardSelection(EnumGlobal.PileSelectionPurpose purpose,int minSelection,int maxSelection,Action<List<Card>> handleCardActions){
        if (hand.Count < minSelection) return false;

        var dataPile = new Godot.Collections.Array<CardData>();
        foreach (Card card in hand) dataPile.Add(card.cardData);

        var selected = await GlobalAccessPoint.GetPlayer().StartSelectionMode(
            dataPile, purpose, minSelection, maxSelection
        );

        var selectedCards = new List<Card>();
        foreach (Card card in hand)
            if (selected.Contains(card.cardData))
                selectedCards.Add(card);

        handleCardActions?.Invoke(selectedCards);

        return true;
    }


    public Task<bool> StartDiscard(int min = 1, int max = 1) =>
	HandleCardSelection(EnumGlobal.PileSelectionPurpose.Discard, min, max, selected => {
		foreach (var card in selected)	{
			RemoveCard(hand.IndexOf(card));
			//_ = CardKeywordSystem.OnDiscardOrForget(card);
			card.putToDiscardPile();
		}
	});

    public Task<bool> StartForget(int min = 1, int max = 1) =>
	HandleCardSelection(EnumGlobal.PileSelectionPurpose.Forget, min, max, selected => {
		foreach (var card in selected)	{
			RemoveCard(hand.IndexOf(card));
			card.BurnCard();
		}
	});

    public void DiscardHand(){
        foreach (Card card in hand)
        {
            card.BurnCard();
        }
        hand.Clear();
    }
    public async void ShuffleFromHandtoDeck(Godot.Collections.Array<int> indexes){ // list of indexes
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
        Godot.Collections.Array<int> indexes = new Godot.Collections.Array<int>();
        for (int i = 0;i<hand.Count;i++) indexes.Add(i);
        ShuffleFromHandtoDeck(new Godot.Collections.Array<int>(indexes));
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
        if (cardManager.selected_card != null ) return;
        HandRadius = radius;cardRadius = HandRadius-200;
        if (collisionShape.Shape is CircleShape2D circle)
            circle.Radius = HandRadius;
        RepositionCards();
    }
    int requiredSelectionCount = 0;
    public int GetHandSize(){
        return hand.Count;
    }
    public async Task PutHandIntoDiscardPile(){
        Godot.Collections.Array<Card> cardsToCycle = new Godot.Collections.Array<Card>(hand);
        hand.Clear();

        Godot.Collections.Array<Card> toDiscard = new Godot.Collections.Array<Card>();
        foreach (Card card in cardsToCycle)
        {
            if (!CardKeywordSystem.OnCycle(card, this))
            {
                toDiscard.Add(card);
            }            
        }
        await discardPile.AddCards(toDiscard);
    }
    public async Task Cycle(){ //
        await PutHandIntoDiscardPile();
        await drawFromDeck(playerStat.cardDrawPerTurn);        
    }
    public void DrawFromDeckSimple(int amount)    {
        _ = drawFromDeck(amount);} // fire-and-forget    
}