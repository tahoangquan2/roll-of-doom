using Godot;
using System;
using System.Collections.Generic;

[Tool]
[GlobalClass]

public partial class Stats : Resource //  base class for character Stat. (Player or Enemy)
{	

	[Export] public string name = "Character";
	[Export] public int maxHealth = 30;

	[Signal] public delegate void StatChangedEventHandler(); // for health, guard, shield change specifically
	[Export] public PackedScene CharacterVisualScene;
	public int currentHealth = 30;	
	public int guard = 0;
	public int shield = 0;

	public Dictionary<EnumGlobal.BuffType, BuffUI> buffs = new Dictionary<EnumGlobal.BuffType, BuffUI>();

	public void SetHealth(int value)	{
		currentHealth = Mathf.Clamp(value, 0, maxHealth);
		EmitSignal(nameof(StatChanged));
	}
	public void Add_guard(int value)	{
		guard = Mathf.Clamp(guard + value, 0, 999);	
		EmitSignal(nameof(StatChanged));
	}

	public void Add_shield(int value)	{
		shield = Mathf.Clamp(shield + value, 0, 999);		
		EmitSignal(nameof(StatChanged));
	}

	public int TakeDamage(int damage)	{
		if (damage <= 0) return 0;

		int remainingDamage = damage;

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

		if (remainingDamage > 0)		{
			currentHealth -= remainingDamage;
		}

		EmitSignal(nameof(StatChanged)); 

		return remainingDamage; // Return actual HP loss
	}


	public void heal(int value)
	{
		currentHealth = Mathf.Clamp(currentHealth + value, 0, maxHealth);
		EmitSignal(nameof(StatChanged));
	}

	public virtual Stats CreateInstance()
	{
		Stats newStat = Duplicate() as Stats;
		newStat.maxHealth = maxHealth;
		newStat.name = name;

		return newStat;
	}

	public virtual void Attack(Stats target, int damage)
	{
		int remainingDamage = target.TakeDamage(damage);
		if (remainingDamage > 0)
		{
			GD.Print($"{name} attacked {target.name} for {damage} damage. Remaining damage: {remainingDamage}");
		}
		else
		{
			GD.Print($"{name} attacked {target.name}, but no damage was dealt.");
		}
	}

	public BuffUI ApplyBuff(EnumGlobal.BuffType type, int value) {
		if (buffs.ContainsKey(type)) {
			return buffs[type];
		} 

		BuffUI buff = BuffDatabase.buffScene.Instantiate<BuffUI>();
		buff.SetBuff(type, value);
		buffs.Add(type, buff);
		return buff;		
	}

	public void RemoveBuff(EnumGlobal.BuffType type) {
		if (buffs.ContainsKey(type)) {
			buffs[type].QueueFree();
			buffs.Remove(type);
		}
	}

	public bool BuffExists(EnumGlobal.BuffType type) {
		return buffs.ContainsKey(type);
	}

	public virtual  void Cycle() {
		// remove guard 
		guard = 0;

		// Update buffs
		foreach (var buff in buffs) {
			var duration = buff.Value.Duration;
			if (duration == EnumGlobal.BuffDuration.Diminishing) {
				buff.Value.AddValue(-1);
			} else if (duration == EnumGlobal.BuffDuration.Volatile) {
				buff.Value.UpdateValue(0);
			} 			
			if (buff.Value.ValueX <= 0) {
				RemoveBuff(buff.Key);
			}
		}
	}



	//### **Stat**

// - **Hitpoint:** Your remaining health.
// - **Ult charge:** When full, allows you to use your ultimate ability.
// - **Mana:** Refills to full at the start of each turn.
// - **Spell Mana:** Stores unused mana from the previous turn (up to 3 max).

// - **guard :** Absorbs incoming damage. Removed at the start of your next turn.
// - **shield  :** Similar to **guard**, but does not expire. Take damage after **guard**.


// ### **Buffs & Debuffs**
// - Dodge (X) : Evades a single instance of damage then lose one value. Calculate before Guard, Diminishing
// - Bounce (X): Returns a X damage to the attacker, Volatile.
// - **Fortify (X):** Increases guard or shield gain by X.
// - **Armed (X)**: Increases attack damage by X.
// - **Vigilant (X):** Like **Fortify**, **Volatile**.
// - **Pump (X):** Like **Armed**, **Volatile**.
// - **Exhaust (X):** Reduces damage dealt 30%, **Diminishing**.
// - **Fragile (X):** Increases damage taken 30%, **Diminishing**.
// - **Poisoned (X):** Deals X damage at the start of each turn, **Diminishing**.

// **Diminishing**: Drop 1 value at start of their turn.

// **Volatile**: Dissappear at start of their turn.
}
