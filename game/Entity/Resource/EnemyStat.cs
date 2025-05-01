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
    public float scaleFactorAttack = 1.0f;
	public float scaleFactorDefend = 1.0f;

    public void SetupActionsForType(EnumGlobal.EnemyType type,float scaleFactor)    
    {
        enemyType = type;
        EnemyActionLibrary.SetupActionsForType(type, ref WeightedActions, ref conditionalActions, scaleFactor);

        //sort the actions by weight
        WeightedActions.Sort((a, b) => b.Weight.CompareTo(a.Weight));
    }

    private void TakeTurn(Stats target)
    {if (currentHealth <= 0) return; // If dead, do nothing
        foreach (var action in intentedAction)        {
            action.Execute(this, target);
        }        
        PickAction(target);
    }
    // check for conditionals // health change
    private void Conditionals(Stats target)    {   
        foreach (var action in conditionalActions)
        {
            if (action.ShouldTrigger(this, target)) {  
                intentedAction.Clear();
                intentedAction.Add(action);
                EmitSignal(nameof(ActionPicked));
                return;
            }
        }
    }

    //override take damage to check for conditionals
    public override int TakeDamage(int damage, bool IsPrecise = false)    {
        int actualDamage = base.TakeDamage(damage, IsPrecise);
        Conditionals(GlobalVariables.playerStat);
        return actualDamage;
    }
    public override void heal(int value)    {
        base.heal(value);
        Conditionals(GlobalVariables.playerStat);
    }    

    public void PickAction(Stats target)    {
        intentedAction.Clear();
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
		newStat.currentHealth = currentHealth != -1 ? currentHealth : maxHealth;
		
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
