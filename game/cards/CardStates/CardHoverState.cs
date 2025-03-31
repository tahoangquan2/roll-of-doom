using Godot;
public partial class CardHoverState : CardState
{
	public override void EnterState(Card card)
	{
		setCard(card);	
	}

	public override void ExitState(Card card)	{}

	public override void HandleInput(InputEvent @event)
	{
		if (@event is InputEventMouseMotion && card!=null)
		{
			card.Shadering(cardManager.GetGlobalMousePosition()-card.GlobalPosition);
			return;
		} 

		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed){
				changeState(State.Dragging, card);
				return;
			}

			if (mouseButton.ButtonIndex == MouseButton.Right && mouseButton.Pressed){
				changeState(State.Selected, card);
				return;
			}
		}
	}

	public override void _on_card_unhovered(Card card)
	{
		Card NewCard = RaycastCheckForCard();
		if (NewCard != null){
			setCard(NewCard);
		} else {
			setCard(null);
			changeState(State.Idle, card);
		}
	}

	public override void setCard(Card card){
		if (card==CardState.card) return;
		if (CardState.card != null) {
			CardHoveredEffect(CardState.card, false);
			CardState.card.ResetShader();
		}
		CardState.card = card;
		if (CardState.card != null) {
			CardHoveredEffect(CardState.card, true);
			CardState.card.Shadering(cardManager.GetGlobalMousePosition()-CardState.card.GlobalPosition);
		}
	}

	private void CardHoveredEffect(Card card, bool isHovering = true)
	{
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

	public override void _on_zone_update(bool isEntered, CardPlayZone zone)
	{
		if (card == null) return;
		if (card.GetCardData().TargetMask==zone.GetPlayZoneType()) {
			
		}
	}

}
