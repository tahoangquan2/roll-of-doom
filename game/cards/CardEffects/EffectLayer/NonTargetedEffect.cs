using System.Threading.Tasks;
using Godot;
[GlobalClass]
public partial class NonTargetedEffect : CardEffect
{
    [Export] public EnumGlobal.enumNonTargetedEffect nonTargetedEffectType;
    [Export] public int Amount = 1;

    public override Task<bool> ApplyEffect(Node2D target)
    {
        Hand hand = target.GetTree().CurrentScene.GetNode<Hand>(GlobalAccessPoint.handPath);
        Deck deck = target.GetTree().CurrentScene.GetNode<Deck>(GlobalAccessPoint.deckPath);
        PlayerStat playerStat= GlobalVariables.playerStat;
        GD.Print(hand);
        GD.Print(deck);

        switch (nonTargetedEffectType)
        {
            case EnumGlobal.enumNonTargetedEffect.DealAoE:
                playerStat.AttackAll(Amount);
                break;

            case EnumGlobal.enumNonTargetedEffect.DealRandom:
                playerStat.AttackRandom(Amount);
                break;

            case EnumGlobal.enumNonTargetedEffect.CheckSelfStat:
                GD.Print("playerStat HP:", playerStat.currentHealth, " Shield:", playerStat.shield);
                break;

            case EnumGlobal.enumNonTargetedEffect.GainShield:
                playerStat.Add_shield(Amount);
                break;

            case EnumGlobal.enumNonTargetedEffect.GainGuard:
                playerStat.Add_guard(Amount);
                break;

            // case EnumGlobal.enumNonTargetedEffect.GainMana:
            //     playerStat.AddMana(Amount);
            //     break;

            // case EnumGlobal.enumNonTargetedEffect.GainSpellMana:
            //     playerStat.AddSpellMana(Amount);
            //     break;

            // case EnumGlobal.enumNonTargetedEffect.ApplyBuff:
            //     playerStat.ApplyBuff(Buff, Amount, EnumGlobal.BuffDuration.Diminishing); // Optional: make duration configurable
            //     break;

            // case EnumGlobal.enumNonTargetedEffect.ApplyDebuff:
            //     foreach (var enemy in GlobalVariables.allStats)
            //         if (enemy != playerStat)
            //             enemy.ApplyDebuff(Buff, Amount, EnumGlobal.BuffDuration.Diminishing);
            //     break;

            // case EnumGlobal.enumNonTargetedEffect.Heal:
            //     playerStat.Heal(Amount);
            //     break;

            // case EnumGlobal.enumNonTargetedEffect.Draw:
            //     await hand.drawFromDeck(Amount);
            //     break;

            // case EnumGlobal.enumNonTargetedEffect.Discard:
            //     if (!hand.startDiscard(Amount))
            //         return false;
            //     break;

            // case EnumGlobal.enumNonTargetedEffect.Forget:
            //     await hand.PutHandIntoDiscardPile(); // or your forget mechanic
            //     break;

            // case EnumGlobal.enumNonTargetedEffect.ShuffleDeck:
            //     deck.ShuffleDeck();
            //     break;

            // case EnumGlobal.enumNonTargetedEffect.DiscardHand:
            //     hand.DiscardHand();
            //     break;

            // case EnumGlobal.enumNonTargetedEffect.Restock:
            //     await discard.Restock();
            //     break;

            // case EnumGlobal.enumNonTargetedEffect.ShuffleHandtoDeck:
            //     hand.ShuffleHandtoDeck();
            //     break;

            // case EnumGlobal.enumNonTargetedEffect.EndTurn:
            //     GlobalVariables.TurnManager?.EndTurn(); // defensive
            //     break;
        }
        

        return Task.FromResult(true);
    }

    //     public enum enumNonTargetedEffect
    // {
    //     DealAoE = enumCardEffect.DealAoE,
    //     DealRandom = enumCardEffect.DealRandom,
    //     CheckSelfStat = enumCardEffect.CheckSelfStat,
    //     GainShield = enumCardEffect.GainShield,
    //     GainGuard = enumCardEffect.GainGuard,
    //     GainMana = enumCardEffect.GainMana,
    //     GainSpellMana = enumCardEffect.GainSpellMana,
    //     ApplyBuff = enumCardEffect.ApplyBuff,     // apply buff to self
    //     ApplyDebuff = enumCardEffect.ApplyDebuff, // apply debuff to all enemies
    //     Heal = enumCardEffect.Heal,


    //     EndTurn = enumCardEffect.EndTurn,
    //     Draw=enumCardEffect.Draw,
    //     Discard=enumCardEffect.Discard,
    //     Forget=enumCardEffect.Forget,
    //     ShuffleDeck=enumCardEffect.ShuffleDeck,
    //     DiscardHand=enumCardEffect.DiscardHand,
    //     Restock=enumCardEffect.Restock,
    //     ShuffleHandtoDeck=enumCardEffect.ShuffleHandtoDeck, 
    // }

}
