using Godot;

public partial class Deck : Node2D
{
    public Godot.Collections.Array<CardData> deck=new Godot.Collections.Array<CardData>();
    private int deckSize = 40;    
    private int drawSize = 1;
    private PackedScene cardScene=null;

    private CardManager cardManager;

    [Signal] public delegate void DeckUpdatedEventHandler(int cardsRemaining);
    [Signal] public delegate void DeckEmptyEventHandler();

    public override void _Ready()
    { // demo
        deck = new Godot.Collections.Array<CardData>();
        cardScene = GD.Load<PackedScene>("res://game/cards/card.tscn");
        cardManager = GetParent().GetNode<CardManager>("CardManager");
        
        for (int i = 0; i < deckSize; i++)
        {
            CardData card = new CardData();
            card.CardName = $"Card {i}";
            card.Description = $"Description {i}";
            card.CardType = (EnumGlobal.enumCardType)GD.RandRange(0, 3);
            card.CardArt = GD.Load<CompressedTexture2D>($"res://assets/cards/Details/Backgrounds/Others/BackgroundCard{GD.RandRange(81, 93)}.png");
            card.Cost = GD.RandRange(1, 10);
            deck.Add(card);
        }
        EmitSignal(nameof(DeckUpdated), deckSize);
    }

    public void SetupDeck( Godot.Collections.Array<CardData> cards)
    {
        deck = cards;
        ShuffleDeck();
    }

    public  Godot.Collections.Array<Card> DrawCards(int amount){
        Godot.Collections.Array<Card> drawnCards = new Godot.Collections.Array<Card>();
        amount = Mathf.Clamp(amount, 0, deck.Count);
        for (int i = 0; i < amount; i++)
        {   
            if (deck.Count == 0)
            {
                GD.Print("Deck is empty.");
                break;
            }
            CardData card = deck[0];            
            deck.RemoveAt(0);
            Card newCard = (Card)cardScene.Instantiate();
            cardManager.AddChild(newCard);
            newCard.SetupCard(card);
            drawnCards.Add(newCard);
        }
        EmitSignal(nameof(DeckUpdated), deck.Count);
        return drawnCards;
    }


    public void ShuffleDeck()    {
        deck.Shuffle();
    }

    public int GetDeckSize()    {
        return deck.Count;
    }

    public void AddCard(CardData card)    {
        deck.Add(card);
        EmitSignal(nameof(DeckUpdated), deck.Count);
    }

    public void RemoveCard()    {
        if (deck.Count == 0) return;
        deck.RemoveAt(deck.Count - 1);
        EmitSignal(nameof(DeckUpdated), deck.Count);
    }
    // on button pressed
    
    public void _on_button_pressed()
    {
        Godot.Collections.Array<Card> drawnCards = DrawCards(drawSize);
        foreach (Card card in drawnCards)
        {
            
        }
        EmitSignal(nameof(DeckUpdated), deck.Count);
    }
}
