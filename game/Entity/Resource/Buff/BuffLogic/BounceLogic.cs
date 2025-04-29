using Godot;

public class BounceLogic : BuffLogic
{
	public override void OnTakeDamage(Stats target, ref int damage, ref int value)
	{   
        target.AttackRandom(damage);
        damage = 0; 
		value--;
	}
}
