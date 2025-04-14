using System.Threading.Tasks;
using Godot;
[GlobalClass]
public partial class AttackSingleEffect : CardEffect
{
    [Export] public int Amount = 1;
    public override Task<bool> ApplyEffect(Node2D target)
    {
        if (target is not EnemyChar enemy) return Task.FromResult(false);
     
        GlobalVariables.playerStat.Attack(enemy.statInstance,Amount);

        return Task.FromResult(true);
    }
}
