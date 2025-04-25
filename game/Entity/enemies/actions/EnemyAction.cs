using System;

public abstract class EnemyActionBase
{
    public string Name;
    public int value;
    public string description;
    public EnumGlobal.IntentType intentType=0;
    
    public abstract bool ShouldTrigger(Stats enemy, Stats target);
    public abstract void Execute(EnemyStat enemy, Stats target);
}
