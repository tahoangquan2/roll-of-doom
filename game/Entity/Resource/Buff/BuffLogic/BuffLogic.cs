public abstract class BuffLogic
{
    public virtual void OnAttack(Stats attacker, ref int damage, ref int value) { }
    public virtual void OnTakeDamage(Stats target, ref int damage, ref int value) { }
    public virtual void OnCycle(Stats target, ref int value) { }
    public virtual void OnApply(Stats target, ref int value) { }
    public virtual void OnRemove(Stats target, ref int value) { }
    public virtual void OnDefend(Stats target, ref int number, ref int value) { }
}
