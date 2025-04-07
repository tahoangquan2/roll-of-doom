using Godot;
using System;
using System.Collections.Generic;

public partial class ManaUi : HBoxContainer
{
	PlayerStat playerStat;
	private const int sizeOfContainer = 32;
	private int mana = 0; // can be unlimited
	private int spellMana = 0;	// cap is to maxSpellMana
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
		// get group from the of this node
		cardManager = GetTree().CurrentScene.GetNodeOrNull<CardManager>(GlobalAccessPoint.cardManagerPath);
		if (cardManager != null) {
			cardManager.CardFocus += HoverCard;
		}

		playerStat = GlobalVariables.playerStat;
		SetAllMana(playerStat.baseMana, playerStat.spellMana);

		playerStat.ManaChanged += () => {
			SetAllMana(playerStat.mana, playerStat.spellMana);
		};			
	}
	public void setMana(int value){		
		mana = value;
		manaLabel.Text = value.ToString();
		int childrenCount = ManaContainer.GetChildCount();

		if (childrenCount < value)		{
			value = Mathf.Clamp(value, 0, 8);
			CreateManaIcons(value-childrenCount,manaIcon,ManaContainer);
		} else if (childrenCount > value) {
			int cnt = childrenCount - value;
			for (int i = 0; i < cnt; i++) {			{
				Control manaIcon = ManaContainer.GetChild(i) as Control;
				if (manaIcon != null) {
					AnimateManaDisappear(manaIcon);
				}
			}
		}
		}

	}

	public void setSpellMana(int value){
		value = Mathf.Clamp(value, 0, playerStat.capSpellMana);
		spellMana = value;
		spellManaLabel.Text = value.ToString();
		int childrenCount = SpellContainer.GetChildCount();

		if (childrenCount < value)		{
			CreateManaIcons(value-childrenCount,spellManaIconTexture,SpellContainer);
		} else if (childrenCount > value) {
			int cnt = childrenCount - value;
			for (int i = 0; i < cnt; i++) {
				Control manaIcon = SpellContainer.GetChild(i) as Control;
				if (manaIcon != null) {
					AnimateManaDisappear(manaIcon);
				}
			}
		}
	}

	private void CreateManaIcons(int cnt,Texture2D manaIcon,Control parentContainer) {			
		for (int i = 0; i < cnt; i++) {			
			TextureRect newManaIcon = new TextureRect
			{
				Texture = manaIcon,
				Size = new Vector2(sizeOfContainer, sizeOfContainer),
				MouseDefaultCursorShape = CursorShape.Help,
				TextureFilter = TextureFilterEnum.Linear,
				Scale = new Vector2(0f, 0f) ,
				PivotOffset = new Vector2(sizeOfContainer / 2, sizeOfContainer / 2),
			};
			parentContainer.AddChild(newManaIcon);
			AnimateManaAppear(newManaIcon);
		}
	}

	private void AnimateManaAppear(Control icon)
	{
		icon.Scale = Vector2.Zero;
		icon.Modulate = new Color(1, 1, 1, 1);

		mainTween ??= CreateTween();
		mainTween.SetParallel();

		mainTween.TweenProperty(icon, "scale", Vector2.One, 0.4f)
			.SetTrans(Tween.TransitionType.Bounce)
			.SetEase(Tween.EaseType.Out);
	}

	private void AnimateManaDisappear(Control manaIcon)
	{
		mainTween ??= CreateTween();
		mainTween.SetParallel();
		mainTween.TweenProperty(manaIcon, "scale", new Vector2(0f, 0f), 0.3f)
			.SetTrans(Tween.TransitionType.Back)
			.SetEase(Tween.EaseType.In);

		mainTween.TweenProperty(manaIcon, "modulate", new Color(1, 1, 1, 0), 0.3f)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.Out);

		mainTween.Finished += () => {manaIcon.QueueFree();};
	}

	private void HoverMana(int cost) { // if cost <= mana, create tween to continuously blink
		if (cost <= 0) return;
		if (cost <= mana) {
			List<TextureRect> manaIcons = new List<TextureRect>();

			for (int  i = 0; i < cost; i++) {
				TextureRect manaIcon = ManaContainer.GetChild(i) as TextureRect;
				manaIcons.Add(manaIcon);		
			}
			// Blink the icons
			BlinkIcons(manaIcons);
			notEnoughManaLabel.Hide();
		} else {
			notEnoughManaLabel.Show();
			UnhoverAllMana();
		}		
	}

	private void HoverSpellMana(int cost)	{
		if (cost <= 0) return;

		int totalAvailable = spellMana + mana;

		if (cost > totalAvailable)
		{
			notEnoughManaLabel.Show();
			UnhoverAllMana();
			return;
		}
		notEnoughManaLabel.Hide();

		List<TextureRect> iconsToBlink = new();

		int spellToUse = Math.Min(cost, spellMana);
		for (int i = 0; i < spellToUse; i++)		{
			if (SpellContainer.GetChild(i) is TextureRect spellIcon)
				iconsToBlink.Add(spellIcon);
		}

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
		CleanTween();
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


	public void UnhoverCard(){
		notEnoughManaLabel.Hide();		
		UnhoverAllMana();	
	}

	private void UnhoverAllMana() {
		CleanTween();
		for (int i = 0; i < ManaContainer.GetChildCount(); i++) {
			Control manaIcon = ManaContainer.GetChild(i) as Control;
			manaIcon.Modulate = new Color(1, 1, 1, 1.0f);
		}
		for (int i = 0; i < SpellContainer.GetChildCount(); i++) {
			Control manaIcon = SpellContainer.GetChild(i) as Control;
			manaIcon.Modulate = new Color(1, 1, 1, 1.0f);
		}
	}

	public void HoverCard(CardData card,bool isHovered)	{ // spell cost both mana and spell mana, priority to spell mana		
		if (!isHovered) {
			UnhoverCard();
			return;
		}
		if (card.CardType==EnumGlobal.enumCardType.Spell) {
			HoverSpellMana(card.Cost);
		} else{
			HoverMana(card.Cost);
		}
	}

	public async void Cycle(){ //
		UnhoverCard();
		int spilledMana = Math.Clamp(mana, 0, playerStat.capSpellMana);
		SetAllMana(0,0);		

		if (mainTween!=null) await ToSignal(mainTween, "finished");

		await ToSignal(GetTree(), "process_frame");

		GD.Print(ManaContainer.GetChildCount());	

		SetAllMana(playerStat.baseMana, spilledMana);
	}

	public void SetAllMana(int mana, int spellMana) {
		CleanTween();
		setMana(mana);
		setSpellMana(spellMana);
	}

	private void CleanTween() {
		if (mainTween != null) {
			mainTween.Kill();
			mainTween = null;
		}
	}
}
