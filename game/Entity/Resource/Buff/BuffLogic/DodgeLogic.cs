using Godot;

public class DodgeLogic : BuffLogic
{
    public override void OnTakeDamage(Stats target, ref int damage,ref int value)
    {
        damage = 0;
        value -= 1; 
        GD.Print($"{target.name} dodged the attack! Remaining value: {value}");
    }
}
