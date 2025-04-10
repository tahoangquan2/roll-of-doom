public class ArmedLogic : BuffLogic
{
	public override void OnAttack(Stats attacker,  ref int damage, ref int value)
	{
		damage+= value;
	}
}
