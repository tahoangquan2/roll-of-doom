using Godot;

public partial class PlayerChar : Character
{	PlayerStat playerStat => statInstance as PlayerStat;

	private Label healthLabel,guardLabel,shieldLabel;
	public override void _Ready()
	{
		base._Ready();
		GlobalVariables.allStats.Clear();
		GlobalVariables.playerStat = playerStat;
		GlobalVariables.allStats.Add(playerStat);		

		playerStat.spellMana = 2; // set to 0 after testing
		playerStat.currentHealth = 25;
		playerStat.guard = 50;
		playerStat.shield = 5;

		// add a buff to the player stat
		AddBuff(EnumGlobal.BuffType.Dodge, 5);
		
		UpdateStatsDisplay();
	}


	public void _input(InputEvent @event){//action "Action" from input map, this is for testing
    if (@event is InputEventMouseMotion) return;
        if (@event.IsActionPressed("Action"))
        {
            // set some value to the player stat
			playerStat.currentHealth -= 1;	
			playerStat.TakeDamage(10);
        }

        if (@event.IsActionPressed("Action2"))
        {
			playerStat.heal(10);
			playerStat.Add_guard(10);
        }
    }


	public override void Cycle()
	{
		playerStat.Cycle();
		UpdateStatsDisplay();
	}
}
