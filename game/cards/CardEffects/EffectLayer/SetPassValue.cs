using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Godot;
[GlobalClass]
public partial class SetPassValue : CardEffect
{
    [Export] public PassedValueType valueType;
    public override Task<bool> ApplyEffect(Node2D target) // implement attack and gain shield
    {      
        int value = 0;
        PlayerStat playerStat = GlobalVariables.playerStat;
        switch (valueType)
        {
            case PassedValueType.selfHealth:
                value = playerStat.currentHealth;
                break;
            case PassedValueType.enemyHealth:
                if (target is not EnemyChar enemy) return Task.FromResult(false);
                value = enemy.statInstance.currentHealth;
                break;
            case PassedValueType.selfDefense:
                value = playerStat.shield + playerStat.guard;
                break;
            case PassedValueType.enemyDefense:
                if (target is not EnemyChar enemy2) return Task.FromResult(false);
                value = enemy2.statInstance.shield + enemy2.statInstance.guard;
                break;
            case PassedValueType.cardInDeck:
                value = GlobalAccessPoint.GetDeck().GetDeckSize();
                break;
            case PassedValueType.cardInHand:
                value = GlobalAccessPoint.GetHand().GetHandSize();
                break;
            case PassedValueType.cardInDiscard:
                value = GlobalAccessPoint.GetDiscardPile().GetDeckSize();
                break;

            case PassedValueType.mana:
                value = playerStat.mana;
                break;
            case PassedValueType.spellMana:
                value = playerStat.spellMana;
                break;
            default:
                GD.PrintErr("SetPassValue: Unknown effect type");
                return Task.FromResult(false);
        }
        GlobalVariables.passedValue = value;
        
        return Task.FromResult(true);
    }

    public enum PassedValueType
    {
        selfHealth,
        enemyHealth,
        selfDefense, // defense = shield + guard
        enemyDefense,

        cardInDeck,
        cardInHand,
        cardInDiscard,
        mana,
        spellMana
    }
}
