using System.Threading.Tasks;
using Godot;
[GlobalClass]
public partial class ApplyBuffEffect : CardEffect
{
    [Export] public EnumGlobal.BuffType buffType;
    [Export] public int Amount=1;

    public override async Task<bool> ApplyEffect(Node2D target)
    {
        PlayerStat playerStat= GlobalVariables.playerStat;

        playerStat.ApplyBuff(buffType, Amount);    

        return true;
    }
}
