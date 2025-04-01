using Godot;
public partial class CardIdleState : CardState
{
	public override void EnterState(Card card)
	{
		CardReset(card);
		cardManager.DeselectCard();
		cardManager.EmitSignal(nameof(cardManager.CardPushup), CardState.card, false);		
		CardState.card = null;
	}

	public override void ExitState(Card card){}
	public override void HandleInput(InputEvent @event){}

	public override void _on_card_hovered(Card card)
	{
		changeState(State.Hover, card);
	}

	private void CardReset(Card card)	{
		if (card == null) return;
		card.Scale = new Vector2(1, 1);
		card.ResetShader();
		cardManager.DeselectCard();
	}

}
