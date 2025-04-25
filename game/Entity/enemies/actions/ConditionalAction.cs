using System;
using System.Collections.Generic;
using Godot;

public class ConditionalAction : EnemyActionBase
{
    private Func<Stats, Stats, bool> condition;
    private List<Action<EnemyStat, Stats>> effect; // multiple actions can be chained together
    public bool Triggered { get; private set; } = false;
    public int baseCooldown = 0; // base cooldown for the action

    private int coolDownCounter = 0; // counter for cooldown


    public ConditionalAction(string name, Func<Stats, Stats, bool> condition, 
        List<Action<EnemyStat, Stats>> effect,EnumGlobal.IntentType intentType, 
        int coolDown = 99, string description = ""  )    
    {
        Name = name;
        this.condition = condition;
        this.effect = effect;
        baseCooldown = coolDown;
        this.description = description;
        this.intentType = intentType;
    }

    public void coolDown(){
        if (coolDownCounter > 0) coolDownCounter--;
        
        else Triggered = false; // reset the trigger
    }

    public override bool ShouldTrigger(Stats enemy, Stats target)
    {
        return !Triggered && condition(enemy, target);
    }

    public override void Execute(EnemyStat enemy, Stats target)
    {
        if (Triggered) return;
        Triggered = true;
        // Execute all effects in the list
        foreach (var singleEffect in effect)
        {
            singleEffect(enemy, target);
        }

        coolDownCounter = baseCooldown; // reset cooldown
    }
}
