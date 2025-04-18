public abstract class EnemyActionBase
{
    public string Name;
    public abstract bool ShouldTrigger(Stats enemy, Stats target);
    public abstract void Execute(EnemyStat enemy, Stats target);
}
