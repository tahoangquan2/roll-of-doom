using Godot;
public partial class CardReleaseState : CardState
{
	public override void EnterState(Card card)
	{
		cardManager.EndDrag();
		changeState(State.Idle, card);
	}

	public override void ExitState(Card card){}
}
