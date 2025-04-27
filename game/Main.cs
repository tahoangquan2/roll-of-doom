using Godot;
using System;
using System.Collections.Generic;

public partial class Main : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GlobalAccessPoint.Instance.UpdateReferences();
		GC.Collect();

		StartGame();
	}

	private Node2D entityLayer=> GetNodeOrNull<Node2D>("EntityLayer");

	float levelScaling = 1.0f;
	
	public void StartGame()
	{
		GlobalVariables.allStats.Clear();
		GlobalVariables.allCharacters.Clear();
		GlobalVariables.allCharacterStats.Clear();

		if (GlobalVariables.playerStat == null)	{
			
			var playerStat = ResourceLoader.Load<PlayerStat>("res://game/Entity/player/Warrior.tres");
			GlobalVariables.playerStat = playerStat;
		} 
		

		var palyer = entityLayer.GetChild(0) as PlayerChar;
		palyer.CharacterSetUp(GlobalVariables.playerStat);	

		//randomize enemy count from 1 to 4
		int enemyCount = GlobalVariables.GetRandomNumber(1, 4);

		for (int i=1;i<5;i++){
			var enemy = entityLayer.GetChild(i) as EnemyChar;
			if (i>enemyCount){
				enemy.QueueFree();			
			}
			else{
				int enemyIndex = GlobalVariables.GetRandomNumber(0, GlobalVariables.enemyStatsBase.Count-1);
				enemy.CharacterSetUp(GlobalVariables.enemyStatsBase[enemyIndex]);
			}
		}

		// get every node in GameStart group
		foreach (Node node in GetTree().GetNodesInGroup("GameStart"))
		{
			// call GameStart method
			var method = node.GetType().GetMethod("GameStart");
			method.Invoke(node, null);
		}	
	}


	//input
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_filedialog_refresh"))
		{
			GetTree().ReloadCurrentScene();			
		}

		//GD.Print(GetViewport().GuiGetFocusOwner());
	}
}
