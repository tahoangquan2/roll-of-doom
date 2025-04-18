using Godot;
using System;

public class WeightedAction : EnemyActionBase
{
    public int Weight { get; set; }
    private Action<EnemyStat, Stats> effect;

    public WeightedAction(string name, int weight, Action<EnemyStat, Stats> effect)
    {
        Name = name;
        Weight = weight;
        this.effect = effect;
    }

    public override bool ShouldTrigger(Stats enemy, Stats target)
    {
        return true;
    }

    public override void Execute(EnemyStat enemy, Stats target)
    {
        GD.Print($"{enemy.name} uses Random action: {Name}");
        effect(enemy, target);
    }
}
