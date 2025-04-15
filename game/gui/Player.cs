using System;
using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class Player : CanvasLayer
{
	// Called when the node enters the scene tree for the first time.
	private Label manaLabel;
	private Label goldLabel;
	private Control pauseMenu;

	public override void _Ready()
	{
		manaLabel = GetNode<Label>("TopGui/Container/BaseMana/TxtBox/PointLabel");
		goldLabel = GetNode<Label>("TopGui/Container/Gold/TxtBox/GoldLabel");

		pauseMenu = GetNode<Control>("Pause");

		update_values();
	}
	public void update_values() // access though unique name
	{
		manaLabel.Text = $"{GlobalVariables.playerStat.baseMana}";
		goldLabel.Text = $"{GlobalVariables.playerStat.gold}";
	}


	private TaskCompletionSource<Godot.Collections.Array<CardData>> selectionCompletionSource;

	// public void StartSelectionMode(Godot.Collections.Array<CardData> cardPile,EnumGlobal.PileSelectionPurpose purpose, int minSelection, int maxSelection,Callable onConfirm)
	// {
	// 	GlobalAccessPoint.GetCardPileView().SetSelectableCardPile(
	// 		cardPile,			
	// 		minSelection,
	// 		maxSelection, 
	// 		onConfirm
	// 		);
	// }

	public Task<Godot.Collections.Array<CardData>> StartSelectionMode(
		Godot.Collections.Array<CardData> cardPile,
		EnumGlobal.PileSelectionPurpose purpose,
		int minSelection,
		int maxSelection)
	{
		selectionCompletionSource = new TaskCompletionSource<Godot.Collections.Array<CardData>>();

		CardPileView cardPileView = GlobalAccessPoint.GetCardPileView();

		//base on purpose cardpileview set title
		switch (purpose)
		{
			case EnumGlobal.PileSelectionPurpose.Shuffle:
				cardPileView.Title = "";
				break;
			case EnumGlobal.PileSelectionPurpose.Discard:
				cardPileView.Title = "Choose card(s) to discard";
				break;
			case EnumGlobal.PileSelectionPurpose.Forget:
				cardPileView.Title = "Choose card(s) to forget";
				break;
			case EnumGlobal.PileSelectionPurpose.Duplicate:
				cardPileView.Title = "Choose card(s) to duplicate";
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(purpose), purpose, null);
		}

		cardPileView.SetSelectableCardPile(
			cardPile,
			minSelection,
			maxSelection,
			Callable.From((Godot.Collections.Array<CardData> selected) =>
			{
				selectionCompletionSource.TrySetResult(selected);
			})
		);

		

		return selectionCompletionSource.Task;
	}



}
