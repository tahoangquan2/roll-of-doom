using System.Threading.Tasks;
using Godot;
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

    public static bool OnCycle(Card card, Hand hand) // true mean do nothing false mean discard
    {
        var data = card.cardData;
        if (data.Keywords.Contains(EnumGlobal.CardKeywords.Keep)
        || data.Keywords.Contains(EnumGlobal.CardKeywords.Eternal)) {
            hand.AddCard(card); // add to hand
            return true; // do nothing
        }


        if (data.Keywords.Contains(EnumGlobal.CardKeywords.Needy)){
            card.KillCard(); // forget the card
            return true;
        }

        return false; // discard the card
    }

    public static void OnPlay(Card card) // after effect is done
    {
        var data = card.cardData;

        // Eternal 
        if (data.Keywords.Contains(EnumGlobal.CardKeywords.Eternal))
        {
            GD.Print($"[Eternal] {data.CardName} has been marked as played this turn.");
        }

        //  Ephemeral → Forget
        if (data.Keywords.Contains(EnumGlobal.CardKeywords.Ephemeral))
        {
            GD.Print($"[Ephemeral] {data.CardName} is forgotten after use.");
            card.KillCard(); 
            return;
        }

        // Default behavior: discard
        GD.Print($"[Default] {data.CardName} is discarded after use.");
        card.putToDiscardPile();
    }

    public static async Task OnDiscardOrForget(Card card) 
    {
        CardData cardData = card.cardData;
        if (cardData.Keywords.Contains(EnumGlobal.CardKeywords.Overwork))
		{
            card.continueAfterEffect = true; // Disable killing the card
            await card.ActivateEffects(GlobalAccessPoint.GetCardActiveZone()); 
        }
    }
}
