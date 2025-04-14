using System.Threading.Tasks;
using Godot;
[GlobalClass]
public partial class DamageEffect : CardEffect
{
    [Export] public int damage = 10;
    public override Task<bool> ApplyEffect(Node2D target)
    {
        if (target is EnemyChar enemyChar)
        {
            GlobalVariables.playerStat.Attack(enemyChar.GetStat(), 10);    

            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
}
