using Godot;

public partial class CardManager : Node2D
{
	public Card card_being_dragged = null;
	public Card card_being_hovered = null;
	public Card selected_card = null;
	private bool isProcessingHover = false;
	private AudioStreamPlayer2D audioPlayer;	
    private PackedScene cardScene=null;	
	private Hand hand=null;
	private bool Locked =false; // if true, no card can be selected or dragged
	[Signal] public delegate void CardPushupEventHandler(Card card,bool isHovered);
	[Signal] public delegate void CardUnhandEventHandler(Card card);
	[Signal] public delegate void CardSelectEventHandler(CardData card);
	public override void _Input(InputEvent @event)
	{	if (Locked) return;
		if (hand is not null && hand.isSelecting) {
			HandleSelectionInput(@event, hand);
			return;			
		}		

		if (@event is InputEventMouseButton mouseButton)        {
			if (mouseButton.ButtonIndex == MouseButton.Left)
			{
				if (mouseButton.Pressed) {	
					if (selected_card != null) {	
						Card tmpCard = selected_card;
						card_being_dragged = tmpCard;
						EndDrag();
						DeselectCard();																				
					}				

					Card card = RaycastCheckForCard();
					if (card != null){
						StartDrag(card);
					}
				}else{
					EndDrag();
				}
			} else if (mouseButton.ButtonIndex == MouseButton.Right && mouseButton.Pressed)
			{
				Card card = RaycastCheckForCard();

				if (card != null) SelectCard(card); else DeselectCard();
			}
		}

		if (@event is InputEventMouseMotion mouseMotion)
		{
			if (card_being_dragged != null && card_being_dragged.canBeMoved){
				card_being_dragged.Position += mouseMotion.Relative;
			} else 
			if (card_being_hovered != null){
				card_being_hovered.Shadering(GetGlobalMousePosition()-card_being_hovered.GlobalPosition);
			}
		}
	}
	private void HandleSelectionInput(InputEvent @event, Hand hand)
	{
		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
			{
				Card card = RaycastCheckForCard();
				if (card != null) hand.SelectCard(card);
			}
		}
	}
	public override void _Ready() 
	{	cardScene = GD.Load<PackedScene>("res://game/cards/card.tscn");
		audioPlayer = GetNode<AudioStreamPlayer2D>("AudioPlayer");
		hand = GetTree().CurrentScene.GetNodeOrNull<Hand>(GlobalAccessPoint.handPath);
	}
	private void SelectCard(Card card) {
		if (selected_card == card) {
			DeselectCard();
			return;
		};
		DeselectCard();
		selected_card = card;
		EmitSignal(nameof(CardPushup), card, true);	
	}
	private void DeselectCard()
	{
		if (selected_card != null){
			selected_card.ResetShader();
			if (card_being_dragged != selected_card) EmitSignal(nameof(CardPushup), selected_card, false);	
			if (card_being_hovered != selected_card) selected_card.Scale = new Vector2(1,1);
			selected_card = null;			
		}
	}
	public void ConnectCardSignals(Card card)
	{card.CardHovered += _on_card_hovered;   card.CardUnhovered += _on_card_unhovered;}
	public void _on_card_hovered(Card card)
	{	if (isProcessingHover || Locked) return;
		isProcessingHover = true;
		card_being_hovered = card;
		CardHoveredEffect(card);
	}
	public void _on_card_unhovered(Card card)
	{	if (Locked) return;
		isProcessingHover = false;

		Card newCard = RaycastCheckForCard();
		if (newCard!=null) {
			isProcessingHover = true;
			card_being_hovered = newCard;
			CardHoveredEffect(newCard);
		} else card_being_hovered = null;
		CardHoveredEffect(card, false);
		card.ResetShader();
	}
	private void CardHoveredEffect(Card card, bool isHovering = true)
	{
		if (card_being_dragged != null || card==selected_card || hand.isSelecting) return;

		float targetScale = isHovering ? 1.2f : 1.0f;

		if (card.Scale.X != targetScale) 
		{
			card.Scale = new Vector2(targetScale, targetScale);	

			EmitSignal(nameof(CardPushup), card, isHovering);	
			cardSound();			
		}
	}
	private void StartDrag(Card card)
	{
		card_being_dragged = card;
		DeselectCard();
		card.Scale = new Vector2(1.05f, 1.05f);
		card.Rotation = 0;
		card.ZIndex = 11;
		card.ResetShader();
	}
	private void EndDrag()
	{
		if (card_being_dragged == null) return;
		Card tmpCard = card_being_dragged;
		card_being_dragged = null;
		
		cardEffectZone zone = RaycastCheckForZone();
		if (zone != null){
			if (GlobalVariables.spirit >= tmpCard.GetCardData().Cost)
			{
				EmitSignal(nameof(CardUnhand), tmpCard);
				card_being_hovered = null;
				tmpCard.ResetShader();
				zone.activeCard(tmpCard,GetGlobalMousePosition()-zone.GlobalPosition);
				GD.Print("cost: "+tmpCard.GetCardData().Cost);
				GlobalVariables.ChangeSpirit(-tmpCard.GetCardData().Cost);
				GD.Print("spirit: "+GlobalVariables.spirit);
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
	public Card RaycastCheckForCard()
	{
		var result = CardGlobal.RaycastCheckForObjects(this,GetGlobalMousePosition(), CardGlobal.cardCollisionMask);
		if (result.Count > 0) return GetCardWithHighestZIndex(result);

		return null;
	}
	public cardEffectZone RaycastCheckForZone()
	{
		var result = CardGlobal.RaycastCheckForObjects(this,GetGlobalMousePosition(), CardGlobal.cardSlotMask);
		if (result.Count > 0){ // get result[0]
			return (cardEffectZone)result[0]["collider"];			
		}
		return null;
	}
	public static Card GetCardWithHighestZIndex(Godot.Collections.Array<Godot.Collections.Dictionary> cards)
	{
		if (cards.Count == 0) return null; 

		Card highestCard = null;
		int highestZIndex = int.MinValue; 

		foreach (var hit in cards){
			if (hit.ContainsKey("collider")){
				Node collider = (Node)hit["collider"];
				if (collider is Node2D node2D && node2D.GetParent() is Card card){
					if (card.ZIndex > highestZIndex){
						highestZIndex = card.ZIndex;
						highestCard = card;
					}
				}
			}
		}

		return highestCard;
	}
	public void checkChange(Card card) {
		if (card_being_hovered == card) card_being_hovered = null;
		if (card_being_dragged == card) card_being_dragged = null;
		if (selected_card == card) selected_card = null;
		RemoveChild(card);
	}
	public void cardSound() {audioPlayer.Play();}
	public void Lock() {Locked = true;GD.Print("Locked");}
	public void Unlock() {Locked = false;}
}