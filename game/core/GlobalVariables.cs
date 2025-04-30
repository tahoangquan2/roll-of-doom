using Godot;
using System;
using System.Collections.Generic;
public partial class GlobalVariables : Node
{
    public static Vector2 cardSize= new Vector2(138, 210);
    public static int maxStackSize = 10;
    public static PlayerStat playerStat=null;   
    public static List<Stats> allStats= new List<Stats>(); 
    public static List<Character> allCharacters= new List<Character>();
    public static Dictionary<Stats,Character> allCharacterStats = new Dictionary<Stats,Character>();
    public static BuffDatabase buffDatabase;    
    public static List<Texture2D> intentTextures = new List<Texture2D>();
    public static List<EnemyStat> enemyStatsBase = new List<EnemyStat>();

    public static Godot.Collections.Array<CardData> cardPool = new Godot.Collections.Array<CardData>();
    public static CardData curseCard = null;

    // get random number with range
    public static int GetRandomNumber(int min, int max)
    {
        return new Random().Next(min, max+1);
    }
    public override void _Ready()
    {
        buffDatabase = new BuffDatabase();
        AddChild(buffDatabase);

        // load intent textures
        foreach (var file in DirAccess.GetFilesAt("res://assets/sprites/intent/")) {
            if (file.EndsWith(".png")) {
                var texture = ResourceLoader.Load<Texture2D>("res://assets/sprites/intent/"+file);
                intentTextures.Add(texture);
            }
        }

        // load all enemy stats
        foreach (var file in DirAccess.GetFilesAt("res://game/Entity/enemies/stats/")) {
            if (file.EndsWith(".tres")) {
                var enemy = ResourceLoader.Load<EnemyStat>("res://game/Entity/enemies/stats/"+file);
                enemyStatsBase.Add(enemy);
            }
        }

        // load all cards
        foreach (var file in DirAccess.GetFilesAt("res://game/cards/CardData/CardPool/")) {
            if (file.EndsWith(".tres")) {
                var card = ResourceLoader.Load<CardData>("res://game/cards/CardData/CardPool/"+file);
                cardPool.Add(card);
            }
        }
        // load curse card
        curseCard = ResourceLoader.Load<CardData>("res://game/cards/CardData/Curse/Curse.tres");
    }

    public static PackedScene mapScene = ResourceLoader.Load<PackedScene>("res://game/levels/LevelMap.tscn");
    public static PackedScene battleScene = ResourceLoader.Load<PackedScene>("res://game/Main.tscn");
    public static PackedScene mainMenuScene = ResourceLoader.Load<PackedScene>("res://game/gui/main_menu.tscn");

    public static MapData SavedMapData = null;

    public static int passedValue = 0;
    public static int getPassedValue() // get then reset passedValue
    {
        int temp = passedValue;
        passedValue = 0;
        return temp;
    }

    public static Godot.Collections.Array<Stats> GetAllCharaterStat()
    {
        Godot.Collections.Array<Stats> alltheStat = new Godot.Collections.Array<Stats>(allStats);
        
        return alltheStat;
    }
}