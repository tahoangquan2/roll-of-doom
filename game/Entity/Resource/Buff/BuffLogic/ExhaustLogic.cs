public class ExhaustLogic : BuffLogic
{
	public override void OnAttack(Stats attacker, ref int damage, ref int value)
	{
        damage = Godot.Mathf.RoundToInt(damage * 0.7f);
		Godot.GD.Print($"{attacker.name} attacked with reduced damage: {damage}");
	}
}
