using Godot;
using System;
using System.Collections.Generic;

public class WeightedAction : EnemyActionBase
{
    public int Weight { get; set; }
    private Action<EnemyStat, Stats> effect;

    public WeightedAction(
        string name,
        int weight,
        Action<EnemyStat, Stats> effect,
        EnumGlobal.IntentType intentType,
        List<int> values,
        string description = ""
    )
    {
        Name = name;
        Weight = weight;
        this.effect = effect;
        this.intentType = intentType;
        this.description = description;
        Values = values ?? new List<int>();
    }
    public override bool ShouldTrigger(Stats enemy, Stats target) => true;    

    public override void Execute(EnemyStat enemy, Stats target)
    {
        effect(enemy, target);
    }
}
