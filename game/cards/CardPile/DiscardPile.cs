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

	public async void AddCard(Card card)    {
		deck.Add(card.GetCardData());
		CardCount.Text = deck.Count.ToString(); 

		card.TransformCard(getTopCardPosition(),0.0f,0.15f);
		card.canBeHovered = false;
		await card.FlipCard(false);
		card.obliterateCard();

		emitDeckUpdated(deck.Count);
	}

	public async Task Restock(){ // flood the deck with cards from the discard pile
		this.deck.Shuffle();
		Deck deck = GetParent().GetNodeOrNull<Deck>(GlobalAccessPoint.deckPath);
		for (int i = 0; i < this.deck.Count; i++)			
			deck.AddCard(this.deck[i]);

		for (int i=0;i<3;i++) 
		{
			Card card = cardManager.createCard(this.deck[0]);
			card.canBeHovered = false;
			card.GlobalPosition = getTopCardPosition();
			card.TransformCard(deck.GlobalPosition,0.0f,0.15f);
			await card.FlipCard(true);
			card.QueueFree();
		}
		
		this.deck.Clear();
		CardCount.Text = this.deck.Count.ToString(); 
		emitDeckUpdated(this.deck.Count);
	}
}
