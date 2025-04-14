public class FragileLogic : BuffLogic
{
    public override void OnTakeDamage(Stats target, ref int damage,ref int value)
    {
        damage = Godot.Mathf.RoundToInt(damage * 1.3f);
    }
}
