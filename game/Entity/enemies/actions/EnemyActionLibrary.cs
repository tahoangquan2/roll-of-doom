using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public static class EnemyActionLibrary
{
    public static Action<EnemyStat, Stats> Attack(int amount,bool isPrecise=false) =>
        (enemy, target) => enemy.Attack(target, amount*(int)enemy.scaleFactor, isPrecise);
    public static Action<EnemyStat, Stats> HealSelf(int amount) =>
        (enemy, target) => enemy.heal(amount);

    public static Action<EnemyStat, Stats> AddShield(int amount) =>
        (enemy, target) => enemy.Add_shield(amount);

	public static Action<EnemyStat, Stats> AddGuard(int amount) =>
		(enemy, target) => enemy.Add_guard(amount);

    public static Action<EnemyStat, Stats> ApplyBuff(EnumGlobal.BuffType type, int value) =>
        (enemy, target) => enemy.ApplyBuff(type, value);

	public static Action<EnemyStat, Stats> ApplyDebuff(EnumGlobal.BuffType type,int value) =>
		(enemy, target) => target.ApplyBuff(type,value);

    public static Func<Stats, Stats, bool> TargetBelowHpPercent(float thresholdPercent) =>
        (enemy, target) => target.currentHealth < target.maxHealth * thresholdPercent;

    public static Func<Stats, Stats, bool> SelfBelowHpPercent(float thresholdPercent) =>
        (enemy, target) => enemy.currentHealth < enemy.maxHealth * thresholdPercent;

	public static Func<Stats, Stats, bool> TargetHasBuff(EnumGlobal.BuffType type) =>
		(enemy, target) => target.GetBuffValue(type) > 0; 	

	public static void SetupActionsForType(EnumGlobal.EnemyType type, 
	ref List<WeightedAction> weightedActions, 
	ref List<ConditionalAction> conditionalActions)
	{
		switch (type)
	{
		case EnumGlobal.EnemyType.Goblin:
			weightedActions.Add(new WeightedAction("Poke", 3, Attack(4),EnumGlobal.IntentType.Attack, "A quick jab with a stick"));
			weightedActions.Add(new WeightedAction("Jitter", 2, AddGuard(6),EnumGlobal.IntentType.Defend, "A defensive stance"));

			conditionalActions.Add(new ConditionalAction(
				"Desperate Swing",
				SelfBelowHpPercent(0.5f),
				new List<Action<EnemyStat, Stats>> {
					(enemy, target) => Attack(10,true),(enemy, target) => enemy.Add_shield(5)
				}, EnumGlobal.IntentType.AttackPrecise, 5, "A wild swing in desperation"		
			));
			break;

		case EnumGlobal.EnemyType.Skeleton:
			weightedActions.Add(new WeightedAction("Bone Throw", 4, Attack(4), EnumGlobal.IntentType.Attack, "Bone throw with a bone"));
			weightedActions.Add(new WeightedAction("Rattle Guard", 2, AddShield(5),EnumGlobal.IntentType.Defend, "A rattling defense"));

			conditionalActions.Add(new ConditionalAction(
				"Death Dance",
				TargetBelowHpPercent(0.4f),

				new List<Action<EnemyStat, Stats>> {
					(enemy, target) => enemy.Attack(target, 12),
					(enemy, target) => enemy.Attack(target, 14),
				}, EnumGlobal.IntentType.AttackPrecise,4, "A flurry of attacks"
			));
			break;

		case EnumGlobal.EnemyType.Orc:
			weightedActions.Add(new WeightedAction("Club Smash", 6, Attack(5),EnumGlobal.IntentType.Attack, "A heavy club smash"));
			weightedActions.Add(new WeightedAction("Berserk", 4, ApplyBuff(EnumGlobal.BuffType.Armed, 2),EnumGlobal.IntentType.Buff, "A surge of strength"));
			weightedActions.Add(new WeightedAction("Club defend", 1, AddGuard(7),EnumGlobal.IntentType.Defend, "A defensive stance"));

			conditionalActions.Add(new ConditionalAction(
				"Rage Recovery",
				SelfBelowHpPercent(0.3f),
				new List<Action<EnemyStat, Stats>> {
					(enemy, target) => enemy.heal(10),
					(enemy, target) => enemy.ApplyBuff(EnumGlobal.BuffType.Armed, 1)
				},EnumGlobal.IntentType.Heal,5, "A surge of adrenaline"
			));
			break;

		case EnumGlobal.EnemyType.Krab:
			weightedActions.Add(new WeightedAction("Claw Pinch", 4, Attack(4),EnumGlobal.IntentType.Attack, "A quick pinch"));
			weightedActions.Add(new WeightedAction("Turtle Up", 3, AddShield(7),EnumGlobal.IntentType.Defend, "A defensive stance"));
			weightedActions.Add(new WeightedAction("Shell Guard", 2, ApplyBuff(EnumGlobal.BuffType.Fortify, 2),EnumGlobal.IntentType.Buff, "Shell fortification"));
			conditionalActions.Add(new ConditionalAction(
				"Iron Shell",
				SelfBelowHpPercent(0.5f),
				new List<Action<EnemyStat, Stats>> {
					(enemy, target) => enemy.Add_shield(15),
					(enemy, target) => target.ApplyBuff(EnumGlobal.BuffType.Fragile, 2)
				},EnumGlobal.IntentType.Special,5, "Shell of steel"
			));

			conditionalActions.Add(new ConditionalAction(
				"Shell Slam",
				TargetHasBuff(EnumGlobal.BuffType.Fragile),
				new List<Action<EnemyStat, Stats>> {
					(enemy, target) => enemy.Add_guard(5),
					(enemy, target) => enemy.Attack(target,enemy.shield+enemy.guard)
				},EnumGlobal.IntentType.AttackPrecise
				,0, "When player has Fragile, deal damage equal to shield + guard"
			));
			break;

		default:
			break;
	}
	}
}


