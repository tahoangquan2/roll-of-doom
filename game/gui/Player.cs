using Godot;
using System;

public partial class Player : CanvasLayer
{
	// Called when the node enters the scene tree for the first time.
	private int health;
	private int maxHealth;
	private int spirit;

	private Label healthLabel;
	private Label spiritLabel;

	private TextureProgressBar healthProgressBar;
	public override void _Ready()
	{
		health=GlobalVariables.health;
		spirit=GlobalVariables.spirit;



		healthLabel = GetNode<Label>("TopGui/Container/Health/TxtBox/HealthLabel");
		spiritLabel = GetNode<Label>("TopGui/Container/Spirit/TxtBox/PointLabel");
		healthProgressBar = GetNode<TextureProgressBar>("TopGui/Container/Health/TextureProgressBar");

		GlobalVariables.gv.HealthChanged += update_values;
		GlobalVariables.gv.SpiritChanged += update_values;

		update_values();
	}
	public void _on_info_toggle_toggled(bool button_pressed)
	{
		if (button_pressed) 
			GetNode<Label>("Info").Show();
		else
			GetNode<Label>("Info").Hide();
	}

	public void update_values() // access though unique name
	{
		health=GlobalVariables.health;
		maxHealth=GlobalVariables.maxHealth;
		spirit=GlobalVariables.spirit;

		healthLabel.Text = $"{health}";
		spiritLabel.Text = $"{spirit}";
		healthProgressBar.MaxValue = maxHealth;
		healthProgressBar.Value = health;
	}
}
