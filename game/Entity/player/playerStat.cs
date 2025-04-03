using System;
using Godot;
using Godot.Collections;

public partial class playerStat : stats
{
	[Export] public int maxMana = 4;
	[Export] public int maxSpellMana = 2;
	[Export] public Array<CardData> startingDeck = new Array<CardData>();
	[Export] public int cardDrawperTurn = 4;
	[Export] public int startingGold = 10;

	public int currentMana = 0;
	public int currentSpellMana = 0;

	public int currentGold = 0;

	public Array<CardData> currentDeck = new Array<CardData>();
	
	public bool can_play_card(CardData card)
	{
		if (card.CardType == EnumGlobal.enumCardType.Spell){
			if (card.Cost > currentMana+currentSpellMana) 
				return false;
		} else	
		if (card.Cost > currentMana) 
			return false;
		return true;
	}

	public void spendMana(int cost)
	{
		if (cost > currentMana) return;
		currentMana -= cost;
	}

	public void spendSpellMana(int cost)
	{
		if (cost > currentSpellMana+currentMana) return;
		int remainingCost = cost;
		
		int spentSpell = Math.Min(currentSpellMana, remainingCost);
		currentSpellMana -= spentSpell;
		remainingCost -= spentSpell;

		currentMana -= remainingCost;		
	}

	public void addCard(CardData card)	{
		currentDeck.Add(card);
	}

	public void Cycle(){
		refillMana();
	}

	public void refillMana()
	{
		currentMana = Mathf.Clamp(currentMana + currentSpellMana, 0, maxMana);
		currentMana = maxMana;
	}

	//- **Mana:** Refills to full at the start of each turn.
	//- **Spell Mana:** Stores unused mana from the previous turn (up to 2 max).
}
