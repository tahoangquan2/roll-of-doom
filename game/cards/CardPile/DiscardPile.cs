using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public partial class DiscardPile : CardPile
{    
    // when the decksize is updated cardcount is updated
    public override void _Ready() { 
        cardCountOffset = new Vector2(20, -50);
        base._Ready();
        cardManager = GetTree().CurrentScene.GetNodeOrNull<CardManager>(GlobalAccessPoint.cardManagerPath);        
    }
	public async Task Restock()
	{
		deck.Shuffle();
		Deck deckNode = GetParent().GetNodeOrNull<Deck>(GlobalAccessPoint.deckPath);
		Hand hand = GetParent().GetNodeOrNull<Hand>(GlobalAccessPoint.handPath);
		cardManager = GetTree().CurrentScene.GetNodeOrNull<CardManager>(GlobalAccessPoint.cardManagerPath);

		PileKeywordSystem.OnRestock(deck, deckNode, hand,cardManager);

		for (int i = 0; i < deck.Count; i++)			
			deckNode.AddCard(deck[i]);

		List<Card> cardsToDisplay = new List<Card>();
		if (deck.Count!= 0) 
		for (int i = 0; i < 5; i++) 
		{			
			Card card = cardManager.createCard(new CardData());
			card.ZIndex = 15;
			cardsToDisplay.Add(card);
			card.canBeHovered = false;
			card.GlobalPosition = getTopCardPosition();
			card.FlipCard(false);
			card.TransformCard(deckNode.GlobalPosition, 0.0f, 0.25f);
			await ToSignal(GetTree().CreateTimer(0.1f), "timeout"); // Delay between cards
		}

		foreach (Card card in cardsToDisplay) card.QueueFree();
		
		deck.Clear();
		CardCount.Text = deck.Count.ToString(); 
		emitDeckUpdated(0);
	}

}
