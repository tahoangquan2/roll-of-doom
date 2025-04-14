using System;
using Godot;
using System.Threading.Tasks;

public partial class CardPile : Node2D
{
    public Godot.Collections.Array<CardData> deck=new Godot.Collections.Array<CardData>();
       
    protected CardManager cardManager;
    protected CardPileView cardPileView = null;

    private Button button => GetNode<Button>("Button");
    protected Label CardCount => GetNode<Label>("Circle-64/CardCount");

    private DeckVisual deckVisual=null;

    [Export] public bool hasCardStack = true;
    [Signal] public delegate void DeckUpdatedEventHandler(int cardsRemaining);
    [Signal] public delegate void DeckEmptyEventHandler();
    [Export] public Vector2 cardCountOffset = new Vector2(0, 0);

    public override void _Ready() {         
        deck = new Godot.Collections.Array<CardData>();
        cardPileView = GetNode<CardPileView>("CardPileView");
        cardPileView.Visible = false;
        cardPileView.SetFunctionForButtonPressed(new Callable(this, nameof(ShuffleDeck)));
        
        cardPileView.SetGlobalPosition(new Vector2(0, 0));        
        CardCount.Text = deck.Count.ToString();
        GetNode<Control>("Circle-64").Position = cardCountOffset;

        if (hasCardStack) deckVisual = GetNode<DeckVisual>("DeckVisual");
    }
    public void SetupDeck( Godot.Collections.Array<CardData> cards)
    {
        deck = cards;
        ShuffleDeck();
    }

    public void ShuffleDeck()    {
        deck.Shuffle();
    }
    public int GetDeckSize()    {
        return deck.Count;
    }
    virtual public void AddCard(CardData card)    {
        deck.Add(card);
        CardCount.Text = deck.Count.ToString(); 
        emitDeckUpdated(deck.Count);
    }
    virtual public void RemoveCard(int index)    {
        if (index < 0 || index >= deck.Count) return;
        if (deck.Count == 0) return;
        deck.RemoveAt(index);
        CardCount.Text = deck.Count.ToString(); 
        emitDeckUpdated(deck.Count);
    }
    virtual public Godot.Collections.Array<CardData> GetRandomCard(int amount)    {
        Godot.Collections.Array<CardData> cardGot = new Godot.Collections.Array<CardData>();
        for (int i = 0; i < amount; i++)
        {
            if (deck.Count == 0) continue;

            int index = GD.RandRange(0, deck.Count-1);
            CardData card = deck[index];
            cardGot.Add(card);
            
            deck.RemoveAt(index);
        }
        CardCount.Text = deck.Count.ToString(); 
        emitDeckUpdated(deck.Count);

        return cardGot;        
    }
    public int GetCardIndexFromType(EnumGlobal.enumCardType type)    {
        for (int i = 0; i < deck.Count; i++)
        {
            if (deck[i].CardType == type) return i;
        }
        return -1;
    }   
    virtual public void ShuffleIntoDeck(CardData card)    {
        deck.Insert(GD.RandRange(0, deck.Count), card);
        CardCount.Text = deck.Count.ToString(); 
        emitDeckUpdated(deck.Count);
    }
    public async Task AddCards(Godot.Collections.Array<Card> cards)	{ 
		foreach (Card card in cards) { // wait 0.05 seconds for each card to be added
			await ToSignal(GetTree().CreateTimer(0.05f), "timeout");
			AddCard(card);
		}
	}

    public async Task AddCard(Card card)    {
        ShuffleIntoDeck(card.GetCardData());

		card.TransformCard(getTopCardPosition(),0.0f,0.15f);
		card.canBeHovered = false;
		await card.FlipCard(false);
		card.obliterateCard();
	}

    protected Vector2 getTopCardPosition()
    {
        if (deckVisual == null) return new Vector2(0, 0);
        return deckVisual.getTopCardPosition();
    }

    public void _on_button_toggled(bool buttonPressed)
    {
        if (buttonPressed)
        {         
            cardManager.Lock();
            cardPileView.SetCardPile(deck);
            cardPileView.Visible = true;
        }
        else
        {
            cardPileView.Visible = false;            
            cardManager.Unlock();
            button.ButtonPressed = false;
        }
    }
    protected void emitDeckUpdated(int newSize)
    {
        EmitSignal(nameof(DeckUpdated), newSize);
        CardCount.Text = deck.Count.ToString();  
    }
}