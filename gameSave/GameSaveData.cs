using Godot;
[GlobalClass]
public partial class GameSaveData : Resource
{   // break down playerstat     
    [Export] public int currentHealth = 0;
    [Export] public int maxHealth = 0;
    [Export] public int capSpellMana = 0;
    [Export] public int baseMana = 0;
    [Export] public int cardDrawPerTurn = 0; // number of cards drawn each turn
    [Export] public int gold = 0; // gold from the start of the run

    //card data
    [Export] public Godot.Collections.Array<int> startingDeckID = new(); // starting deck card id at the start of the run


    [Export] public Godot.Collections.Array<EnumGlobal.EnemyType> enemiesType = new();
    [Export] public Godot.Collections.Array<float> enemiesScale = new(); // enemy stats
    [Export] public float globalScale = 1.0f;
}