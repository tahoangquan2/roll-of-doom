using Godot;
public partial class CardDragState : CardState
{
	public override void EnterState(Card card)
	{		
		CardState.card = card;
		cardManager.StartDrag(card);
		cardManager.EmitSignal(nameof(cardManager.CardFocus), card.GetCardData(),true);
		movingCard = true;
	}

	public override void ExitState(Card card)
	{
		cardManager.EmitSignal(nameof(cardManager.CardFocus), card.GetCardData(),false);
		movingCard = true;
		cardManager.displayArc(false);
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
			if (card.canBeMoved && movingCard)
				card.Position += mouseMotion.Relative;
			
			return;
		} 		
	}

	public override void _on_zone_update(bool isEntered, CardPlayZone zone)
	{		
		if (card == null) return;

		if (!card.IsLayerNone()) 
		{
			//GD.Print("CardDragState _on_zone_update: "+isEntered+" "+zone);
			if (playZones.Count != 0) {
				movingCard=false;
				cardManager.SetCardMiddle(card);
				cardManager.displayArc(true);				
			} else{
				movingCard=true;
				card.TransformCard(cardManager.GetGlobalMousePosition(),0);
				cardManager.displayArc(false);
			}

			if (card.GetCardData().TargetMask==zone.GetPlayZoneType()) {
				if (isEntered) cardManager.SetCardArcZone(zone);	
				else cardManager.SetCardArcZone(null);
			}
			//GD.Print("CardDragState _on_zone_update: "+card.GetCardData().TargetMask+" "+zone.GetPlayZoneType());
		}		
	}

	private bool movingCard = true;
}
