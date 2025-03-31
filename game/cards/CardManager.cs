using System;
using System.Collections.Generic;
using Godot;

public partial class CardManager : Node2D
{
	//public Card card_being_dragged = null;
	public Card selected_card = null;
	private AudioStreamPlayer2D audioPlayer;	
    private PackedScene cardScene = GD.Load<PackedScene>("res://game/cards/Card.tscn");
	private Hand hand=null;
	private bool Locked =false; // if true, no card can be selected or dragged
	[Signal] public delegate void CardPushupEventHandler(Card card,bool isHovered);
	[Signal] public delegate void CardUnhandEventHandler(Card card);
	[Signal] public delegate void CardSelectEventHandler(CardData card);
	public CardState.State currentCardState = CardState.State.Idle;
	private Dictionary<CardState.State, CardState> cardStates = new Dictionary<CardState.State, CardState>();
	public override void _Input(InputEvent @event)
	{	if (Locked) return;
		if (hand is not null && hand.isSelecting) {
			HandleSelectionInput(@event);
			return;			
		}		

		cardStates[currentCardState].HandleInput(@event);	
		GetNode<Label>("Label").Text = currentCardState.ToString();	
	}
	public void StateChangeRequest(CardState.State from,CardState.State to,Card card=null)
	{
		if (from != currentCardState) 
		{
			GD.Print("StateChangeRequest: "+from+" != "+currentCardState);
			return;}

		cardStates[from].ExitState(card);
		currentCardState = to;	
		cardStates[to].EnterState(card);			
	}
	private void HandleSelectionInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
			{
				Card card = CardState.RaycastCheckForCard();
				if (card != null) hand.SelectCard(card);
			}
		}
	}
	public override void _Ready() 
	{	audioPlayer = GetNode<AudioStreamPlayer2D>("AudioPlayer");
		hand = GetTree().CurrentScene.GetNodeOrNull<Hand>(GlobalAccessPoint.handPath);

		foreach (Node a in GetChildren()){
			if (a is CardState cardState) cardStates.Add(cardState.cardState,cardState);			
		}

		currentCardState = CardState.State.Idle;
	}
	public void SelectCard(Card card) {
		if (selected_card == card) {
			DeselectCard();
			return;
		};
		DeselectCard();
		selected_card = card;
		EmitSignal(nameof(CardPushup), card, true);	
	}
	public void DeselectCard()
	{
		if (selected_card != null){
			selected_card.ResetShader();
			//if (card_being_dragged != selected_card) 
			EmitSignal(nameof(CardPushup), selected_card, false);	
			//if (card_being_hovered != selected_card) selected_card.Scale = new Vector2(1,1);
			selected_card = null;			
		}
	}
	public void StartDrag(Card card)
	{
		DeselectCard();
		card.Scale = new Vector2(1.05f, 1.05f);
		card.Rotation = 0;
		card.ZIndex = 11;
		card.ResetShader();
	}
	public void EndDrag()
	{
		Card tmpCard = CardState.card;
		
		CardPlayZone zone = CardState.RaycastCheckForZone(tmpCard.GetCardData().TargetMask);
		if (zone != null){
			if (GlobalVariables.spirit >= tmpCard.GetCardData().Cost)
			{
				EmitSignal(nameof(CardUnhand), tmpCard);
				tmpCard.ResetShader();
				zone.activeCard(tmpCard,GetGlobalMousePosition()-zone.GlobalPosition);

				//GD.Print("cost: "+tmpCard.GetCardData().Cost);
				GlobalVariables.ChangeSpirit(-tmpCard.GetCardData().Cost);
				//GD.Print("spirit: "+GlobalVariables.spirit);
			}			
		}		
	}
	public Card createCard(CardData cardData)
	{	
		Card newCard = (Card)cardScene.Instantiate();
        AddChild(newCard);
        newCard.SetupCard(cardData);
		return newCard;
	}
	public void checkChange(Card card) {
		if (selected_card == card) selected_card = null;
		if (card == CardState.card) CardState.card = null;
	}
	public void cardSound() {audioPlayer.Play();}
	public void Lock() {Locked = true;GD.Print("Locked");}
	public void Unlock() {Locked = false;}
	public bool IsLocked() {return Locked;}
	
	// signals for card and playzone
	public void ConnectCardSignals(Card card)
	{card.CardHovered += _on_card_hovered;   card.CardUnhovered += _on_card_unhovered;}
	public void _on_card_hovered(Card card)
	{	if (Locked) return;
		cardStates[currentCardState]._on_card_hovered(card);
	}
	public void _on_card_unhovered(Card card)
	{	if (Locked) return;
		cardStates[currentCardState]._on_card_unhovered(card);
	}
	
	public void ConnectPlayZoneSignals(CardPlayZone zone)
	{zone.ZoneUpdate += _on_zone_update;  }
	public void _on_zone_update(bool isEntered, CardPlayZone zone)
	{cardStates[currentCardState]._on_zone_update(isEntered, zone);}
}