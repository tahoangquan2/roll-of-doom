using System.Collections;
using System.Collections.Generic;
using Godot;

public partial class CardManager : Node2D
{
	private PackedScene cardScene;
	public Card card_being_dragged = null;
	public Card card_being_hovered = null;
	private Tween tween = null;
	public bool isProcessingHover = false;
	private AudioStreamPlayer2D audioPlayer;
	
	[Signal] public delegate void CardPushupEventHandler(Card card,bool isHovered);
	[Signal] public delegate void CardHasSlotEventHandler(Card card);

	public override void _Process(double delta)
	{
	}


	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButton && mouseButton.ButtonIndex == MouseButton.Left)        {
			if (mouseButton.Pressed)
			{
				Card card = RaycastCheckForCard();
				if (card != null){
					StartDrag(card);
				}
			}else{
				EndDrag();
			}
		}

		if (@event is InputEventMouseMotion mouseMotion)
		{
			if (card_being_dragged != null){
				card_being_dragged.Position += mouseMotion.Relative;
			} else 
			if (card_being_hovered != null){
				card_being_hovered.Shadering(GetGlobalMousePosition()-card_being_hovered.GlobalPosition);
			}
		}

		if (@event is InputEventAction action && action.Action == "Action" && action.Pressed)
		{
			GD.Print("Action input pressed");
		}
	}

	public override void _Ready()
	{
		cardScene = GD.Load<PackedScene>("res://game/cards/card.tscn");
		audioPlayer = GetNode<AudioStreamPlayer2D>("AudioPlayer");
	}

	// connect card signals
	public void ConnectCardSignals(Card card)
	{
		card.CardHovered += _on_card_hovered;   
		card.CardUnhovered += _on_card_unhovered;
	}

	// Signal handler for when the mouse hovers over a card
	public void _on_card_hovered(Card card)
	{
		if (isProcessingHover) return;

		isProcessingHover = true;
		card_being_hovered = card;
		CardHoveredEffect(card);
	}

	public void _on_card_unhovered(Card card)
	{
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

	// Hovered card effect
	private void CardHoveredEffect(Card card, bool isHovering = true)
	{
		if (card_being_dragged != null) return;

		float targetScale = isHovering ? 1.05f : 1.0f;
		//int targetZIndex = isHovering ? 2 : 1;	

		if (card.Scale.X != targetScale) 
		{
			card.Scale = new Vector2(targetScale, targetScale);	

			EmitSignal(nameof(CardPushup), card, isHovering);	
			audioPlayer.Play();					
		}
		
		//card.ZIndex = targetZIndex;
	}

	private void StartDrag(Card card)
	{
		card_being_dragged = card;
		card.Scale = new Vector2(1.05f, 1.05f);
		card.Rotation = 0;
		card.ZIndex = 11;
		card.ResetShader();

		if (tween != null && tween.IsRunning()) tween.Kill();
	}

	private void EndDrag()
	{
		if (card_being_dragged == null) return;
		// Return to the last known slot
		if (card_being_dragged.CurrentSlot != null)
		{
			EmitSignal(nameof(CardHasSlot), card_being_dragged);
			AnimateCardToPosition(card_being_dragged, card_being_dragged.CurrentSlot.Position);
		}
		card_being_dragged = null;
	}

	private void AnimateCardToPosition(Card card, Vector2 targetPosition)
	{
		if (tween != null && tween.IsRunning()) tween.Kill();
		
		tween = GetTree().CreateTween();
		tween.TweenProperty(card, "position", targetPosition, 0.2f).SetEase(Tween.EaseType.Out);
	}

	// Raycast to check for card
	public Card RaycastCheckForCard()
	{
		var result = CardGlobal.RaycastCheckForObjects(this,GetGlobalMousePosition(), CardGlobal.cardCollisionMask);
		if (result.Count > 0) return GetCardWithHighestZIndex(result);

		return null;
	}

	// Get the card with the highest ZIndex from a list of collision results
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
}