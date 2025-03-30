using Godot;
public partial class CardDragState : CardState
{
	public override void EnterState(Card card)
	{		
		CardState.card = card;
		CardState.card.ResetShader();
		cardManager.StartDrag(card);
	}

	public override void ExitState(Card card)
	{
	}
	
	public override void HandleInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == MouseButton.Left && !mouseButton.Pressed){
				changeState(State.Released, card);
				return;
			}

			if (mouseButton.ButtonIndex == MouseButton.Right && mouseButton.Pressed){
				changeState(State.Idle, card);
				return;
			}
		}

		if (@event is InputEventMouseMotion mouseMotion)
		{
			if (card.canBeMoved)
				card.Position += mouseMotion.Relative;
			
			return;
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
}
