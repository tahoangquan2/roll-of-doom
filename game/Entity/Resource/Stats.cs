using Godot;
using System;
using System.Collections.Generic;

[Tool]
[GlobalClass]

public partial class Stats : Resource //  base class for character Stat. (Player or Enemy)
{	
	[Export] public string name = "Character";
	[Export] public int maxHealth = 30;
	[Export] public PackedScene CharacterVisualScene;
	[Signal] public delegate void StatChangedEventHandler(); // for health, guard, shield change specifically
	[Signal] public delegate void BuffChangedEventHandler(BuffUI buffUI,bool alreadyExists); // for buff change specifically
	[Signal] public delegate void AttackAniEventHandler(Stats target); // for attack specifically
	public int currentHealth = 30, guard=0, shield = 0;
	enum ActionType	{
		Attack, Defend, TakeDamage, // pass number
		Apply, Remove, Cycle, Death // pass nothing
	}
	public Dictionary<EnumGlobal.BuffType, BuffUI> buffs = new Dictionary<EnumGlobal.BuffType, BuffUI>();
	public void SetHealth(int value)	{
		currentHealth = Mathf.Clamp(value, 0, maxHealth);
		EmitSignal(nameof(StatChanged));
	}
	public void Add_guard(int value)	{
		CheckForBuff(ActionType.Defend, ref value);
		guard = Mathf.Clamp(guard + value, 0, 999);	
		EmitSignal(nameof(StatChanged));
	}
	public void Add_shield(int value)	{
		CheckForBuff(ActionType.Defend, ref value);
		shield = Mathf.Clamp(shield + value, 0, 999);		
		EmitSignal(nameof(StatChanged));
	}
	public virtual int TakeDamage(int damage,bool IsPrecise=false)	{
		if (damage <= 0) return 0;
		int tmp = damage;
		CheckForBuff(ActionType.TakeDamage, ref damage);

		int remainingDamage = IsPrecise ? tmp : damage;

		if (guard > 0)		{
			int absorbedByguard = Math.Min(guard, remainingDamage);
			guard -= absorbedByguard;
			remainingDamage -= absorbedByguard;
		}

		if (shield > 0 && remainingDamage > 0)		{
			int absorbedByshield = Math.Min(shield, remainingDamage);
			shield -= absorbedByshield;
			remainingDamage -= absorbedByshield;
		}

		if (remainingDamage > 0) currentHealth -= remainingDamage;

		EmitSignal(nameof(StatChanged)); 
		if (currentHealth <= 0) Die();

		return remainingDamage; // Return actual HP loss
	}
	public virtual void heal(int value) // negative value to damage by pass defensive buffs
	{
		currentHealth = Mathf.Clamp(currentHealth + value, 0, maxHealth);		
		EmitSignal(nameof(StatChanged));
		if (currentHealth <= 0) Die();
	}
	public virtual Stats CreateInstance()
	{
		Stats newStat = Duplicate() as Stats;
		newStat.maxHealth = maxHealth;
		newStat.name = name;
		newStat.currentHealth = currentHealth;

		return newStat;
	}

	public virtual void Attack(Stats target, int damage,bool IsPrecise=false)
	{
		if (target == null) return;
		EmitSignal(nameof(AttackAni), target);

		CheckForBuff(ActionType.Attack, ref damage);
		
		int remainingDamage = target.TakeDamage(damage,IsPrecise);
	}
	public void AttackRandom(int damage,bool IsPrecise=false)
	{	var possibleTargets = GlobalVariables.allStats.FindAll(s => s != this);
		if (possibleTargets.Count == 0) return;

		int index = GlobalVariables.GetRandomNumber(0, possibleTargets.Count - 1);
		Stats target = possibleTargets[index];

		Attack(target,damage,IsPrecise);
	}
	public void AttackAll(int damage,bool IsPrecise=false){ 
		var possibleTargets = GlobalVariables.allStats.FindAll(s => s != this);
		foreach (Stats target in possibleTargets) Attack(target, damage,IsPrecise);
	}

	private void CheckForBuff(ActionType actionType, ref int number)
	{
		var keysToRemove = new List<EnumGlobal.BuffType>();

		foreach (var buffEntry in buffs)
		{
			var buff = buffEntry.Value;
			var logic = buff._buffLogic;
			int value = buff.ValueX;

			switch (actionType)
			{
				case ActionType.TakeDamage:
					logic?.OnTakeDamage(this, ref number, ref value);
					break;
				case ActionType.Attack:
					logic?.OnAttack(this, ref number, ref value); 
					break;
				case ActionType.Defend:
					logic?.OnDefend(this, ref number, ref value);
					break;
				case ActionType.Cycle:
					logic?.OnCycle(this, ref value);
					break;
				case ActionType.Apply:
					logic?.OnApply(this, ref value);
					break;
				case ActionType.Remove:
					logic?.OnRemove(this, ref value);
					break;
				case ActionType.Death:
					logic?.OnDeath(this, ref value);
					break;

				default:
					break;
			}

			buff.UpdateValue(value);

			if (value <= 0 && actionType!= ActionType.Remove)
				keysToRemove.Add(buffEntry.Key);
		}

		foreach (var key in keysToRemove)
		{
			RemoveBuff(key);
		}
	}
	public void ApplyBuff(EnumGlobal.BuffType type, int value) {		
		if (buffs.ContainsKey(type)) {			
			buffs[type].AddValue(value);
			CheckForBuff(ActionType.Apply, ref NAN);
			EmitSignal(nameof(BuffChanged), buffs[type],true);
			return;
		} 

		BuffUI buff = BuffDatabase.buffScene.Instantiate<BuffUI>();
		buff.SetBuff(type, value);
		buffs.Add(type, buff);

		CheckForBuff(ActionType.Apply, ref NAN);
		buff.UpdateValue(value);

		EmitSignal(nameof(BuffChanged), buff,false);
	}
	public void RemoveBuff(EnumGlobal.BuffType type) {
		if (buffs.ContainsKey(type)) {			
			CheckForBuff(ActionType.Remove,ref NAN);
			buffs[type].QueueFree();
			buffs.Remove(type);
		}
	}
	public void Die() {
		CheckForBuff(ActionType.Death, ref NAN);
		GlobalVariables.allStats.Remove(this);
		GlobalVariables.allCharacterStats.Remove(this); 
		if (GlobalVariables.playerStat == this) {
			GlobalVariables.playerStat = null;
			GlobalAccessPoint.GetPlayer().LoseGame();
		} else{
			if (GlobalVariables.playerStat != null) {
				if (GlobalVariables.allStats.Count == 1) {
					GlobalAccessPoint.GetPlayer().WinGame();
				} 
			}
		}	
	}
	public virtual  void Cycle() {//		GD.Print($"{name} Cycle");
		guard = 0;
		// Apply Buffs Effect on cycle
		CheckForBuff(ActionType.Cycle, ref NAN);
		
		foreach (var buff in buffs) {			
			var duration = buff.Value.Duration;
			if (duration == EnumGlobal.BuffDuration.Diminishing) {
				buff.Value.AddValue(-1);
			} else if (duration == EnumGlobal.BuffDuration.Volatile) {
				buff.Value.UpdateValue(0);
			} 			
			if (buff.Value.ValueX <= 0) RemoveBuff(buff.Key);
		}

		EmitSignal(nameof(StatChanged));
	}
	private int NAN = 0;
	public int GetBuffValue(EnumGlobal.BuffType type) {
		if (buffs.ContainsKey(type)) return buffs[type].GetValue();
		return 0;
	}
}
