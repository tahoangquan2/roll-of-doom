using System;
using System.Collections.Generic;
using Godot;

public static class EnemyActionLibrary
{
    public static Action<EnemyStat, Stats> Attack(int amount, bool isPrecise = false) =>
        (enemy, target) => enemy.Attack(target, amount * (int)enemy.scaleFactor, isPrecise);

    public static Action<EnemyStat, Stats> HealSelf(int amount) =>
        (enemy, target) => enemy.heal(amount);

    public static Action<EnemyStat, Stats> AddShield(int amount) =>
        (enemy, target) => enemy.Add_shield(amount);

    public static Action<EnemyStat, Stats> AddGuard(int amount) =>
        (enemy, target) => enemy.Add_guard(amount);

    public static Action<EnemyStat, Stats> ApplyBuff(EnumGlobal.BuffType type, int value) =>
        (enemy, target) => enemy.ApplyBuff(type, value);

    public static Action<EnemyStat, Stats> ApplyDebuff(EnumGlobal.BuffType type, int value) =>
        (enemy, target) => target.ApplyBuff(type, value);

    public static Action<EnemyStat, Stats> FilltrashToDiscard(int count,int type) =>
        (enemy, target) =>
        {
            GD.Print("Filling trash to discard pile");
            for (int i = 0; i < count; i++)
            {
                GlobalAccessPoint.GetDiscardPile().AddCard(GlobalVariables.curseCard);
            }
            
        };

    // Conditionals
    public static Func<Stats, Stats, bool> TargetBelowHpPercent(float thresholdPercent) =>
        (enemy, target) => target.currentHealth < target.maxHealth * thresholdPercent;

    public static Func<Stats, Stats, bool> SelfBelowHpPercent(float thresholdPercent) =>
        (enemy, target) => enemy.currentHealth < enemy.maxHealth * thresholdPercent;

    public static Func<Stats, Stats, bool> TargetHasBuff(EnumGlobal.BuffType type) =>
        (enemy, target) => target.GetBuffValue(type) > 0;

    public static Func<Stats, Stats, bool> SelfHasBuff(EnumGlobal.BuffType type) =>
        (enemy, target) => enemy.GetBuffValue(type) > 0;

    public static void SetupActionsForType(
        EnumGlobal.EnemyType type,
        ref List<WeightedAction> weightedActions,
        ref List<ConditionalAction> conditionalActions)
    {
        switch (type)
        {
            case EnumGlobal.EnemyType.Bat:
                weightedActions.Add(new WeightedAction(
                    "Swoop", 4, Attack(5), EnumGlobal.IntentType.Attack,
                    new List<int> { 5 },
                    "Dives swiftly at the enemy"
                ));
                weightedActions.Add(new WeightedAction(
                    "Screech", 2, ApplyDebuff(EnumGlobal.BuffType.Exhaust, 2), EnumGlobal.IntentType.Special,
                    new List<int> { },
                    "Emits a disorienting screech"
                ));
                conditionalActions.Add(new ConditionalAction(
                    "Night Terror",
                    SelfBelowHpPercent(0.3f),
                    new List<Action<EnemyStat, Stats>> {
                        (e, t) => Attack(10, true)(e,t),
                        (e, t) => ApplyDebuff(EnumGlobal.BuffType.Poisoned, 2)(e,t)
                    },
                    EnumGlobal.IntentType.AttackPrecise,
                    new List<int> { 10 },
                    coolDown: 4,
                    description: "Strikes with frenzied ferocity at low health"
                ));
                break;

            case EnumGlobal.EnemyType.Rat:
                weightedActions.Add(new WeightedAction(
                    "Bite", 5, Attack(5), EnumGlobal.IntentType.Attack,
                    new List<int> { 5 },
                    "A quick but weak bite"
                ));
                weightedActions.Add(new WeightedAction(
                    "Scratch", 3, Attack(4), EnumGlobal.IntentType.Attack,
                    new List<int> { 4 },
                    "A sharp claw scratch"
                ));
                weightedActions.Add(new WeightedAction(
                    "Plague", 2, ApplyDebuff(EnumGlobal.BuffType.Exhaust, 2), EnumGlobal.IntentType.Debuff,
                    new List<int> { },
                    "A weak but annoying debuff"
                ));
                conditionalActions.Add(new ConditionalAction(
                    "Skitter",
                    SelfBelowHpPercent(0.5f),
                    new List<Action<EnemyStat, Stats>> {
                        (e, t) => ApplyDebuff(EnumGlobal.BuffType.Fragile, 2)(e,t),
                        (e, t) => ApplyBuff(EnumGlobal.BuffType.Armed, 3)(e,t),
                        (e, t) => ApplyBuff(EnumGlobal.BuffType.Dodge, 2)(e,t)
                    },
                    EnumGlobal.IntentType.Buff,
                    new List<int> { },
                    coolDown: 3,
                    description: "Skitters away to buff"
                ));
                break;

            case EnumGlobal.EnemyType.Rat2:
                weightedActions.Add(new WeightedAction(
                    "Fierce Bite", 4, Attack(3), EnumGlobal.IntentType.Attack,
                    new List<int> { 3 },
                    "A stronger bite"
                ));
                weightedActions.Add(new WeightedAction(
                    "Savage Tear", 2, Attack(4), EnumGlobal.IntentType.Attack,
                    new List<int> { 4 },
                    "Rips at the enemy fiercely"
                ));
                conditionalActions.Add(new ConditionalAction(
                    "Bloodlust",
                    TargetBelowHpPercent(0.4f),
                    new List<Action<EnemyStat, Stats>> {
                        (e, t) => Attack(5)(e,t),
                        (e, t) => Attack(5)(e,t),
                        (e, t) => ApplyBuff(EnumGlobal.BuffType.Armed, 10)(e,t)
                    },
                    EnumGlobal.IntentType.Special,
                    new List<int> { 5, 5 },
                    coolDown: 4,
                    description: "Lands a devastating blow on weakened foes"
                ));
                break;

            case EnumGlobal.EnemyType.Spider:
                weightedActions.Add(new WeightedAction(
                    "Spider sense", 2, ApplyBuff(EnumGlobal.BuffType.Dodge,2), EnumGlobal.IntentType.Defend,
                    new List<int> { },
                    "Sensing attack "
                ));
                weightedActions.Add(new WeightedAction(
                    "Web Shot", 4, ApplyDebuff(EnumGlobal.BuffType.Poisoned, 2), EnumGlobal.IntentType.Debuff,
                    new List<int> { },
                    "Fires venomous web to poison"
                ));
                weightedActions.Add(new WeightedAction(
                    "Bite", 3, Attack(6), EnumGlobal.IntentType.Attack,
                    new List<int> { 6 },
                    "Delivers a venomous bite"
                ));
                conditionalActions.Add(new ConditionalAction(
                    "Venom Burst",
                    TargetHasBuff(EnumGlobal.BuffType.Poisoned),
                    new List<Action<EnemyStat, Stats>> {
                        (e, t) => e.Attack(t, 5, true),
                        (e, t) => ApplyDebuff(EnumGlobal.BuffType.Poisoned, 4)(e,t)
                    },
                    EnumGlobal.IntentType.Special,
                    new List<int> { 4 },
                    coolDown: 5,
                    description: "Triggers a burst of toxic venom"
                ));
                break;

            case EnumGlobal.EnemyType.Ghost:
                weightedActions.Add(new WeightedAction(
                    "Haunt", 3, ApplyDebuff(EnumGlobal.BuffType.Fragile, 3), EnumGlobal.IntentType.Special,
                    new List<int> { },
                    "Haunts to weaken defenses"
                ));
                weightedActions.Add(new WeightedAction(
                    "Chill Touch", 4, Attack(5, true), EnumGlobal.IntentType.AttackPrecise,
                    new List<int> { 5 },
                    "A precise chilling strike"
                ));
                weightedActions.Add(new WeightedAction(
                    "Phasing", 1, ApplyBuff(EnumGlobal.BuffType.Dodge, 3), EnumGlobal.IntentType.Buff,
                    new List<int> { },
                    "Phases to dodge attacks"
                ));
                conditionalActions.Add(new ConditionalAction(
                    "Curse",
                    SelfBelowHpPercent(0.5f),
                    new List<Action<EnemyStat, Stats>> {
                        (e, t) => FilltrashToDiscard(3,0)(e,t),
                        (e, t) => e.ApplyBuff(EnumGlobal.BuffType.Dodge, 4)
                    },
                    EnumGlobal.IntentType.Special,
                    new List<int> { },
                    coolDown: 5,
                    description: "Phasing away to curse player discard pile"
                ));
                break;

            case EnumGlobal.EnemyType.Ghost2:
                weightedActions.Add(new WeightedAction(
                    "Haunt", 2, ApplyDebuff(EnumGlobal.BuffType.Fragile, 3), EnumGlobal.IntentType.Debuff,
                    new List<int> { },
                    "Haunts to weaken defenses"
                ));
                weightedActions.Add(new WeightedAction(
                    "Chill Touch", 3, Attack(8, true), EnumGlobal.IntentType.AttackPrecise,
                    new List<int> { 8 },
                    "A precise icy strike"
                ));
                weightedActions.Add(new WeightedAction(
                    "Phasing", 1, ApplyBuff(EnumGlobal.BuffType.Dodge, 3), EnumGlobal.IntentType.Buff,
                    new List<int> { },
                    "Phases to dodge attacks"
                ));
                conditionalActions.Add(new ConditionalAction(
                    "Haunt",
                    SelfHasBuff(EnumGlobal.BuffType.Dodge),
                    new List<Action<EnemyStat, Stats>> {
                        (e, t) => Attack(13, true)(e,t),
                        (e, t) => t.ApplyBuff(EnumGlobal.BuffType.Exhaust, 2)
                    },
                    EnumGlobal.IntentType.AttackPrecise,
                    new List<int> { 13 },
                    coolDown: 3,
                    description: "Sneak strikes precisely weakened foes"
                ));
                conditionalActions.Add(new ConditionalAction(
                    "Spectral Onslaught",
                    SelfBelowHpPercent(0.2f),
                    new List<Action<EnemyStat, Stats>> { 
                        (e,t) => t.ApplyBuff(EnumGlobal.BuffType.Exhaust,10),
                        (e,t) => t.ApplyBuff(EnumGlobal.BuffType.Fragile,10)
                    },
                    EnumGlobal.IntentType.Debuff,
                    new List<int> {},
                    coolDown: 10,
                    description: "Heavy debuffing"
                ));
                break;

            case EnumGlobal.EnemyType.Orc:
                weightedActions.Add(new WeightedAction(
                    "Club Smash", 6, Attack(8), EnumGlobal.IntentType.Attack,
                    new List<int> { 8 },
                    "A heavy club smash"
                ));
                weightedActions.Add(new WeightedAction(
                    "Berserk", 4, ApplyBuff(EnumGlobal.BuffType.Armed, 3), EnumGlobal.IntentType.Buff,
                    new List<int> { 3 },
                    "A surge of strength"
                ));
                weightedActions.Add(new WeightedAction(
                    "Club Defend", 1, AddGuard(7), EnumGlobal.IntentType.Defend,
                    new List<int> {  },
                    "A defensive guard up"
                ));
                conditionalActions.Add(new ConditionalAction(
                    "Rage Recovery",
                    SelfBelowHpPercent(0.3f),
                    new List<Action<EnemyStat, Stats>> {
                        (e, t) => e.heal(17),
                        (e, t) => e.ApplyBuff(EnumGlobal.BuffType.Armed, 5)
                    },
                    EnumGlobal.IntentType.Heal,
                    new List<int> { 17 },
                    coolDown: 5,
                    description: "Recovers health and strength in rage"
                ));
                break;

            case EnumGlobal.EnemyType.Krab:
                weightedActions.Add(new WeightedAction(
                    "Claw Pinch", 5, Attack(7), EnumGlobal.IntentType.Attack,
                    new List<int> { 7 },
                    "A quick pinch"
                ));
                weightedActions.Add(new WeightedAction(
                    "Turtle Up", 4, AddGuard(10), EnumGlobal.IntentType.Defend,
                    new List<int> { 12 },
                    "A defensive stance"
                ));
                weightedActions.Add(new WeightedAction(
                    "Shell Guard", 2, ApplyBuff(EnumGlobal.BuffType.Fortify, 3), EnumGlobal.IntentType.Buff,
                    new List<int> { },
                    "Shell fortification"
                ));
                conditionalActions.Add(new ConditionalAction(
                    "Iron Shell",
                    SelfBelowHpPercent(0.5f),
                    new List<Action<EnemyStat, Stats>> {
                        (e, t) => AddShield(25)(e,t),
                        (e, t) => t.ApplyBuff(EnumGlobal.BuffType.Fragile, 3)
                    },
                    EnumGlobal.IntentType.Special,
                    new List<int> { 25 },
                    coolDown: 5,
                    description: "Solid shell retaliation"
                ));
                conditionalActions.Add(new ConditionalAction(
                    "Shell Slam",
                    TargetHasBuff(EnumGlobal.BuffType.Fragile),
                    new List<Action<EnemyStat, Stats>> {
                        (e, t) => AddGuard(8)(e,t),
                        (e, t) => Attack(5 + e.shield + e.guard,false)(e,t)
                    },
                    EnumGlobal.IntentType.AttackPrecise,
                    new List<int> { 5 },
                    coolDown: 0,
                    description: "Smashes fragile foes with shield and guard"
                ));
                break;

            case EnumGlobal.EnemyType.Necromancer:

                weightedActions.Add(new WeightedAction(
                    "Umbra Barrier", 6, AddShield(10), EnumGlobal.IntentType.Defend,
                    new List<int> {},
                    "Raises a barrier of dark energy"
                ));
                weightedActions.Add(new WeightedAction(
                    "Dark Bolt", 4, Attack(10), EnumGlobal.IntentType.Attack,
                    new List<int> { 10 },
                    "A bolt of dark energy"
                ));
                weightedActions.Add(new WeightedAction(
                    "Bone Shield", 1, ApplyBuff(EnumGlobal.BuffType.Fortify, 3), EnumGlobal.IntentType.Defend,
                    new List<int> {  },
                    "Raises a shield of bones"
                ));
                conditionalActions.Add(new ConditionalAction(
                    "Necrotic Surge",
                    SelfBelowHpPercent(0.3f),
                    new List<Action<EnemyStat, Stats>> {
                        (e, t) => Attack(15)(e,t),
                        (e, t) => AddShield(15)(e,t),
                    },
                    EnumGlobal.IntentType.Special,
                    new List<int> { 15 },
                    coolDown: 2,
                    description: "Mastery of dark energy"
                ));
                conditionalActions.Add(new ConditionalAction(
                    "Curse of the Undead",
                    SelfBelowHpPercent(0.5f),
                    new List<Action<EnemyStat, Stats>> {
                        (e, t) => FilltrashToDiscard(5,0)(e,t),
                        (e, t) => ApplyDebuff(EnumGlobal.BuffType.Fragile, 3)(e,t)
                    },
                    EnumGlobal.IntentType.Debuff,
                    new List<int> { },
                    coolDown: 6,
                    description: "Curses the enemy with mind shackles"
                ));
                conditionalActions.Add(new ConditionalAction(
                    "Rebirth",
                    SelfBelowHpPercent(0.7f),
                    new List<Action<EnemyStat, Stats>> {
                        (e, t) => e.heal(13),
                        (e, t) => ApplyBuff(EnumGlobal.BuffType.Fortify, 2)(e,t)
                    },
                    EnumGlobal.IntentType.Buff,
                    new List<int> { },
                    coolDown: 6,
                    description: "Absorbs life force to heal and fortify"
                ));
                

                break;

            default:
                break;
        }
    }

}
