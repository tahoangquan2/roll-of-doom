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
	
	public void StartGame()
	{
		GlobalVariables.allStats.Clear();
		GlobalVariables.allCharacters.Clear();
		GlobalVariables.allCharacterStats.Clear();
		int enemyCount = 1;

		List<EnemyStat> enemyStat = new List<EnemyStat>();
		var playerStat = ResourceLoader.Load<PlayerStat>("res://game/Entity/player/Warrior.tres");

		GameSaveData save = SaveManager.LoadGame();
		if (save != null)	{	
			GlobalVariables.globalScale = save.globalScale;

			GlobalVariables.playerStat = playerStat;
			GlobalVariables.playerStat.maxHealth = save.maxHealth;
			GlobalVariables.playerStat.currentHealth = save.currentHealth;
			GlobalVariables.playerStat.capSpellMana = save.capSpellMana;
			GlobalVariables.playerStat.baseMana = save.baseMana+(int) save.globalScale-1;
			GlobalVariables.playerStat.cardDrawPerTurn = save.cardDrawPerTurn;
			GlobalVariables.playerStat.gold = save.gold;
			GlobalVariables.playerStat.startingDeck.Clear();
			foreach (int cardID in save.startingDeckID) {
				CardData card = GlobalVariables.cardPoolDict[cardID];
				if (card != null) {
					GlobalVariables.playerStat.startingDeck.Add(card);
				}
			}

			// load enemy stats
			enemyCount = save.enemiesType.Count;
			for (int i=0;i<enemyCount;i++){
				enemyStat.Add(GlobalVariables.enemyStatsBase[save.enemiesType[i]]);
				enemyStat[i].scaleFactor = save.enemiesScale[i];
			}

		}	else	{			
			GlobalVariables.playerStat = playerStat;	
			enemyCount = GlobalVariables.GetRandomNumber(1,2);	
			GlobalVariables.globalScale = 1.0f;			
		}		

		var palyer = entityLayer.GetChild(0) as PlayerChar;
		palyer.CharacterSetUp(GlobalVariables.playerStat);	

		for (int i=1;i<=4;i++){
			var enemy = entityLayer.GetChild(i) as EnemyChar;
			if (i>enemyCount){
				enemy.QueueFree();			
			} else {
				if (enemyStat.Count>0){ // if saved enemy use saved enemy
					enemy.CharacterSetUp(enemyStat[i-1]);
				} else { // if no saved enemy generate random enemy
					EnumGlobal.EnemyType enemyIndex = (EnumGlobal.EnemyType)GlobalVariables.GetRandomNumber(0, GlobalVariables.enemyStatsBase.Count - 1);
					enemy.CharacterSetUp(GlobalVariables.enemyStatsBase[enemyIndex]);							
				}
			}
		}

		// save data to file
		SaveManager.SaveGame();

		// get every node in GameStart group
		foreach (Node node in GetTree().GetNodesInGroup("GameStart"))
		{
			// call GameStart method
			var method = node.GetType().GetMethod("GameStart");
			method.Invoke(node, null);
		}	
	}


	// //input
	// public override void _Input(InputEvent @event)
	// {
	// 	if (@event.IsActionPressed("ui_filedialog_refresh"))
	// 	{
	// 		GetTree().ReloadCurrentScene();			
	// 	}

	// 	//GD.Print(GetViewport().GuiGetFocusOwner());
	// }
}
