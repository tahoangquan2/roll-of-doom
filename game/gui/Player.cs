using Godot;
using System;

public partial class Player : Control
{
	// Called when the node enters the scene tree for the first time.
	private int health;
	private int spirit;

	private Label healthLabel;
	private Label spiritLabel;
	public override void _Ready()
	{
		health=GlobalVariables.health;
		spirit=GlobalVariables.spirit;

		healthLabel = GetNode<Label>("TopGui/HealthLabel");
		spiritLabel = GetNode<Label>("TopGui/PointLabel");

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
		spirit=GlobalVariables.spirit;

		healthLabel.Text = $"Health: {health}";
		spiritLabel.Text = $"Spirit: {spirit}";
	}
}
