using System;
using Godot;
using System.Threading.Tasks;
using System.Runtime.Versioning;

public partial class Player : CanvasLayer
{
	// Called when the node enters the scene tree for the first time.
	private Label manaLabel,goldLabel,gameOverLabel;
	private Control pauseMenu;

	private TextureButton pauseMenuButton;

	private Node RootNode => GetTree().Root;
	bool gamewon = false;

	public void GameStart()
	{	
		manaLabel = GetNode<Label>("TopGui/Container/BaseMana/TxtBox/PointLabel");
		goldLabel = GetNode<Label>("TopGui/Container/Gold/TxtBox/GoldLabel");

		pauseMenu = GetNode<Control>("Pause");		
		pauseMenu.Visible = false;

		gameOverLabel = pauseMenu.GetNode<Label>("GameOverLabel");
		pauseMenuButton = pauseMenu.GetNode<TextureButton>("ContinueButton");
		// gameOverLabel.Visible = false;
		// pauseMenuButton.Visible = false;

		update_values();
	}
	public void update_values() // access though unique name
	{
		manaLabel.Text = $"{GlobalVariables.playerStat.baseMana}";
		goldLabel.Text = $"{GlobalVariables.playerStat.gold}";
	}


	private TaskCompletionSource<Godot.Collections.Array<CardData>> selectionCompletionSource;

	public Task<Godot.Collections.Array<CardData>> StartSelectionMode(
		Godot.Collections.Array<CardData> cardPile,
		EnumGlobal.PileSelectionPurpose purpose,
		int minSelection,
		int maxSelection)
	{
		selectionCompletionSource = new TaskCompletionSource<Godot.Collections.Array<CardData>>();

		CardPileView cardPileView = GlobalAccessPoint.GetCardPileView();

		string prefix;
		if (minSelection == maxSelection){prefix = "Choose " + minSelection + " card(s)";} else {
			prefix = "Choose " + minSelection + " to " + maxSelection + " card(s)";}

		//base on purpose cardpileview set title
		switch (purpose)
		{
			case EnumGlobal.PileSelectionPurpose.Shuffle:
				cardPileView.Title = prefix+" to shuffle into draw pile";
				break;
			case EnumGlobal.PileSelectionPurpose.Discard:
				cardPileView.Title = prefix+" to discard";
				break;
			case EnumGlobal.PileSelectionPurpose.Forget:
				cardPileView.Title = prefix+" to forget";
				break;
			case EnumGlobal.PileSelectionPurpose.Duplicate:
				cardPileView.Title = prefix+" to duplicate";
				break;
			case EnumGlobal.PileSelectionPurpose.Scry:
				cardPileView.Title = "Scry up to " + maxSelection + " card(s)";
				break;
			case EnumGlobal.PileSelectionPurpose.AddtoDeck:
				cardPileView.Title = prefix+" to add to deck";
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

	public void WinGame()
	{
		gameOverLabel.Text = "You Win";		
		gamewon = true;
		endTheGame();		
	}
	public void LoseGame()
	{	
		gameOverLabel.Text = "Game Over";
		SaveManager.DeleteSaveGame();
		gamewon = false;
		endTheGame();
	}

	public async void _on_continue_button_pressed()
	{
		if (gamewon)
		{
			// creat 3 random cards from card pool
			Godot.Collections.Array<CardData> randomCards = new Godot.Collections.Array<CardData>();
			for (int i = 0; i < 3; i++)
			{
				int randomIndex = GlobalVariables.GetRandomNumber(0, GlobalVariables.cardPool.Count - 1);
				// check if card already in randomCards
				while (randomCards.Contains(GlobalVariables.cardPool[randomIndex]))
				{
					randomIndex = GlobalVariables.GetRandomNumber(0, GlobalVariables.cardPool.Count - 1);
				}
				randomCards.Add(GlobalVariables.cardPool[randomIndex]);
			}
		
			var selected = await StartSelectionMode(
				randomCards,
				EnumGlobal.PileSelectionPurpose.AddtoDeck,
				1, 1
			);

			foreach (var card in selected)
				GlobalVariables.playerStat.startingDeck.Add(card);

			SaveManager.newLevel();

			GetTree().ChangeSceneToPacked(GlobalVariables.battleScene);
			}
		else
		{
			GetTree().ChangeSceneToPacked(GlobalVariables.mainMenuScene);
		}
		
	}

	private void endTheGame()
	{
		pauseMenu.Visible = true;
	}
}
