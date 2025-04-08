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

	public async Task AddCard(Card card)    {
		deck.Add(card.GetCardData());
		CardCount.Text = deck.Count.ToString(); 

		card.TransformCard(getTopCardPosition(),0.0f,0.15f);
		card.canBeHovered = false;
		await card.FlipCard(false);
		card.obliterateCard();

		emitDeckUpdated(deck.Count);
	}

	public async Task AddCards(Godot.Collections.Array<Card> cards)	{ 
		foreach (Card card in cards) { // wait 0.05 seconds for each card to be added
			await ToSignal(GetTree().CreateTimer(0.05f), "timeout");
			AddCard(card);
		}
	}

	public async Task Restock()
	{
		this.deck.Shuffle();
		Deck deck = GetParent().GetNodeOrNull<Deck>(GlobalAccessPoint.deckPath);

		for (int i = 0; i < this.deck.Count; i++)			
			deck.AddCard(this.deck[i]);

		List<Card> cardsToDisplay = new List<Card>();

		for (int i = 0; i < 7; i++) 
		{			
			Card card = cardManager.createCard(this.deck[0]);
			card.ZIndex = 15;
			cardsToDisplay.Add(card);
			card.canBeHovered = false;
			card.GlobalPosition = getTopCardPosition();
			card.FlipCard(false);
			card.TransformCard(deck.GlobalPosition, 0.0f, 0.15f);
			await ToSignal(GetTree().CreateTimer(0.1f), "timeout"); // Delay between cards
		}

		foreach (Card card in cardsToDisplay) 
		{
			card.QueueFree();
		}
		
		this.deck.Clear();
		CardCount.Text = this.deck.Count.ToString(); 
		emitDeckUpdated(this.deck.Count);
	}

}
