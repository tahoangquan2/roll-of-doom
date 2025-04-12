public static class CardKeywordSystem
{
    public static void OnDraw(Card card, Hand hand)
    {
        if (card.cardData.Keywords.Contains(EnumGlobal.CardKeywords.Bundled))
            hand.DrawFromDeckSimple(1);

        if (card.cardData.Keywords.Contains(EnumGlobal.CardKeywords.Auto)){
			hand.RemoveCard(card); // remove the card from hand
            card.ActivateEffects(GlobalAccessPoint.GetCardActiveZone()); // or with a dummy zone
		}
    }

    public static void OnCycle(Card card, Hand hand)
    {
        if (card.cardData.Keywords.Contains(EnumGlobal.CardKeywords.Keep))
            return; // do nothing

        if (card.cardData.Keywords.Contains(EnumGlobal.CardKeywords.Needy))
            card.KillCard(); // or move to Forgotten pile

        else card.putToDiscardPile();
    }

    public static void OnPlay(Card card) // after effect is done
    {
        if (card.cardData.Keywords.Contains(EnumGlobal.CardKeywords.Ephemeral))
            card.KillCard(); // forget the card
		else card.putToDiscardPile(); // normal discard
    }
}
