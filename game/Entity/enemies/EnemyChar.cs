using Godot;
using System;
using System.Threading.Tasks;

public partial class EnemyChar : Character
{
	// Called when the node enters the scene tree for the first time.
	EnemyStat enemyStat => statInstance as EnemyStat;
	HBoxContainer _actionGrid => GetNode<HBoxContainer>("IntendContainer");
	private PackedScene intentUiScene = GD.Load<PackedScene>("res://game/Entity/IntentUi.tscn");	

	public override void _Ready()
	{
		base._Ready();

		enemyStat.SetupActionsForType(enemyStat.enemyType);
		enemyStat.ActionPicked += UpdateIntent;
		enemyStat.PickAction(GlobalVariables.playerStat);	

		playZoneType = EnumGlobal.enumCardTargetLayer.Enemy; 

		UpdateStatsDisplay();
	}
	
    public void UpdateIntent(){		 
		foreach (Node child in _actionGrid.GetChildren()) child.QueueFree();

		foreach (var action in enemyStat.intentedAction){
			EnemyActionBase enemyAction = action;
			var intent = intentUiScene.Instantiate<IntentUi>();		
			_actionGrid.AddChild(intent);
			intent.SetIntent(enemyAction);
			intent.Pressed+= () => EmitSignal(nameof(IntentClicked), intent);
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
