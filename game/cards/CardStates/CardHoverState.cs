using Godot;
public partial class CardHoverState : CardState
{
	public override void EnterState(Card card)
	{
		setCard(card);	
	}

	public override void ExitState(Card card)
	{
	}

	public override void HandleInput(InputEvent @event)
	{
		if (@event is InputEventMouseMotion)
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

	public override void setCard(Card card){
		if (card==this.card) return;
		if (this.card != null) {
			CardHoveredEffect(this.card, false);
			this.card.ResetShader();
		}
		this.card = card;
		if (this.card != null) {
			CardHoveredEffect(this.card, true);
			this.card.Shadering(cardManager.GetGlobalMousePosition()-this.card.GlobalPosition);
		}
		cardManager.card_being_processed = card;
	}

	private void CardHoveredEffect(Card card, bool isHovering = true)
	{
		//if (card_being_dragged != null || card == selected_card || hand.isSelecting) return;

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
