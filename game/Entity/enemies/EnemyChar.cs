using Godot;
using System;
using System.Threading.Tasks;

public partial class EnemyChar : Character
{
	// Called when the node enters the scene tree for the first time.
	EnemyStat enemyStat => statInstance as EnemyStat;
	public override void _Ready()
	{
		base._Ready();

		// enemyStat.currentHealth = 25;
		// enemyStat.guard = 50;
		// enemyStat.shield = 5;

		// // add a buff to the player stat
		// AddBuff(EnumGlobal.BuffType.Dodge, 5);

		UpdateStatsDisplay();
	}

	public override void Cycle() {}

	public async Task EnemyTurn()
	{
		await ToSignal(GetTree().CreateTimer(1), "timeout");
		await enemyStat.EnemyTurn();
		//1 second delay		
	}
}
