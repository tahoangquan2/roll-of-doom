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

		playerStat.currentHealth = 25;
		playerStat.guard = 50;
		playerStat.shield = 5;

		// add a buff to the player stat
		playerStat.ApplyBuff(EnumGlobal.BuffType.Fragile, 1);playerStat.ApplyBuff(EnumGlobal.BuffType.Fragile, 5);
		playerStat.ApplyBuff(EnumGlobal.BuffType.Fragile, 10);
		playerStat.ApplyBuff(EnumGlobal.BuffType.Dodge, 10);
		playerStat.ApplyBuff(EnumGlobal.BuffType.Pump, 10);

		//every buff in the buff database

		playerStat.ApplyBuff(EnumGlobal.BuffType.Fortify, 10);
		playerStat.ApplyBuff(EnumGlobal.BuffType.Bounce, 10);
		playerStat.ApplyBuff(EnumGlobal.BuffType.Armed, 10);
		playerStat.ApplyBuff(EnumGlobal.BuffType.Vigilant, 10);
		playerStat.ApplyBuff(EnumGlobal.BuffType.Fortify, 1);
		
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
