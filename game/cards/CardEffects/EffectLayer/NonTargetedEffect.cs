using System.Threading.Tasks;
using Godot;
[GlobalClass]
public partial class NonTargetedEffect : CardEffect
{
    [Export] public EnumGlobal.enumNonTargetedEffect nonTargetedEffectType;
    [Export] public int Amount = 1;

    public override async Task<bool> ApplyEffect(Node2D target)
    {
        Hand hand = target.GetTree().CurrentScene.GetNode<Hand>(GlobalAccessPoint.handPath);
        Deck deck = target.GetTree().CurrentScene.GetNode<Deck>(GlobalAccessPoint.deckPath);
        PlayerStat playerStat= GlobalVariables.playerStat;

        bool result = true;

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

            case EnumGlobal.enumNonTargetedEffect.GainMana:
                playerStat.GainMana(Amount);
                break;

            case EnumGlobal.enumNonTargetedEffect.GainSpellMana:
                playerStat.GainSpellMana(Amount);
                break;

            case EnumGlobal.enumNonTargetedEffect.Heal:
                playerStat.heal(Amount);
                break;

            case EnumGlobal.enumNonTargetedEffect.Draw:
                await hand.drawFromDeck(Amount);
                break;

            case EnumGlobal.enumNonTargetedEffect.Discard:
                result = await hand.StartDiscard(Amount,Amount);
                break;

            case EnumGlobal.enumNonTargetedEffect.DiscardUpTo:
                result = await hand.StartDiscard(0,Amount);
                break;

            case EnumGlobal.enumNonTargetedEffect.Forget:
                result = await hand.StartForget(Amount);
                break;

            case EnumGlobal.enumNonTargetedEffect.ShuffleDeck:
                deck.ShuffleDeck();
                break;

            case EnumGlobal.enumNonTargetedEffect.DiscardHand:
                hand.DiscardHand();
                break;
            case EnumGlobal.enumNonTargetedEffect.Scry:
                await deck.Scry(Amount);
                break;

            case EnumGlobal.enumNonTargetedEffect.Restock:
                //await discard.Restock();
                break;

            // case EnumGlobal.enumNonTargetedEffect.ShuffleHandtoDeck:
            //     hand.ShuffleHandtoDeck();
            //     break;

            // case EnumGlobal.enumNonTargetedEffect.EndTurn:
            //     GlobalVariables.TurnManager?.EndTurn(); // defensive
            //     break;
            default:
                GD.PrintErr("NonTargetedEffect: Unknown effect type.");
                return false;
        }        

        return result;
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
