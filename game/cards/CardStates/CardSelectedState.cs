using Godot;
public partial class CardSelectedState : CardState
{
	private Card tmpCard = null;
	public override void EnterState(Card card)
	{
		cardManager.SelectCard(card);
		tmpCard = card;
	}

	public override void ExitState(Card card)
	{
		cardManager.DeselectCard();
		tmpCard = null;
	}

	public override void HandleInput(InputEvent @event)
	{
		if (@event is InputEventMouseMotion && tmpCard != null)
		{
			tmpCard.Shadering(cardManager.GetGlobalMousePosition()-tmpCard.GlobalPosition);
			return;
		} 

		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed){
				Card newCard=RaycastCheckForCard();
				if (newCard == null ) {
					changeState(State.Released, card);
					return;
				}
				changeState(State.Hover, newCard);
				return;
			}

			if (mouseButton.ButtonIndex == MouseButton.Right && mouseButton.Pressed){
				Card newCard=RaycastCheckForCard();

				if (newCard == null ) {
					changeState(State.Idle, card);
					return;
				}
				if (newCard != card) {	
					CardHoveredEffect(newCard, true);
					tmpCard = card;		
					card = newCard;											
					CardHoveredEffect(tmpCard, false);
					tmpCard = card;
					cardManager.SelectCard(card);
				}
				else changeState(State.Hover, card);
				
				return;
			}
		}
	}

	public override void _on_card_unhovered(Card card)
	{
		Card NewCard = RaycastCheckForCard();
		setCard(NewCard);
	}

	public override void _on_card_hovered(Card card)
	{
		
	}

	public override void setCard(Card card){
		if (card==tmpCard) return;
		if (tmpCard != null) {
			CardHoveredEffect(tmpCard, false);
			tmpCard.ResetShader();
		}
		tmpCard = card;
		if (tmpCard != null) {
			CardHoveredEffect(tmpCard, true);
			tmpCard.Shadering(cardManager.GetGlobalMousePosition()-tmpCard.GlobalPosition);
		}
	}

	private void CardHoveredEffect(Card card, bool isHovering = true)
	{		
		if (card==CardState.card) return;
		float targetScale = isHovering ? 1.2f : 1.0f;

		if (card.Scale.X != targetScale)
		{
			Tween tween = GetTree().CreateTween(); 
			tween.TweenProperty(card, "scale", new Vector2(targetScale, targetScale), 0.35f)
				.SetTrans(Tween.TransitionType.Elastic)
				.SetEase(Tween.EaseType.Out);

			cardManager.EmitSignal(nameof(cardManager.CardPushup), card, isHovering);	
			cardManager.cardSound();
		}
	}

}
