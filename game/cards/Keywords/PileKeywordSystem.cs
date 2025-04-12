using System.Collections.Generic;
using Godot;

public static class PileKeywordSystem
{
    public static void OnRestock(Godot.Collections.Array<CardData> discardPile, Deck deck, Hand hand)
    {
        List<CardData> moved = new List<CardData>();

        foreach (var cardData in discardPile)
        {
            if (cardData.Keywords.Contains(EnumGlobal.CardKeywords.Magnetic))
            {
                //Card card = deck.cardManager.createCard(cardData);
                //hand.AddCard(card);
                moved.Add(cardData);
            }
        }

        // Remove moved cards from discard pile
        foreach (var cd in moved) discardPile.Remove(cd);
    }

    public static void OnDiscardOrForget(CardData cardData)
    {
        if (cardData.Keywords.Contains(EnumGlobal.CardKeywords.Overwork))
		{
			// Handle Overwork logic here
            GD.Print($"{cardData.CardName} Overworked!");
        }
    }

	public static void OnGameStart(Deck deck, Hand hand)
	{
		foreach (var cardData in deck.deck)
		{
			if (cardData.Keywords.Contains(EnumGlobal.CardKeywords.Fated))
			{
				// Handle Magnetic logic here
				GD.Print($"{cardData.CardName} is Fated!");
			}
		}
	}
}
