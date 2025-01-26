using System.Collections;
using System.Collections.Generic;
using Godot;

public partial class CardManager : Node2D
{
	public Card card_being_dragged = null;
	public Card card_being_hovered = null;
	public Card selected_card = null;
	private Tween tween = null;
	public bool isProcessingHover = false;
	private AudioStreamPlayer2D audioPlayer;
	
	[Signal] public delegate void CardPushupEventHandler(Card card,bool isHovered);
	[Signal] public delegate void CardUnhandEventHandler(Card card);

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButton)        {
			if (mouseButton.ButtonIndex == MouseButton.Left)
			{
				if (mouseButton.Pressed)
				{
					if (selected_card != null) 
					{ 
						card_being_dragged = selected_card;
						DeselectCard();
						EndDrag();
						return;
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
			if (card_being_dragged != null){
				card_being_dragged.Position += mouseMotion.Relative;
			} else 
			if (card_being_hovered != null){
				card_being_hovered.Shadering(GetGlobalMousePosition()-card_being_hovered.GlobalPosition);
			}
		}
	}
	public override void _Ready()
	{
		audioPlayer = GetNode<AudioStreamPlayer2D>("AudioPlayer");
	}

	private void SelectCard(Card card)
	{
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
			selected_card = null;
		}

	}
	public void ConnectCardSignals(Card card)
	{
		card.CardHovered += _on_card_hovered;   
		card.CardUnhovered += _on_card_unhovered;
	}

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
	private void CardHoveredEffect(Card card, bool isHovering = true)
	{
		if (card_being_dragged != null || card==selected_card) return;

		float targetScale = isHovering ? 1.05f : 1.0f;

		if (card.Scale.X != targetScale) 
		{
			card.Scale = new Vector2(targetScale, targetScale);	

			EmitSignal(nameof(CardPushup), card, isHovering);	
			audioPlayer.Play();					
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

		if (tween != null && tween.IsRunning()) tween.Kill();
	}
	private void EndDrag()
	{
		if (card_being_dragged == null) return;
		Card tmpCard = card_being_dragged;
		card_being_dragged = null;
		
		cardEffectZone zone = RaycastCheckForZone();
		if (zone != null){
			EmitSignal(nameof(CardUnhand), tmpCard);
			card_being_hovered = null;
			tmpCard.ResetShader();
			zone.activeCard(tmpCard);
		}		
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