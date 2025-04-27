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
                    "Swoop", 4, Attack(3), EnumGlobal.IntentType.Attack,
                    new List<int> { 3 },
                    "Dives swiftly at the enemy"
                ));
                weightedActions.Add(new WeightedAction(
                    "Screech", 2, ApplyDebuff(EnumGlobal.BuffType.Exhaust, 2), EnumGlobal.IntentType.Special,
                    new List<int> (),
                    "Emits a disorienting screech"
                ));
                conditionalActions.Add(new ConditionalAction(
                    "Night Terror",
                    SelfBelowHpPercent(0.5f),
                    new List<Action<EnemyStat, Stats>> {
                        (e, t) => e.Attack(t, 5, true),
                        (e, t) => t.ApplyBuff(EnumGlobal.BuffType.Poisoned, 4)
                    },
                    EnumGlobal.IntentType.AttackPrecise,
                    new List<int> { 5 },
                    coolDown: 4,
                    description: "Strikes with frenzied ferocity at low health"
                ));
                break;

            case EnumGlobal.EnemyType.Rat:
                weightedActions.Add(new WeightedAction(
                    "Bite", 5, Attack(3), EnumGlobal.IntentType.Attack,
                    new List<int> { 3 },
                    "A quick but weak bite"
                ));
                weightedActions.Add(new WeightedAction(
                    "Scratch", 3, Attack(2), EnumGlobal.IntentType.Attack,
                    new List<int> { 2 },
                    "A sharp claw scratch"
                ));
                //aply debuff exhaust
                weightedActions.Add(new WeightedAction(
                    "Plague", 2, ApplyDebuff(EnumGlobal.BuffType.Exhaust, 2), EnumGlobal.IntentType.Debuff,
                    new List<int> { },
                    "A weak but annoying debuff"
                ));

                conditionalActions.Add(new ConditionalAction(
                    "Rabid Frenzy",
                    SelfBelowHpPercent(0.5f),
                    new List<Action<EnemyStat, Stats>> {
                        (e,t) => e.Attack(t,4, true),
                        (e,t) => e.Attack(t,4, true)
                    },
                    EnumGlobal.IntentType.AttackPrecise,
                    new List<int> { 4, 4 },
                    coolDown: 3,
                    description: "Becomes frenzied when hurt"
                ));
                break;

            case EnumGlobal.EnemyType.Rat2:
                weightedActions.Add(new WeightedAction(
                    "Fierce Bite", 4, Attack(4), EnumGlobal.IntentType.Attack,
                    new List<int> { 5 },
                    "A stronger bite"
                ));
                weightedActions.Add(new WeightedAction(
                    "Savage Tear", 2, Attack(5), EnumGlobal.IntentType.Attack,
                    new List<int> { 6 },
                    "Rips at the enemy fiercely"
                ));
                conditionalActions.Add(new ConditionalAction(
                    "Bloodlust",
                    TargetBelowHpPercent(0.3f),
                    new List<Action<EnemyStat, Stats>> {
                        (e,t) => e.Attack(t,3),
                        (e,t) => e.Attack(t,3),
                        (e,t) => e.ApplyBuff(EnumGlobal.BuffType.Armed, 6)
                    },
                    EnumGlobal.IntentType.Special,
                    new List<int> { 3,3 },
                    coolDown: 4,
                    description: "Lands a devastating blow on weakened foes"
                ));
                break;

            case EnumGlobal.EnemyType.Spider:
                weightedActions.Add(new WeightedAction(
                    "Web Shot", 3, ApplyDebuff(EnumGlobal.BuffType.Poisoned, 3), EnumGlobal.IntentType.Debuff,
                    new List<int> { },
                    "Fires venomous web to poison"
                ));
                weightedActions.Add(new WeightedAction(
                    "Bite", 2, Attack(4), EnumGlobal.IntentType.Attack,
                    new List<int> { 4 },
                    "Delivers a venomous bite"
                ));
                conditionalActions.Add(new ConditionalAction(
                    "Venom Burst",
                    TargetHasBuff(EnumGlobal.BuffType.Poisoned),
                    new List<Action<EnemyStat, Stats>> {
                        (e,t) => e.Attack(t, 7, true),
                        (e,t) => t.ApplyBuff(EnumGlobal.BuffType.Poisoned, 4)
                    },
                    EnumGlobal.IntentType.Special,
                    new List<int> { 7},
                    coolDown: 5,
                    description: "Triggers a burst of toxic venom deal damage and self poison"
                ));
                break;

            case EnumGlobal.EnemyType.Ghost:
                weightedActions.Add(new WeightedAction(
                    "Haunt", 3, ApplyDebuff(EnumGlobal.BuffType.Fragile, 2), EnumGlobal.IntentType.Special,
                    new List<int> {},
                    "Haunts to weaken defenses"
                ));
                weightedActions.Add(new WeightedAction(
                    "Chill Touch", 4, Attack(3, true), EnumGlobal.IntentType.AttackPrecise,
                    new List<int> { 3 },
                    "A precise chilling strike"
                ));
                weightedActions.Add(new WeightedAction(
                    "Phasing", 1, ApplyBuff(EnumGlobal.BuffType.Dodge,3), EnumGlobal.IntentType.Buff,
                    new List<int> (),
                    "Phases to dodge attacks"
                ));
                conditionalActions.Add(new ConditionalAction(
                    "Phantom Strike",
                    SelfBelowHpPercent(0.5f),
                    new List<Action<EnemyStat, Stats>> {
                        (e,t) => e.Attack(t,6, true),
                        (e,t) => e.ApplyBuff(EnumGlobal.BuffType.Dodge, 3)
                    },
                    EnumGlobal.IntentType.Special,
                    new List<int> { 6 },
                    coolDown: 4,
                    description: "Strikes precisely weakened foes"
                ));
                break;

            case EnumGlobal.EnemyType.Ghost2:
                weightedActions.Add(new WeightedAction(
                    "Haunt", 2, ApplyDebuff(EnumGlobal.BuffType.Fragile, 2), EnumGlobal.IntentType.Debuff,
                    new List<int> (),
                    "Stronger haunting"
                ));
                weightedActions.Add(new WeightedAction(
                    "Chill Touch", 3, Attack(5, true), EnumGlobal.IntentType.AttackPrecise,
                    new List<int> { 5 },
                    "A precise icy strike"
                ));
                weightedActions.Add(new WeightedAction(
                    "Phasing", 1, ApplyBuff(EnumGlobal.BuffType.Dodge, 2), EnumGlobal.IntentType.Buff,
                    new List<int> (),
                    "Phases to dodge attacks"
                ));
                conditionalActions.Add(new ConditionalAction(
                    "Haunt",
                    SelfHasBuff(EnumGlobal.BuffType.Dodge),
                    new List<Action<EnemyStat, Stats>> {
                        (e,t) => e.Attack(t,8, true),
                        //aplly Fragile
                        (e,t) => ApplyDebuff(EnumGlobal.BuffType.Fragile, 2)
                    },
                    EnumGlobal.IntentType.AttackPrecise,
                    new List<int> { 8 },
                    coolDown: 3,
                    description: "Strikes precisely weakened foes"
                ));
                conditionalActions.Add(new ConditionalAction(
                    "Spectral Onslaught",
                    SelfBelowHpPercent(0.5f),
                    new List<Action<EnemyStat, Stats>> {
                        (e,t) => e.Attack(t,4, true),
                        (e,t) => e.Attack(t,4, true)
                    },
                    EnumGlobal.IntentType.AttackPrecise,
                    new List<int> { 4, 4 },
                    coolDown: 5,
                    description: "Unleashes a flurry of spectral strikes"
                ));
                break;

            case EnumGlobal.EnemyType.Orc:
                weightedActions.Add(new WeightedAction(
                    "Club Smash", 6, Attack(5), EnumGlobal.IntentType.Attack,
                    new List<int> { 5 },
                    "A heavy club smash"
                ));
                weightedActions.Add(new WeightedAction(
                    "Berserk", 4, ApplyBuff(EnumGlobal.BuffType.Armed, 2), EnumGlobal.IntentType.Buff,
                    new List<int> { 2 },
                    "A surge of strength"
                ));
                weightedActions.Add(new WeightedAction(
                    "Club Defend", 1, AddGuard(7), EnumGlobal.IntentType.Defend,
                    new List<int> { 7 },
                    "A defensive guard up"
                ));
                conditionalActions.Add(new ConditionalAction(
                    "Rage Recovery",
                    SelfBelowHpPercent(0.3f),
                    new List<Action<EnemyStat, Stats>> {
                        (e,t) => e.heal(10),
                        (e,t) => e.ApplyBuff(EnumGlobal.BuffType.Armed, 3)
                    },
                    EnumGlobal.IntentType.Heal,
                    new List<int> { 10, 1 },
                    coolDown: 5,
                    description: "Recovers health and strength in rage"
                ));
                break;

            case EnumGlobal.EnemyType.Krab:
                weightedActions.Add(new WeightedAction(
                    "Claw Pinch", 5, Attack(4), EnumGlobal.IntentType.Attack,
                    new List<int> { 4 },
                    "A quick pinch"
                ));
                weightedActions.Add(new WeightedAction(
                    "Turtle Up", 4, AddGuard(7), EnumGlobal.IntentType.Defend,
                    new List<int> { 7 },
                    "A defensive stance"
                ));
                weightedActions.Add(new WeightedAction(
                    "Shell Guard", 2, ApplyBuff(EnumGlobal.BuffType.Fortify, 2), EnumGlobal.IntentType.Buff,
                    new List<int> { 2 },
                    "Shell fortification"
                ));
                conditionalActions.Add(new ConditionalAction(
                    "Iron Shell",
                    SelfBelowHpPercent(0.5f),
                    new List<Action<EnemyStat, Stats>> {
                        (e,t) => e.Add_shield(15),
                        (e,t) => t.ApplyBuff(EnumGlobal.BuffType.Fragile, 2)
                    },
                    EnumGlobal.IntentType.Special,
                    new List<int> { 15 },
                    coolDown: 5,
                    description: "Solid shell retaliation"
                ));
                conditionalActions.Add(new ConditionalAction(
                    "Shell Slam",
                    TargetHasBuff(EnumGlobal.BuffType.Fragile),
                    new List<Action<EnemyStat, Stats>> {
                        (e,t) => e.Add_guard(5),
                        (e,t) => e.Attack(t, 3+e.shield + e.guard)
                    },
                    EnumGlobal.IntentType.AttackPrecise,
                    new List<int>(),
                    coolDown: 0,
                    description: "Smashes fragile foes with shield and guard"
                ));
                break;

            case EnumGlobal.EnemyType.Necromancer:
                weightedActions.Add(new WeightedAction(
                    "Dark Bolt", 4, Attack(6), EnumGlobal.IntentType.Attack,
                    new List<int> { 6 },
                    "A bolt of dark energy"
                ));
                weightedActions.Add(new WeightedAction(
                    "Bone Shield", 2, ApplyBuff(EnumGlobal.BuffType.Fortify, 2), EnumGlobal.IntentType.Defend,
                    new List<int> { 2 },
                    "Raises a shield of bones"
                ));
                // weightedActions.Add(new WeightedAction( //implementing this later
                //     "Raise Undead", 
                //     "Raises undead minions"
                // ));
                conditionalActions.Add(new ConditionalAction(
                    "Raise Undead",
                    SelfBelowHpPercent(0.7f),
                    new List<Action<EnemyStat, Stats>> {
                        (e,t) => e.heal(8),
                        (e,t) => e.ApplyBuff(EnumGlobal.BuffType.Fortify, 1)
                    },
                    EnumGlobal.IntentType.Buff,
                    new List<int> { 8, 1 },
                    coolDown: 6,
                    description: "Summons undead to bolster defenses"
                ));
                break;

            default:
                break;
        }
    }
}
