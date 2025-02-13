using Godot;
using System;

public partial class Player : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _on_info_toggle_toggled(bool button_pressed)
	{
		if (button_pressed)
		{
			GetNode<Label>("Info").Show();
		}
		else
		{
			GetNode<Label>("Info").Hide();
		}
	}
}
