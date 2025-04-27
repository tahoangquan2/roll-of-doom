using System.Collections.Generic;

public abstract class EnemyActionBase
{
    public string Name;
    public List<int> Values { get; set; } = new();
    public string description;
    public EnumGlobal.IntentType intentType=0;
    
    public abstract bool ShouldTrigger(Stats enemy, Stats target);
    public abstract void Execute(EnemyStat enemy, Stats target);
}
