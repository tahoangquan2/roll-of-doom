using System;
using System.Threading.Tasks;
using Godot;

public partial class Deck : CardPile
{ 
    private DeckVisual deckVisual => GetNode<DeckVisual>("DeckVisual");    
    
    // when the decksize is updated cardcount is updated

    public void GameStart() { 
        cardCountOffset = new Vector2(20, -50);
        cardManager = GetTree().CurrentScene.GetNodeOrNull<CardManager>(GlobalAccessPoint.cardManagerPath);
        Hand hand = GetParent().GetNodeOrNull<Hand>(GlobalAccessPoint.handPath);

        foreach (CardData card in GlobalVariables.playerStat.startingDeck)        {
            if (card == null) continue;
            if (card.Keywords.Contains(EnumGlobal.CardKeywords.Fated))            {
                Card cardNode=cardManager.createCard(card);   
                cardNode.Position = deckVisual.getTopCardPosition();    
                hand.AddCard(cardNode);         
                GD.Print($"{card.CardName} is Fated!");
                continue;
            }
            deck.Add(card);
        }

        deck.Shuffle();
        
        emitDeckUpdated(deck.Count);   
    }

    public Card DrawCard(int index){
        if (index < 0 || index >= deck.Count) return null;
        CardData card = deck[index];
        deck.RemoveAt(index);
        Card newCard=cardManager.createCard(card);
        newCard.Position = GlobalPosition;
        emitDeckUpdated(deck.Count); 
        return newCard;
    }
    public async Task<Godot.Collections.Array<Card>> DrawCards(int amount){
        Godot.Collections.Array<Card> drawnCards = new Godot.Collections.Array<Card>();
        for (int i = 0; i < amount; i++)
        {   
            if (deck.Count == 0)
            {
                DiscardPile discardPile = GetParent().GetNodeOrNull<DiscardPile>(GlobalAccessPoint.discardPilePath);

                await discardPile.Restock();
                GD.Print("Deck is empty, restocking from discard pile");
                if (deck.Count == 0) break;
            }
            CardData card = deck[0];            
            deck.RemoveAt(0);
            Card newCard = cardManager.createCard(card);
            drawnCards.Add(newCard);
            newCard.Position = deckVisual.getTopCardPosition();            
        }
        emitDeckUpdated(deck.Count);
        
        return drawnCards;
    }


    public async Task<bool> Scry(int amount)
    {
        if (deck.Count == 0) return false;
        // pass top amount card in deck to selection mode
        if (amount > deck.Count) amount = deck.Count;
        Godot.Collections.Array<CardData> passed = new Godot.Collections.Array<CardData>();
        
        for (int i = 0; i < amount; i++)      
            passed.Add(deck[i]);

        var selected = await GlobalAccessPoint.GetPlayer().StartSelectionMode(
            passed, EnumGlobal.PileSelectionPurpose.Scry, 0, amount
        );
        
        Godot.Collections.Array<Card> selectedCards = new Godot.Collections.Array<Card>();
        foreach (var cardData in selected) {
            deck.Remove(cardData);      
            Card card = cardManager.createCard(cardData);
            card.Position = deckVisual.getTopCardPosition();
            selectedCards.Add(card);
        }
        await GlobalAccessPoint.GetDiscardPile().AddCards(selectedCards);
        
        emitDeckUpdated(deck.Count);
        
        return true;
        
    }
}