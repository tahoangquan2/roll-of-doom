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

		enemyStat.SetupActions();

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
