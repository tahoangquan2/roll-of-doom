using Godot;
using System;
public partial class GlobalVariables : Node
{
    public static Vector2 cardSize= new Vector2(138, 210);
    public static int maxStackSize = 10;
    public static GlobalVariables gv;
    public static PlayerStat playerStat;

    // get random number with range
    public static int GetRandomNumber(int min, int max)
    {
        return new Random().Next(min, max+1);
    }
    public override void _Ready()
    {
        gv = this;
    }
}
