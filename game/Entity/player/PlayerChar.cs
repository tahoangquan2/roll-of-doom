using Godot;

public partial class PlayerChar : CardPlayZone
{
	[Export] PlayerStat baseStat;
	PlayerStat playerStat;
	private TextureRect HealthBar,GuardBar,ShieldBar,progressBackground;

	private Label healthLabel,guardLabel,shieldLabel;
	public override void _Ready()
	{
		base._Ready();
		GlobalVariables.playerStat = baseStat.CreateInstance();
		GlobalVariables.playerStat.spellMana = 2; // set to 0 after testing
		GlobalVariables.playerStat.currentHealth = 25;
		GlobalVariables.playerStat.guard = 50;
		GlobalVariables.playerStat.shield = 5;

		playerStat = GlobalVariables.playerStat;

		progressBackground = GetNode<TextureRect>("CharacterTab/StatTab/ProgressBackground");

		HealthBar = progressBackground.GetNode<TextureRect>("HBoxContainer/HealthBar");
		GuardBar = progressBackground.GetNode<TextureRect>("HBoxContainer/GuardBar");
		ShieldBar = progressBackground.GetNode<TextureRect>("HBoxContainer/ShieldBar");

		healthLabel = GetNode<Label>("CharacterTab/StatTab/HealthLabel");
		guardLabel = GetNode<Label>("CharacterTab/StatTab/GuardLabel");
		shieldLabel = GetNode<Label>("CharacterTab/StatTab/ShieldLabel");
		
		UpdateValue();
	}

	// hp has constant scale, shield and guard share the same scale
	// if all hp, shield and guard sum are less than max heath, their value to display on has the same scale,
	// if it over flow, guard and shield share scale to fill,
	// subsequently from the left to right, health, shield, guard

	public void UpdateValue()
	{
		float max = playerStat.maxHealth;float health = playerStat.currentHealth;
		float shield = playerStat.shield;float guard = playerStat.guard;
		float barSize = progressBackground.Size.X;

		float hpWidth = barSize * (health / max);
		float shieldWidth, guardWidth;

		if (health + shield + guard <= max)		{
			shieldWidth = barSize * (shield / max);
			guardWidth = barSize * (guard / max);
		} else {
			float remainder = Mathf.Max(0, barSize - hpWidth);
			float totalSG = shield + guard;

			float shieldRatio = totalSG > 0 ? shield / totalSG : 0;
			float guardRatio = totalSG > 0 ? guard / totalSG : 0;

			shieldWidth = remainder * shieldRatio;
			guardWidth = remainder * guardRatio;
		}

		// Apply sizes
		HealthBar.CustomMinimumSize = new Vector2(hpWidth, 0);
		ShieldBar.CustomMinimumSize = new Vector2(shieldWidth, 0);
		GuardBar.CustomMinimumSize = new Vector2(guardWidth, 0);

		// Labels
		healthLabel.Text = $"{health}/{max}";
		guardLabel.Text = $"{guard}";
		shieldLabel.Text = $"{shield}";

		// Hide bars & labels if empty
		GuardBar.Visible = guard > 0;
		ShieldBar.Visible = shield > 0;
		guardLabel.Visible = guard > 0;
		shieldLabel.Visible = shield > 0;
	}


	public void _input(InputEvent @event){//action "Action" from input map, this is for testing
    if (@event is InputEventMouseMotion) return;
        if (@event.IsActionPressed("Action"))
        {
            // set some value to the player stat
			playerStat.currentHealth -= 1;	
			playerStat.takeDamage(10);

			UpdateValue();
        }

        if (@event.IsActionPressed("Action2"))
        {
			playerStat.heal(10);
			playerStat.shield += 10;
			UpdateValue();
        }
    }
}
