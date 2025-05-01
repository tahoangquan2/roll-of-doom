using Godot;

public partial class SaveManager : Node
{
	public static void SaveGame()
	{
		// get card id from player stat deck
		var cardIDs = new Godot.Collections.Array<int>();
		foreach (var card in GlobalVariables.playerStat.startingDeck)
			cardIDs.Add(card.CardID);

		// get enemy stats from all stats
		var enemyTypes = new Godot.Collections.Array<EnumGlobal.EnemyType>();
		var enemyScales = new Godot.Collections.Array<float>();
		foreach (var stat in GlobalVariables.allStats)
			if (stat is EnemyStat enemyStat)
			{
				enemyTypes.Add(enemyStat.enemyType);
				enemyScales.Add(enemyStat.scaleFactor);
			}		

		var saveDict = new Godot.Collections.Dictionary
		{
			{ "maxHealth", GlobalVariables.playerStat.maxHealth },
			{ "currentHealth", GlobalVariables.playerStat.currentHealth },
			{ "gold", GlobalVariables.playerStat.gold },
			{ "cardDrawPerTurn", GlobalVariables.playerStat.cardDrawPerTurn },
			{ "capSpellMana", GlobalVariables.playerStat.capSpellMana },
			{ "baseMana", GlobalVariables.playerStat.baseMana },
			{ "startingDeckID", cardIDs },
			// starting deck id
			{ "enemiesType", enemyTypes },
			{ "enemiesScale", enemyScales },
			{ "globalScale", GlobalVariables.globalScale }
			// Add more if needed, like deck, buffs, etc.
		};

		var path = OS.GetUserDataDir() + "/savegame.json";
		var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
		file.StoreString(Json.Stringify(saveDict));
		file.Close();
	}
	public static GameSaveData LoadGame()
	{
		var path = OS.GetUserDataDir() + "/savegame.json";
		if (!System.IO.File.Exists(path))
		{
			GD.Print("Save file not found.");
			return null;
		}

		var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
		var jsonString = file.GetAsText();
		file.Close();
		var json = new Json();
		Error err = json.Parse(jsonString);
		if (err != Error.Ok)
		{
			GD.Print("Failed to parse save file: ", err);
			return null;
		}
		var saveDict = json.Data.As<Godot.Collections.Dictionary>();
		var save = new GameSaveData
		{
			currentHealth = (int)saveDict["currentHealth"],
			maxHealth = (int)saveDict["maxHealth"],
			capSpellMana = (int)saveDict["capSpellMana"],
			baseMana = (int)saveDict["baseMana"],
				cardDrawPerTurn = (int)saveDict["cardDrawPerTurn"],
				gold = (int)saveDict["gold"],
				startingDeckID = saveDict["startingDeckID"].As<Godot.Collections.Array<int>>(),
				enemiesType = saveDict["enemiesType"].As<Godot.Collections.Array<EnumGlobal.EnemyType>>(),
				enemiesScale = saveDict["enemiesScale"].As<Godot.Collections.Array<float>>(),
				globalScale = (float)saveDict["globalScale"]
			};
			GD.Print("Save file loaded.");

			return save;			
		}

	public static void DeleteSaveGame()
	{
		var path = OS.GetUserDataDir() + "/savegame.json";
		if (System.IO.File.Exists(path))
		{
			System.IO.File.Delete(path);
			GD.Print("Save file deleted.");
		}
		else
		{
			GD.Print("No save file to delete.");
		}		
	}

	public static void newLevel()
	{
		GD.Print("New Level");
		PlayerStat playerStat = GlobalVariables.playerStat.Duplicate() as PlayerStat;
		float newScale = GlobalVariables.globalScale+0.1f;
		// random enemy count from 1 to max with level scaling
		// max is at 2 when scale is 2.0f , 3.0f is at 3.0f and 4.0f is at 4.0f
		// count cannot be more than 4
		var enemyCount = GlobalVariables.GetRandomNumber(1, Mathf.Min((int)(newScale),4));

		//clear all stats
		GlobalVariables.allStats.Clear();
		GlobalVariables.allStats.Add(playerStat);

		//generate new enemy stats
		for (int i = 0; i < enemyCount; i++)
		{
			EnumGlobal.EnemyType enemyIndex = (EnumGlobal.EnemyType)GlobalVariables.GetRandomNumber(0, GlobalVariables.enemyStatsBase.Count - 1);
			var enemyStat = GlobalVariables.enemyStatsBase[enemyIndex].Duplicate() as EnemyStat;
			//random scale from newScale+-0.1
			enemyStat.scaleFactor = newScale + (GD.Randf() * 0.2f - 0.1f);;
			GD.Print("Enemy Scale: ", enemyStat.scaleFactor);
			GlobalVariables.allStats.Add(enemyStat);
		}
		GlobalVariables.globalScale = newScale;

		// save game data
		SaveGame();
	}

}
