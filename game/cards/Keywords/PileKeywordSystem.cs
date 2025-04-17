using System.Collections.Generic;
using Godot;

public static class PileKeywordSystem
{
    public static void OnRestock(Godot.Collections.Array<CardData> discardPile, Deck deck, Hand hand,CardManager cardManager)
    {
        List<CardData> moved = new List<CardData>();

        foreach (var cardData in discardPile)
        {
            if (cardData.Keywords.Contains(EnumGlobal.CardKeywords.Magnetic))
            {
                Card card = cardManager.createCard(cardData);
                card.GlobalPosition = deck.GlobalPosition;
                hand.AddCard(card);
                moved.Add(cardData);
            }
        }

        // Remove moved cards from discard pile
        foreach (var cd in moved) discardPile.Remove(cd);
    }

	public static void OnGameStart(Deck deck, Hand hand,CardManager cardManager)
	{
		foreach (var cardData in deck.deck)
		{
			if (cardData.Keywords.Contains(EnumGlobal.CardKeywords.Fated))
			{				
				GD.Print($"{cardData.CardName} is Fated!");
			}
		}
	}
}
