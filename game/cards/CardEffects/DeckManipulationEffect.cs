using Godot;
[GlobalClass]
public partial class DeckManipulationEffect : CardEffect
{
    [Export] public EnumGlobal.enumDeckEffect DeckEffectType ;
    // Draw, Discard, Duplicate, ResetDeck are the options
    [Export] public int Amount = 1;

    public override bool ApplyEffect(Node2D target)
    {
        Hand hand = GlobalAccessPoint.GetHand();
        Deck deck = GlobalAccessPoint.GetDeck();
        GD.Print(hand);
        GD.Print(this,$" Applying deck manipulation effect: {DeckEffectType} to {target.Name}.");
        
        switch (DeckEffectType)
        {
            case EnumGlobal.enumDeckEffect.Draw:
                hand.drawFromDeck(Amount);
                GD.Print($"Drew {Amount} cards.");
                break;

            case EnumGlobal.enumDeckEffect.Discard:
                //cardManager.DiscardCards(Amount);
                hand.startDiscard(Amount);
                GD.Print($"Discarded {Amount} cards.");
                break;

            case EnumGlobal.enumDeckEffect.Duplicate:
                //cardManager.DuplicateCard(Amount);
                GD.Print($"Duplicated {Amount} cards.");
                break;

            case EnumGlobal.enumDeckEffect.ShuffleDeck:
                deck.ShuffleDeck();
                GD.Print("Shuffled deck.");
                break;

            case EnumGlobal.enumDeckEffect.ShuffleCard:
                //cardManager.ShuffleCard();
                GD.Print("Shuffled a card back into deck.");
                break;

            case EnumGlobal.enumDeckEffect.ShuffleHand:
                //hand.ShuffleHand();
                GD.Print("Shuffled hand back into the deck.");
                break;

            case EnumGlobal.enumDeckEffect.DiscardHand:
                hand.DiscardHand();
                GD.Print("Discarded hand.");
                break;

        }

        return true;
    }
}
