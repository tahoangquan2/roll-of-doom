using Godot;

public abstract partial class CardState : Node
{
	public enum State { Idle, Hover, Selected, Dragging, Released }

	protected static CardManager cardManager = null;
	protected Card card{ get; set; } = null;
	[Export] public State cardState = State.Idle;
	public abstract void EnterState(Card card=null);
	public abstract void ExitState(Card card=null);

	public virtual void HandleInput(InputEvent @event)
	{
		GD.Print("CardState input: "+cardState);
	}

	public void changeState(State newState, Card card=null)
	{
		cardManager.StateChangeRequest(cardState, newState, card);
	}

	public override void _Ready()
	{
		cardManager = GetParent<CardManager>();
		GD.Print("CardState: "+cardState+" CardManager: "+cardManager);
		SetProcessInput(false);
		SetProcessUnhandledInput(false);
	}	

	public virtual void setCard(Card card){
		this.card = card;
	}

	public static Card RaycastCheckForCard()
	{
		var result = CardGlobal.RaycastCheckForObjects(cardManager,cardManager.GetGlobalMousePosition(), CardGlobal.CardCollisionMask);
		if (result.Count > 0) return GetCardWithHighestZIndex(result);

		return null;
	}
	public static CardPlayZone RaycastCheckForZone(EnumGlobal.enumCardTargetLayer targetMask = EnumGlobal.enumCardTargetLayer.None)
	{
		var result = CardGlobal.RaycastCheckForObjects(cardManager,cardManager.GetGlobalMousePosition(), (int) targetMask);
		GD.Print("RaycastCheckForZone: "+result);
		if (result.Count > 0){ // get result[0]
			return (CardPlayZone)result[0]["collider"];			
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

	public virtual void _on_card_hovered(Card card){}
	public virtual void _on_card_unhovered(Card card){}
}