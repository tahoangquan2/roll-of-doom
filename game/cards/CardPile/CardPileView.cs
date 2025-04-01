using Godot;
using System;
using System.Collections.Generic;

public partial class CardPileView : Control
{
	private PackedScene cardUI = GD.Load<PackedScene>("res://game/cards/CardPile/cardMenuUI.tscn");

	private GridContainer gridContainer => GetNode<GridContainer>("NinePatchRect/ScrollContainer/GridContainer");

	private Label title => GetNode<Label>("NinePatchRect/Title");

	private Button SortTypeLabel => GetNode<Button>("NinePatchRect/SortType");

	private List<CardData> cardPile;

	[Export] public string Title
	{
		get => title.Text;
		set => title.Text = value;
	}

	enum SortType	{
		Alphabetical,Cost,Type
	}

	private SortType sortType = SortType.Alphabetical;

	private Callable onItemChosenGD;

	public void SetCardPile(Godot.Collections.Array<CardData> cardPile)
	{
		var tmpCardPile = sortPile(sortType,new List<CardData>(cardPile));

		if (tmpCardPile==this.cardPile)
			return;

		this.cardPile = tmpCardPile;

		resetView();
	}

	private void resetView()
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

	public void switchSortType(){
		sortType = (SortType)(((int)sortType + 1) % Enum.GetValues(typeof(SortType)).Length);
		cardPile = sortPile(sortType,cardPile);
		SortTypeLabel.Text = "Sort Type: "+sortType.ToString();
		resetView();
	}

	private List<CardData> sortPile(SortType sortType,List<CardData> cardPile)
	{
		var sortedCardPile = new List<CardData>(cardPile);
		switch (sortType)
		{
			case SortType.Alphabetical:				
				sortedCardPile.Sort((a, b) => a.CardName.CompareTo(b.CardName));				
				break;
			case SortType.Cost:
				sortedCardPile.Sort((a, b) => a.Cost.CompareTo(b.Cost));
				break;
			case SortType.Type:
				sortedCardPile.Sort((a, b) => a.CardType.CompareTo(b.CardType));
				break;
		}
		return sortedCardPile;
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

	public void _on_sort_type_pressed()
	{
		switchSortType();
	}
}