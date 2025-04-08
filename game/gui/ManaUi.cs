using Godot;
using System;
using System.Collections.Generic;

public partial class ManaUi : HBoxContainer
{
	PlayerStat playerStat;
	private const int sizeOfContainer = 32;
	private int mana = 0; // can be unlimited
	private List<TextureRect> manaIcons = new List<TextureRect>();
	private int spellMana = 0;	// cap is to maxSpellMana
	private List<TextureRect> spellManaIcons = new List<TextureRect>();
	private CardManager cardManager;
	private Tween mainTween = null;

	private Label manaLabel => GetNode<Label>("BasicMana/ManaLabel");
	private Label spellManaLabel => GetNode<Label>("SpellMana/ManaLabel");
	private Label notEnoughManaLabel => GetNode<Label>("BasicMana/NotMana");
	private VBoxContainer ManaContainer => GetNode<VBoxContainer>("BasicMana/VContainer"); // this contains 8 mana icons
	private VBoxContainer SpellContainer => GetNode<VBoxContainer>("SpellMana/VContainer");// this contains 4 spell mana icons

	private Texture2D manaIcon => ResourceLoader.Load<Texture2D>("res://assets/cards/mana_slot.png");
	private Texture2D spellManaIconTexture => ResourceLoader.Load<Texture2D>("res://assets/cards/mana_spell.png");

	public override void _Ready()
	{
		// get group from the of this node
		cardManager = GetTree().CurrentScene.GetNodeOrNull<CardManager>(GlobalAccessPoint.cardManagerPath);
		if (cardManager != null) {
			cardManager.CardFocus += HoverCard;
		}

		// get all the mana icons
		foreach (TextureRect manaIcon in ManaContainer.GetChildren()) 
		{	manaIcon.PivotOffset = new Vector2(sizeOfContainer / 2, sizeOfContainer / 2);
			manaIcons.Add(manaIcon); } manaIcons.Reverse();

		foreach (TextureRect manaIcon in SpellContainer.GetChildren())
		{	manaIcon.PivotOffset = new Vector2(sizeOfContainer / 2, sizeOfContainer / 2);
			spellManaIcons.Add(manaIcon);	}  spellManaIcons.Reverse();

		playerStat = GlobalVariables.playerStat;
		SetAllMana(playerStat.baseMana, playerStat.spellMana);		

		playerStat.ManaChanged += () => {
			SetAllMana(playerStat.mana, playerStat.spellMana);
		};			
	}
	public void setMana(int value) // max display is 8, 
	// manaIcons has 8 mana icons, appear and disappear according ly
	{
		manaLabel.Text = value.ToString();
		
		List<TextureRect> toAppear = new();
		List<TextureRect> toDisappear = new();

		for (int i = 0; i < manaIcons.Count; i++)
		{
			var icon = manaIcons[i];

			if (i < value)
			{
				if (!icon.Visible)
				{
					icon.Texture = manaIcon;
					toAppear.Add(icon);
				}
			}
			else if (icon.Visible)
			{
				toDisappear.Add(icon);
			}
		}		

		AnimateManaAppear(toAppear);
		AnimateManaDisappear(toDisappear);

		mana = value;
	}

	public void setSpellMana(int value){
		value = Mathf.Clamp(value, 0, playerStat.capSpellMana);
		spellManaLabel.Text = value.ToString();

		int visualValue = Mathf.Clamp(value, 0, playerStat.capSpellMana);

		List<TextureRect> toAppear = new();
		List<TextureRect> toDisappear = new();

		for (int i = 0; i < spellManaIcons.Count; i++)
		{
			var icon = spellManaIcons[i];

			if (i < visualValue)
			{
				if (!icon.Visible)
				{
					icon.Texture = spellManaIconTexture;
					toAppear.Add(icon);
				}
			}
			else if (icon.Visible)
			{
				toDisappear.Add(icon);
			}
		}

		AnimateManaAppear(toAppear);
		AnimateManaDisappear(toDisappear);

		spellMana = value;
	}


	private List<TextureRect> GetVisibleManaIcons(int count,bool isSpell=false)
	{
		List<TextureRect> icons = new();
		List<TextureRect> container = isSpell ? spellManaIcons : manaIcons;

		count = Math.Clamp(count, 0, container.Count);

		// get count of visible icons bottom to top
		for (int i = container.Count - 1; i>=0 && count!=0; i--)
		{
			if (container[i].Visible) {
				icons.Add(container[i]);
				count--;
			}
		}

		return icons;
	}

	private async void AnimateManaAppear(List<TextureRect> icons)
	{	
		if (icons.Count == 0) return;
		GD.Print("AnimateManaAppear " + icons.Count);
		foreach (var icon in icons){
			icon.Scale = Vector2.Zero;
			icon.Visible = true;			
		}

		mainTween ??= CreateTween();
		mainTween.SetParallel();

		foreach (var icon in icons)	{
			mainTween.TweenProperty(icon, "scale", Vector2.One, 5.55f)
				.SetTrans(Tween.TransitionType.Bounce)
				.SetEase(Tween.EaseType.Out);
		}
	}

	private void AnimateManaDisappear(List<TextureRect> icons)
	{
		if (icons.Count == 0) return;		
		GD.Print("AnimateManaDisappear " + icons.Count);
		mainTween ??= CreateTween();
		mainTween.SetParallel();

		foreach (var icon in icons)		{
			mainTween.TweenProperty(icon, "scale", Vector2.Zero, 0.55f)
				.SetTrans(Tween.TransitionType.Back)
				.SetEase(Tween.EaseType.In);
		}

		mainTween.Chain().TweenCallback(Callable.From(() =>
		{
			foreach (var icon in icons)
				icon.Visible = false;
		}));
	}



	private void HoverMana(int cost) { // if cost <= mana, create tween to continuously blink
		if (cost <= 0) return;
		if (cost <= mana) {
			List<TextureRect> manaIcons;

			// Get the visible mana icons
			manaIcons = GetVisibleManaIcons(cost);

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
		iconsToBlink.AddRange(GetVisibleManaIcons(spellToUse, true));

		int remainingCost = cost - spellToUse;
		iconsToBlink.AddRange(GetVisibleManaIcons(remainingCost));		

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

		SetAllMana(playerStat.baseMana, spilledMana);
	}

	public void SetAllMana(int mana, int spellMana) {
		CleanTween();
		setSpellMana(spellMana);
		setMana(mana);		
	}

	private void CleanTween() {
		if (mainTween != null) {
			mainTween.Kill();
			mainTween = null;
		}
	}
}
