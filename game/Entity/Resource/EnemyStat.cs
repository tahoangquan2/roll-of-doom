using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[Tool]
[GlobalClass]

public partial class EnemyStat : Stats // additional enemy Actions 
{
	private List<ConditionalAction> conditionalActions = new();
    private List<WeightedAction> WeightedActions = new();
    private Random random = new();

    public void _Ready()
    {
        SetupActions();
    }

    private void SetupActions() // for testing
    {
        // Conditional: only if target HP is below 50%
        conditionalActions.Add(new ConditionalAction(
            "Execute",
            (enemy, target) => target.currentHealth < target.maxHealth / 2,
            (enemy, target) => target.TakeDamage(15)
        ));

        // Random weighted actions
        WeightedActions.Add(new WeightedAction("Strike", 5, (from, to) => Attack(to, 5)));
        WeightedActions.Add(new WeightedAction("Defend", 2, (from, to) => Add_shield(8)));
        WeightedActions.Add(new WeightedAction("Weaken", 1, (from, to) => ApplyBuff(EnumGlobal.BuffType.Exhaust, 1)));
    }

    public void TakeTurn(Stats target)
    {
        // Check conditionals first
        foreach (var action in conditionalActions)
        {
            if (action.ShouldTrigger(this, target))
            {
                action.Execute(this, target);
                return;
            }
        }

        // If no conditionals triggered, pick a random one by weight
        int totalWeight = 0;
        foreach (var a in WeightedActions) totalWeight += a.Weight;
        int roll = random.Next(totalWeight);

        int cumulative = 0;
        foreach (var action in WeightedActions)
        {
            cumulative += action.Weight;
            if (roll < cumulative)
            {
                action.Execute(this, target);
                return;
            }
        }
    }

	public override EnemyStat CreateInstance()
	{
		EnemyStat newStat = Duplicate() as EnemyStat;
		newStat.maxHealth = maxHealth;
		newStat.name = name;
		newStat.currentHealth = currentHealth;
		
		newStat.random = new Random();
		newStat.conditionalActions = new List<ConditionalAction>(conditionalActions);
		newStat.WeightedActions = new List<WeightedAction>(WeightedActions);

		return newStat;
	}

    public async Task EnemyTurn()
    {
        GD.Print($"{name} is taking its turn.");      
    }
}
