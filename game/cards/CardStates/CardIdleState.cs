using Godot;
public partial class CardIdleState : CardState
{
	public override void EnterState(Card card)
	{
		cardManager.card_being_processed=null;		
		base.card = null;
	}

	public override void ExitState(Card card)
	{
	}
	public override void HandleInput(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouseMotion)
		{
			
		}
	}
}
