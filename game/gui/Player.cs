using Godot;

public partial class Player : CanvasLayer
{
	// Called when the node enters the scene tree for the first time.
	private Label manaLabel;
	private Label goldLabel;
	private Control pauseMenu;

	public override void _Ready()
	{
		manaLabel = GetNode<Label>("TopGui/Container/BaseMana/TxtBox/PointLabel");
		goldLabel = GetNode<Label>("TopGui/Container/Gold/TxtBox/GoldLabel");

		pauseMenu = GetNode<Control>("Pause");

		update_values();
	}
	public void update_values() // access though unique name
	{
		manaLabel.Text = $"{GlobalVariables.playerStat.baseMana}";
		goldLabel.Text = $"{GlobalVariables.playerStat.gold}";
	}


}
