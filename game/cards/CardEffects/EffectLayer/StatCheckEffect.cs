using System.Threading.Tasks;
using Godot;
[GlobalClass]
public partial class StatCheckEffect : AttackSingleEffect
{
	public enum StatCheckType
	{
		Buff,	Guard,
		Shield, Health,
	}

	[Export] public StatCheckType statCheckType = StatCheckType.Buff;
	[Export] public EnumGlobal.BuffType Buff;
	[Export] public bool isLowerThan = false; // 
    public override Task<bool> ApplyEffect(Node2D target)
    {
        if (target is not EnemyChar enemy) return Task.FromResult(false);

        var stat = enemy.statInstance;
		int value = 0 ;

		switch (statCheckType)
		{
			case StatCheckType.Buff:
				value=stat.GetBuffValue(Buff);
				break;

			case StatCheckType.Guard:
				value=stat.guard;
				break;

			case StatCheckType.Shield:
				value=stat.shield;
				break;

			case StatCheckType.Health:
				value=stat.currentHealth;				
				break;
			default:
				value=10;
				break;
		}

		//compare to ammount
		if (isLowerThan)	{
			if (value >= Amount) return Task.FromResult(false);
		}		else		{
			if (value < Amount) return Task.FromResult(false);
		}
            

        return Task.FromResult(true);
    }
}
