using Godot;
using System;
public partial class GlobalVariables : Node
{
    public static Vector2 cardSize= new Vector2(138, 210);

    public static int health = 75;
    public static int maxHealth = 100;
    public static int spirit = 500;
    public static int maxStackSize = 10;

    public static int gold = 1420;

    public static GlobalVariables gv;

    // get random number with range
    public static int GetRandomNumber(int min, int max)
    {
        return new Random().Next(min, max+1);
    }
    public override void _Ready()
    {
        gv = this;
    }

    public static void ChangeHealth(int value)
    {
        health += value;
        if (health > maxHealth) health = maxHealth;
        if (health < 0) health = 0;
        gv.EmitSignal(nameof(HealthChanged));
    }
    public static void ChangeSpirit(int value)
    {
       spirit += value;
       if (spirit < 0) spirit = 0;
       gv.EmitSignal(nameof(SpiritChanged));
    }

    [Signal] public delegate void HealthChangedEventHandler();
    [Signal] public delegate void SpiritChangedEventHandler();
}
