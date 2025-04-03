using Godot;
using System;

public partial class stats : Resource //  base class for character Stat. (Player or Enemy)
{	

	[Export] public string name = "Character";
	[Export] public int maxHealth = 30;
	public int currentHealth = 30;	

	public int guard = 0;
	public int shield = 0;

	public void setHealth(int value)
	{
		currentHealth = Mathf.Clamp(value, 0, maxHealth);
	}
	public void add_guard(int value)
	{
		guard = Mathf.Clamp(guard + value, 0, 999);	
	}

	public void add_shield(int value)
	{
		shield = Mathf.Clamp(shield + value, 0, 999);		
	}
	public Resource getCopy()
	{
		return Duplicate();
	}

	public int takeDamage(int damage)
	{
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

		return remainingDamage; // Return actual HP loss
	}


	public void heal(int value)
	{
		currentHealth = Mathf.Clamp(currentHealth + value, 0, maxHealth);
	}

	//### **Stat**

// - **Hitpoint:** Your remaining health.
// - **Ult charge:** When full, allows you to use your ultimate ability.
// - **Mana:** Refills to full at the start of each turn.
// - **Spell Mana:** Stores unused mana from the previous turn (up to 3 max).
// - **Stance Points:** Resets each **cycle**. When depleted, take damage.
// - **Dodge :** Evades a single instance of damage. Take damage before **guard**.
// - **guard :** Absorbs incoming damage. Removed at the start of your next turn.
// - **shield  :** Similar to **guard**, but does not expire. Take damage after **guard**.
// - **Bounce:** Returns a portion of damage to the attacker.

// ### **Buffs & Debuffs**

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
