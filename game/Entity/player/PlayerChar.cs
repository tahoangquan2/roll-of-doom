using Godot;

public partial class PlayerChar : CardPlayZone
{
	[Export] PlayerStat baseStat;
	private TextureRect HealthBar;
	private TextureRect GuardBar;
	private TextureRect ShieldBar;
	private TextureRect EmptyBar;

	private float barSize = 100; // size of the bar in pixels
	public override void _Ready()
	{
		base._Ready();
		GlobalVariables.playerStat = baseStat.CreateInstance();
		GlobalVariables.playerStat.spellMana = 2; // set to 0 after testing

		barSize = GetNode<TextureRect>("ProgressBackground").Size.X;

		HealthBar = GetNode<TextureRect>("ProgressBackground/HBoxContainer/HealthBar");
		GuardBar = GetNode<TextureRect>("ProgressBackground/HBoxContainer/GuardBar");
		ShieldBar = GetNode<TextureRect>("ProgressBackground/HBoxContainer/ShieldBar");
		EmptyBar = GetNode<TextureRect>("ProgressBackground/HBoxContainer/EmptyBar"); //this has fill expand
	}

	// if all hp, shield and guard sum are less than max heath, their value to display on has the same scale,
	//change to if it over flow, the three just share scale to fill,

	public void UpdateHealthBar(float health)
	{
		float healthPercent = health / GlobalVariables.playerStat.maxHealth;
		HealthBar.Size = new Vector2(barSize * healthPercent, HealthBar.Size.Y);
	}
}
