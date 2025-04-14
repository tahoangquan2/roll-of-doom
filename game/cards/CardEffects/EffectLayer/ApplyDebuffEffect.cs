using System.Threading.Tasks;
using Godot;
[GlobalClass]
public partial class ApplyDebuffEffect : AttackSingleEffect
{
	[Export] public EnumGlobal.BuffType BuffType;
	public override Task<bool> ApplyEffect(Node2D target)
	{
		if (target is not EnemyChar enemy) return Task.FromResult(false);

		enemy.statInstance.ApplyBuff(BuffType, Amount);
		
		return Task.FromResult(true);
	}
}
