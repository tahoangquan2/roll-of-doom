using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
	private AudioStreamPlayer2D manaSound => GetNode<AudioStreamPlayer2D>("ManaSound");
	private Label manaLabel => GetNode<Label>("BasicMana/ManaLabel");
	private Label spellManaLabel => GetNode<Label>("SpellMana/ManaLabel");
	private Label notEnoughManaLabel => GetNode<Label>("BasicMana/NotMana");
	private VBoxContainer ManaContainer => GetNode<VBoxContainer>("BasicMana/VContainer"); // this contains 8 mana icons
	private VBoxContainer SpellContainer => GetNode<VBoxContainer>("SpellMana/VContainer");// this contains 4 spell mana icons

	private Texture2D manaIcon => ResourceLoader.Load<Texture2D>("res://assets/cards/mana_slot.png");
	private Texture2D spellManaIconTexture => ResourceLoader.Load<Texture2D>("res://assets/cards/mana_spell.png");

	public void GameStart()
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
	public async Task setMana(int value){ // max display is 8, manaIcons has 8 mana icons, appear and disappear accordingly
		await UpdateManaIcons(value, manaIcons, manaIcon, manaLabel, false);		
	}

	public async Task setSpellMana(int value){
		await UpdateManaIcons(value, spellManaIcons, spellManaIconTexture, spellManaLabel, true);
	}

	private async Task UpdateManaIcons(int value,List<TextureRect> iconList,Texture2D iconTexture,Label label,bool isSpell)
	{
		label.Text = value.ToString();
		int clampedValue = Mathf.Clamp(value, 0, iconList.Count);

		List<TextureRect> toAppear = new();
		List<TextureRect> toDisappear = new();

		for (int i = 0; i < iconList.Count; i++)
		{
			var icon = iconList[i];

			if (i < clampedValue){
				if (!icon.Visible){
					icon.Texture = iconTexture;
					toAppear.Add(icon);
				}
			}
			else if (icon.Visible){
				toDisappear.Add(icon);
			}
		}

		await AnimateManaAppear(toAppear);
		await AnimateManaDisappear(toDisappear);

		if (isSpell) spellMana = value;
		else mana = value;
	}

	private List<TextureRect> GetVisibleManaIcons(int count,bool isSpell=false)
	{
		List<TextureRect> icons = new();
		List<TextureRect> container = isSpell ? spellManaIcons : manaIcons;

		count = Math.Clamp(count, 0, container.Count);

		// get count of visible icons bottom to top
		for (int i = container.Count - 1; i>=0 && count!=0; i--){
			if (container[i].Visible) {
				icons.Add(container[i]);
				count--;
			}
		}

		return icons;
	}

	private async Task AnimateManaDisappear(List<TextureRect> icons)
	{
		if (icons.Count == 0) return;

		ManaSoundPlay(false);

		var disappearTween = CreateTween();
		disappearTween.SetParallel();

		foreach (var icon in icons)
		{
			disappearTween.TweenProperty(icon, "scale", Vector2.Zero, 0.55f)
				.SetTrans(Tween.TransitionType.Back)
				.SetEase(Tween.EaseType.In);
		}

		disappearTween.Chain().TweenCallback(Callable.From(() =>
		{
			foreach (var icon in icons)
				icon.Visible = false;
		}));		

		await ToSignal(disappearTween, "finished");
		await ToSignal(GetTree().CreateTimer(1f), "timeout");
	}
	private async Task AnimateManaAppear(List<TextureRect> icons)
	{
		if (icons.Count == 0) return;

		foreach (var icon in icons)	{
			icon.Visible = true;
			icon.Scale = Vector2.Zero;			
		}
		await ToSignal(GetTree(), "process_frame");

		ManaSoundPlay(true);
		var appearTween = CreateTween();
		appearTween.SetParallel();		

		foreach (var icon in icons)	{
			icon.Scale = Vector2.Zero;
			appearTween.TweenProperty(icon, "scale", Vector2.One, 0.55f)
				.SetTrans(Tween.TransitionType.Bounce)
				.SetEase(Tween.EaseType.Out);
		}

		await ToSignal(appearTween, "finished");
		await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
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

	public async Task Cycle(){ //
		UnhoverCard();
		int spilledMana = Math.Clamp(mana, 0, playerStat.capSpellMana);

		// First, disable all current mana
		List<TextureRect> allMana = new(manaIcons);
		allMana.AddRange(spellManaIcons);
		
		manaLabel.Text = "0"; spellManaLabel.Text = "0";
		await AnimateManaDisappear(allMana);

		await ToSignal(GetTree(), "process_frame");

		await SetAllMana(playerStat.baseMana, spilledMana);
		//GD.Print("ManaUi Cycle end: "+playerStat.baseMana+" "+spilledMana);
	}

	public async Task SetAllMana(int mana, int spellMana) {
		CleanTween();
		await setSpellMana(spellMana);
		await setMana(mana);		
	}

	private async void ManaSoundPlay(bool manaAppear) {
		manaSound.Stop();
		if (manaAppear) {
			manaSound.Seek(0);
			manaSound.Play();
			await ToSignal(GetTree().CreateTimer(2.4f), "timeout");
			manaSound.Stop();
		} else {
			manaSound.Seek(2);
			manaSound.Play();
		}
	}

	private void CleanTween() {
		if (mainTween != null) {
			mainTween.Kill();
			mainTween = null;
		}
	}
}
