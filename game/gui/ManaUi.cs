using Godot;
using System;
using System.Collections.Generic;

public partial class ManaUi : HBoxContainer
{
	private const int sizeOfContainer = 32;
	private int mana = 0;
	private int spellMana = 0;	
	private CardManager cardManager;
	private Tween mainTween = null;

	private Label manaLabel => GetNode<Label>("BasicMana/ManaLabel");
	private Label spellManaLabel => GetNode<Label>("SpellMana/ManaLabel");
	private Label notEnoughManaLabel => GetNode<Label>("BasicMana/NotMana");

	private VBoxContainer ManaContainer => GetNode<VBoxContainer>("BasicMana/VContainer");
	private VBoxContainer SpellContainer => GetNode<VBoxContainer>("SpellMana/VContainer");

	private Texture2D manaIcon => ResourceLoader.Load<Texture2D>("res://assets/cards/mana_slot.png");
	private Texture2D spellManaIconTexture => ResourceLoader.Load<Texture2D>("res://assets/cards/mana_spell.png");

	public override void _Ready()
	{
		setMana(6);
		setSpellMana(2);
		cardManager = GetTree().CurrentScene.GetNodeOrNull<CardManager>(GlobalAccessPoint.cardManagerPath);
		if (cardManager != null) {
			cardManager.CardFocus += hoverCard;
		}

	}
	public void setMana(int value){		
		mana = value;
		manaLabel.Text = value.ToString();
		int childrenCount = ManaContainer.GetChildCount();

		if (childrenCount < value)		{
			value = Mathf.Clamp(value, 0, 8);
			for (int i = childrenCount; i < value; i++)		{
				TextureRect newManaIcon = createManaIcon(manaIcon);
				ManaContainer.AddChild(newManaIcon);
			}
		} else if (childrenCount > value) {
			for (int i = childrenCount - 1; i >= value; i--)
				ManaContainer.RemoveChild(ManaContainer.GetChild(i));
		}

	}

	public void setSpellMana(int value){
		spellMana = value;
		spellManaLabel.Text = value.ToString();
		int childrenCount = SpellContainer.GetChildCount();

		if (childrenCount < value)		{
			value = Mathf.Clamp(value, 0, 3);
			for (int i = childrenCount; i < value; i++)		{
				TextureRect newManaIcon = createManaIcon(spellManaIconTexture);
				SpellContainer.AddChild(newManaIcon);
			}
		} else if (childrenCount > value) {
			for (int i = childrenCount - 1; i >= value; i--)			{
				SpellContainer.RemoveChild(SpellContainer.GetChild(i));
			}
		}
	}

	private TextureRect createManaIcon(Texture2D manaIcon)	{
		TextureRect newManaIcon = new TextureRect();
		newManaIcon.Texture = manaIcon;
		newManaIcon.SetSize(new Vector2(sizeOfContainer, sizeOfContainer));
		newManaIcon.MouseDefaultCursorShape = CursorShape.Help;
		newManaIcon.TextureFilter = TextureFilterEnum.Linear;
		return newManaIcon;
	}

	private void hoverMana(int cost) { // if cost <= mana, create tween to continuously blink
		if (cost <= 0) return;
		if (cost <= mana) {
			List<TextureRect> manaIcons = new List<TextureRect>();

			for (int  i = 0; i < cost; i++) {
				TextureRect manaIcon = ManaContainer.GetChild(i) as TextureRect;
				manaIcons.Add(manaIcon);		
			}
			// Blink the icons
			BlinkIcons(manaIcons);
		} else {
			notEnoughManaLabel.Show();
		}		
	}

	private void hoverSpellMana(int cost)
	{
		if (cost <= 0) return;

		int totalAvailable = spellMana + mana;

		if (cost > totalAvailable)
		{
			notEnoughManaLabel.Show();
			return;
		}

		List<TextureRect> iconsToBlink = new();

		// First: collect spell mana icons to blink
		int spellToUse = Math.Min(cost, spellMana);
		for (int i = 0; i < spellToUse; i++)		{
			if (SpellContainer.GetChild(i) is TextureRect spellIcon)
				iconsToBlink.Add(spellIcon);
		}

		// Then: collect regular mana icons for the overflow
		int remainingCost = cost - spellToUse;
		for (int i = 0; i < remainingCost; i++)		{
			if (ManaContainer.GetChild(i) is TextureRect manaIcon)
				iconsToBlink.Add(manaIcon);
		}

		// Blink the icons
		BlinkIcons(iconsToBlink);
	}

	private void BlinkIcons(List<TextureRect> iconsToBlink)
	{
		if (mainTween != null)		{
			mainTween.Kill();
			mainTween = null;
		}
		mainTween = CreateTween().SetLoops();
		mainTween.SetParallel();
		foreach (var icon in iconsToBlink)		{
			mainTween.TweenProperty(icon, "modulate", new Color(1, 1, 1, 0.5f), 0.5f)
				.SetTrans(Tween.TransitionType.Sine)
				.SetEase(Tween.EaseType.InOut);
		}

		mainTween.Chain();
		foreach (var icon in iconsToBlink)		{
			mainTween.TweenProperty(icon, "modulate", new Color(1, 1, 1, 1.0f), 0.5f)
				.SetTrans(Tween.TransitionType.Sine)
				.SetEase(Tween.EaseType.InOut);
		}
	}


	public void unhoverCard(){
		if (mainTween != null) {
			mainTween.Kill();
			mainTween = null;
		}
		notEnoughManaLabel.Hide();
		for (int i = 0; i < ManaContainer.GetChildCount(); i++) {
			Control manaIcon = ManaContainer.GetChild(i) as Control;
			manaIcon.Modulate = new Color(1, 1, 1, 1.0f);
		}
		for (int i = 0; i < SpellContainer.GetChildCount(); i++) {
			Control manaIcon = SpellContainer.GetChild(i) as Control;
			manaIcon.Modulate = new Color(1, 1, 1, 1.0f);
		}		
	}

	public void hoverCard(CardData card,bool isHovered)	{ // spell cost both mana and spell mana, priority to spell mana
		
		if (!isHovered) {
			unhoverCard();
			return;
		}
		if (card.CardType==EnumGlobal.enumCardType.Spell) {
			hoverSpellMana(card.Cost);
		} else{
			hoverMana(card.Cost);
		}
	}
}
