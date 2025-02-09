using Godot;
using System;
public partial class GlobalVariables : Node
{
    public static Vector2 cardSize= new Vector2(150, 210);

    // get random number with range
    public static int GetRandomNumber(int min, int max)
    {
        return new Random().Next(min, max);
    }
}
