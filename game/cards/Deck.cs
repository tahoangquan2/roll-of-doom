using Godot;
using System;

public partial class Deck : Node2D
{
    public Godot.Collections.Array<CardData> deck;
    private int deckSize = 10;    
    private int drawSize = 1;


    public override void _Ready()
    {

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
            Card newCard = new Card();
            newCard.cardData = card;
            drawnCards.Add(newCard);
        }
        return drawnCards;
    }


    public void ShuffleDeck()    {
        deck.Shuffle();
    }

    public void AddCard(CardData card)    {
        deck.Add(card);
    }
}
