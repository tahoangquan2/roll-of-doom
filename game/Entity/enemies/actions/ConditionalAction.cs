using System;
using Godot;

public class ConditionalAction : EnemyActionBase
{
    private Func<Stats, Stats, bool> condition;
    private Action<Stats, Stats> effect;
    public bool Triggered { get; private set; } = false;

    public ConditionalAction(string name, Func<Stats, Stats, bool> condition, Action<Stats, Stats> effect)
    {
        Name = name;
        this.condition = condition;
        this.effect = effect;
    }

    public override bool ShouldTrigger(Stats enemy, Stats target)
    {
        return !Triggered && condition(enemy, target);
    }

    public override void Execute(Stats enemy, Stats target)
    {
        if (Triggered) return;
        Triggered = true;
        //GD.Print($"{enemy.name} uses CONDITIONAL action: {Name}");
        effect(enemy, target);
    }
}
