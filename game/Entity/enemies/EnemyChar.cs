using Godot;
using System;
using System.Threading.Tasks;

public partial class EnemyChar : Character
{
	// Called when the node enters the scene tree for the first time.
	EnemyStat enemyStat => statInstance as EnemyStat;
	HBoxContainer _actionGrid => GetNode<HBoxContainer>("IntendContainer");
	public override void _Ready()
	{
		base._Ready();

		enemyStat.SetupActionsForType(enemyStat.enemyType);
		enemyStat.ActionPicked += UpdateIntent;
		enemyStat.PickAction(GlobalVariables.playerStat);		

		UpdateStatsDisplay();
	}

	
    public void UpdateIntent(){		    
		GD.Print("Intent Updated");
		// get rid of all children of the grid
		foreach (Node child in _actionGrid.GetChildren()) child.QueueFree();

		foreach (var action in enemyStat.intentedAction){
			EnemyActionBase enemyAction = action;
			var intent = new IntentUi();			
			_actionGrid.AddChild(intent);
			intent.SetIntent(enemyAction);
		}
    }

	public override void Cycle() {}

	public async Task EnemyTurn()
	{
		await ToSignal(GetTree().CreateTimer(1), "timeout");
		await enemyStat.EnemyTurn();
		//1 second delay		
	}
}
