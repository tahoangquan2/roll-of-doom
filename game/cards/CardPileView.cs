using Godot;

public partial class CardPileView : Control
{
	private PackedScene cardUI = GD.Load<PackedScene>("res://game/cards/cardMenuUI.tscn");

	private GridContainer gridContainer => GetNode<GridContainer>("NinePatchRect/ScrollContainer/GridContainer");

	public void SetCardPile(Godot.Collections.Array<CardData> cardPile)
	{
		foreach (Node child in gridContainer.GetChildren())
		{
			child.QueueFree();
		}
		
		foreach (CardData card in cardPile)
		{
			CardMenuUi cardInstance = (CardMenuUi)cardUI.Instantiate();
			cardInstance.SetCardData(card);
			gridContainer.AddChild(cardInstance);
		}
	}
	
	public void _on_button_pressed()
	{
		Visible = !Visible;
	}
}