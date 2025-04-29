using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Godot;
[GlobalClass]
public partial class EffectFromPassed : CardEffect
{
    [Export] public PassedEffectType effectType;
    public override Task<bool> ApplyEffect(Node2D target) // implement attack and gain shield
    {        
        int value = GlobalVariables.getPassedValue();
        PlayerStat playerStat = GlobalVariables.playerStat;
        GD.Print("EffectFromPassed: ", value);
        switch (effectType)
        {
            case PassedEffectType.Attack:
                if (target is not EnemyChar enemy) return Task.FromResult(false);
                playerStat.Attack(enemy.statInstance,value);
                break;
            case PassedEffectType.AttackAll:
                playerStat.AttackAll(value);
                break;
            case PassedEffectType.GainMana:
                playerStat.GainMana(value);
                break;
            case PassedEffectType.GainShield:
                playerStat.Add_shield(value);
                break;
            case PassedEffectType.GainGuard:
                playerStat.Add_guard(value);
                break;
            case PassedEffectType.Draw:
               
                break;
            case PassedEffectType.AddStatusEffect:
                //playerStat.AddStatusEffect(status, value);
                break;
            default:
                GD.PrintErr("EffectFromPassed: Unknown effect type");
                return Task.FromResult(false);
        }
        

        return Task.FromResult(true);
    }

    public enum PassedEffectType
    {
        Attack, AttackAll,
        GainMana,
        GainShield,
        GainGuard,
        Draw,
        AddStatusEffect,
    }
}
