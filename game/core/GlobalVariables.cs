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
    }

    public static PackedScene mapScene = ResourceLoader.Load<PackedScene>("res://game/levels/LevelMap.tscn");
    public static PackedScene battleScene = ResourceLoader.Load<PackedScene>("res://game/Main.tscn");
    public static PackedScene mainMenuScene = ResourceLoader.Load<PackedScene>("res://game/gui/main_menu.tscn");
}