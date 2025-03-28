using Godot;
using System;

public partial class CardPileView : Control
{
	private PackedScene cardUI = GD.Load<PackedScene>("res://game/cards/cardMenuUI.tscn");

	private GridContainer gridContainer => GetNode<GridContainer>("NinePatchRect/ScrollContainer/GridContainer");

	private Label title => GetNode<Label>("NinePatchRect/Title");

	[Export] public string Title
	{
		get => title.Text;
		set => title.Text = value;
	}

	private Callable onItemChosenGD;

	public void SetCardPile(Godot.Collections.Array<CardData> cardPile)
	{
		foreach (Node child in gridContainer.GetChildren())
		{
			child.QueueFree();
		}
		
		foreach (CardData card in cardPile)
		{
			CardMenuUi cardInstance = (CardMenuUi)cardUI.Instantiate();
			gridContainer.AddChild(cardInstance);
			cardInstance.SetCardData(card);	
			cardInstance.SetFunctionForButtonPressed(onItemChosenGD);		
		}
	}

	public void _ready()
	{
		setTitle(Title);
	}

	public void SetFunctionForButtonPressed(Callable onSelection)
	{
		onItemChosenGD = onSelection;
	}

	public void setTitle(string title)
	{
		Title = title;
	}
	
	public void _on_button_pressed()
	{
		var parentControl = GetParent();
		if (parentControl.HasMethod("_on_button_toggled"))
		{
			parentControl.Call("_on_button_toggled",false);
		}
	}
}