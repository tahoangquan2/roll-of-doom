using Godot;

public partial class PlayerChar : Character
{	PlayerStat playerStat;

	private Label healthLabel,guardLabel,shieldLabel;
	public override void CharacterSetUp(Stats stat)
	{
		base.CharacterSetUp(stat);
		playerStat = (PlayerStat)statInstance;		

		GlobalVariables.playerStat = playerStat;		
	}


	public override void Cycle()
	{
		playerStat.Cycle();
		UpdateStatsDisplay();
	}
}
