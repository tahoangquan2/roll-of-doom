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
	private Button confirmButton => GetNode<Button>("SelectionFilter/Button");


	private List<CardMenuUi> selectedCards = new();
	public int maxSelection = 0;
	public int minSelection = 0; // if min is not met confirm button can not be pressed
	private Callable onSelectionConfirmed;
	private bool isSelectable = false;




	[Export] public string Title {get => title.Text;set => title.Text = value;}

	enum SortType	{Alphabetical,Cost,Type}

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

	public void SetSelectableCardPile(Godot.Collections.Array<CardData> cardPile, int minSelect, int maxSelect, Callable onConfirm)
	{	
		Visible = true;
		isSelectable = true;
		
		onSelectionConfirmed = onConfirm;
		SetCardPile(cardPile);
		minSelection = minSelect;
		maxSelection = Math.Min(maxSelect, cardPile.Count);
		confirmButton.Disabled = true;
		confirmButton.Visible = true;
	}

	private void HandleCardToggle(CardMenuUi cardUI)
	{
		if (selectedCards.Contains(cardUI))
		{
			cardUI.ToggleSelection();
			selectedCards.Remove(cardUI);
		}
		else
		{
			if (selectedCards.Count >= maxSelection)
			{
				var oldest = selectedCards[0];
				oldest.ToggleSelection();
				selectedCards.RemoveAt(0);
			}

			cardUI.ToggleSelection();
			selectedCards.Add(cardUI);
		}

		confirmButton.Disabled = selectedCards.Count < minSelection;
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

			if (isSelectable) cardInstance.button.Pressed += () => HandleCardToggle(cardInstance);
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
		GD.Print("CardPileView: confirm button pressed");
		var parentControl = GetParent();
		
		if (parentControl.HasMethod("_on_button_toggled"))
		{
			parentControl.Call("_on_button_toggled",false);
		} else{
			Visible = false;        
		}

		isSelectable = false;
		var selected = new Godot.Collections.Array<CardData>();
		foreach (var cardUI in selectedCards)
			selected.Add(cardUI.cardData);

		onSelectionConfirmed.Call(selected);
		
	}

	public void _on_sort_type_pressed()
	{
		switchSortType();
	}
}