using Godot;
using System;
using System.Threading.Tasks;

public partial class EnemyChar : Character
{
	// Called when the node enters the scene tree for the first time.
	EnemyStat enemyStat;
	private HBoxContainer actionGrid;
	private PackedScene intentUiScene = GD.Load<PackedScene>("res://game/Entity/intentUI.tscn");

	public override void CharacterSetUp(Stats stat)
	{		
		actionGrid = GetNode<HBoxContainer>("IntendContainer");
		base.CharacterSetUp(stat);
		enemyStat = (EnemyStat)statInstance;
		setScale(enemyStat.scaleFactor);
		
		enemyStat.SetupActionsForType(enemyStat.enemyType,enemyStat.scaleFactor);

		enemyStat.ActionPicked += UpdateIntent;
		enemyStat.PickAction(GlobalVariables.playerStat);	

		playZoneType = EnumGlobal.enumCardTargetLayer.Enemy; 
	}

	private void setScale(float scaleFactor){
		statInstance.maxHealth = (int)(statInstance.maxHealth * scaleFactor);
		statInstance.currentHealth = (int)(statInstance.currentHealth * scaleFactor);
		UpdateStatsDisplay();
	}
	
    public void UpdateIntent(){		 
		foreach (Node child in actionGrid.GetChildren()) child.QueueFree();

		foreach (var action in enemyStat.intentedAction){
			EnemyActionBase enemyAction = action;
			var intent = intentUiScene.Instantiate<IntentUi>();		
			actionGrid.AddChild(intent);
			intent.SetIntent(enemyAction);
			// set press event and print action name
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

	public override void _ExitTree()
	{
		base._ExitTree();

		if (enemyStat != null)
			enemyStat.ActionPicked -= UpdateIntent;
	}

}
