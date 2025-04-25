using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[Tool]
[GlobalClass]

public partial class EnemyStat : Stats // additional enemy Actions 
{
    [Export] public EnumGlobal.EnemyType enemyType; // Add this at the top of the class
	private List<ConditionalAction> conditionalActions = new();
    private List<WeightedAction> WeightedActions = new();
    private Random random = new();

    public List<EnemyActionBase> intentedAction = new(); 
    // after enemy turn, enemy will pick an action 
    // at start of enemy turn, enemy will perform an action from this list
    [Signal] public delegate void ActionPickedEventHandler();

    [Export] public float scaleFactor = 1.0f;

    public void SetupActionsForType(EnumGlobal.EnemyType type)
    {
        enemyType = type;
        EnemyActionLibrary.SetupActionsForType(type, ref WeightedActions, ref conditionalActions);

        //sort the actions by weight
        WeightedActions.Sort((a, b) => b.Weight.CompareTo(a.Weight));
    }

    private void TakeTurn(Stats target)
    {
        foreach (var action in intentedAction)
        {
            action.Execute(this, target);
        }
        intentedAction.Clear();
        PickAction(target);
    }

    public void PickAction(Stats target)
    {
        GD.Print("Picking Action");
        foreach (var action in conditionalActions)
        {
            action.coolDown();
            if (action.ShouldTrigger(this, target))
            {
                intentedAction.Add(action);
                EmitSignal(nameof(ActionPicked));
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
                intentedAction.Add(action);
                EmitSignal(nameof(ActionPicked));
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
        Cycle();
        TakeTurn(GlobalVariables.playerStat); 
    }
}
