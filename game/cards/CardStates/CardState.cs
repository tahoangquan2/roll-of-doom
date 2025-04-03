using System.Collections.Generic;
using Godot;

public abstract partial class CardState : Node
{
	public enum State { Idle, Hover, Selected, Dragging, Released }

	protected static CardManager cardManager = null;
	public static Card card{ get; set; } = null;
	public static List<CardPlayZone> playZones = new List<CardPlayZone>();
	[Export] public State cardState = State.Idle;
	public abstract void EnterState(Card card=null);
	public abstract void ExitState(Card card=null);

	public virtual void HandleInput(InputEvent @event)
	{
		//GD.Print("CardState input: "+cardState);
	}

	public void changeState(State newState, Card card=null)
	{
		//GD.Print("CardState changeState: "+cardState+" -> "+newState);
		cardManager.StateChangeRequest(cardState, newState, card);
	}

	public override void _Ready()
	{
		card=null;
		cardManager = GetParent<CardManager>();
		SetProcessInput(false);
		SetProcessUnhandledInput(false);
	}	

	public virtual void setCard(Card card){
		CardState.card = card;
	}

	public static Card RaycastCheckForCard()
	{
		var result = CardGlobal.RaycastCheckForObjects(cardManager,cardManager.GetGlobalMousePosition(), CardGlobal.CardCollisionMask);
		if (result.Count > 0) return GetCardWithHighestZIndex(result);

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
	public virtual void _on_zone_update(bool isEntered, CardPlayZone zone){}
}