using System;
using Godot;
using Godot.Collections;

[Tool]
[GlobalClass]
// This class is used to store the player's stats and manage the player's deck and mana.
// often 2 type of player stats are used:
// 1. Starting stats: used to create the player at the start of every new run. 
// Create in the editor.

// 2. Running stats: store the stats of the player during the run, reset every new run. 
// Create from Starting stats every new run. 
// every run, the player will have a new instance of PlayerStat
public partial class PlayerStat : Stats
{	
	[Export] public int capSpellMana = 2; // max spell mana that can be stored
	[Export] public int baseMana = 4; // mana that naturally refills each turn
	[Export] public Array<CardData> startingDeck = new Array<CardData>(); 
	// 1. starting deck at the start of the run
	// 2. deck of the player before combat
	[Export] public int cardDrawPerTurn = 4; // number of cards drawn each turn
	[Export] public int gold = 10; // gold from the start of the run
	[Signal] public delegate void ManaChangedEventHandler();

	private bool hasInfiniteMana = false; // if true, mana will not be consumed

	public int mana = 0; // current mana
	public int spellMana = 0; // current spell mana

	public void AddCard(CardData card)	{
		startingDeck.Add(card);
	}

	public bool RequestPlayCard(CardData card)	{
		if (hasInfiniteMana) return true;
		if (card.Keywords.Contains(EnumGlobal.CardKeywords.Unplayable)) return false;
		bool result = true;
		int cost = card.Cost;
		if (card.CardType == EnumGlobal.enumCardType.Spell){
			if (cost > mana+spellMana) 
				result = false;
			else {
				int spellToUse = Math.Min(cost, spellMana);
				spellMana -= spellToUse;	
				cost -= spellToUse;
				mana -= cost;
				EmitSignal(nameof(ManaChanged));
			}			
		} else	
		if (card.Cost > mana) result = false;	
		else {
			mana -= card.Cost;	
			EmitSignal(nameof(ManaChanged));
		}		
			
		return result;
	}

	public void GainMana(int amount)	{
		mana += amount;
		EmitSignal(nameof(ManaChanged));
	}
	public void GainSpellMana(int amount)	{
		spellMana += amount;
		EmitSignal(nameof(ManaChanged));
	}


	public override PlayerStat CreateInstance()
	{
		PlayerStat newStat = Duplicate() as PlayerStat;
		newStat.maxHealth = maxHealth;
		newStat.name = name;
		newStat.currentHealth = currentHealth != -1 ? currentHealth : maxHealth;
		newStat.cardDrawPerTurn = cardDrawPerTurn;
		newStat.baseMana = baseMana;
		newStat.capSpellMana = capSpellMana;

		newStat.mana = baseMana;
		newStat.spellMana = 0;

		newStat.startingDeck = new Array<CardData>();
		foreach (CardData card in startingDeck)	if (card != null) {
			CardData newCard = card.Duplicate() as CardData;
			newStat.startingDeck.Add(newCard);
		}				

		return newStat;
	}

	public override void Cycle()
	{
		base.Cycle();		

		spellMana = Mathf.Clamp(mana, 0, capSpellMana);
		mana = baseMana;
	}
}
