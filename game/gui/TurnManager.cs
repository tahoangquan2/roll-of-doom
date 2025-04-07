using Godot;
using System;

public partial class TurnManager : TextureButton
{
	[Signal] public delegate void TurnEndedEventHandler();
	[Signal] public delegate void CycleStartedEventHandler();
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void EndTurn()
	{
		//disable the button
		Disabled = true;
		EmitSignal(nameof(TurnEnded));
		GD.Print("Turn Ended");
		EnemyTurn();
	}
	public void StartCycle()
	{
		// Emit the CycleStarted signal
		GD.Print("Cycle Started");

		GetTree().CallGroup("Update on Cycle", "Cycle");
		EmitSignal(nameof(CycleStarted));
		// Enable the button
		Disabled = false;
	}

	public void _on_pressed()
	{
		//GD.Print("End Turn Button Pressed");
		EndTurn();
		
	}

	private async void EnemyTurn()
	{
		GD.Print("Enemy Turn");
		await ToSignal(GetTree().CreateTimer(1), "timeout");
		StartCycle();
	}
}
