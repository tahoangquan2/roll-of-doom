using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Godot;
[GlobalClass]
public partial class AttackSingleEffect : CardEffect
{
    [Export] public int Amount = 1;
    [Export] public bool IsPrecise= false;
    public override Task<bool> ApplyEffect(Node2D target)
    {
        if (target is not EnemyChar enemy) return Task.FromResult(false);
     
        GlobalVariables.playerStat.Attack(enemy.statInstance,Amount,IsPrecise);

        return Task.FromResult(true);
    }
}
