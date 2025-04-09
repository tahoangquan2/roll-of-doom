using Godot;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

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

	public async Task EndTurn()
	{
		Disabled = true;
		EmitSignal(nameof(TurnEnded));
		//GD.Print("Turn Ended");
		await EnemyTurn();
		StartCycle();
	}
	public async void StartCycle()
	{
		//GD.Print("Cycle Started");

		var groupNodes = GetTree().GetNodesInGroup("Update on Cycle");
		//GD.Print($"Found {groupNodes.Count} nodes in group 'Update on Cycle'");
		var tasks = new List<Task>();

		foreach (var node in groupNodes)
		{
			// Try casting to Node and using reflection to call the method properly
			if (node is Node n)			{
				var method = n.GetType().GetMethod("Cycle");
				if (method != null )
					if (method.ReturnType == typeof(Task)){
						Task task = (Task)method.Invoke(n, null);
						tasks.Add(task);
					} else {
						method.Invoke(n, null);
					}
			}
		}

		//GD.Print($"Waiting for {tasks.Count} tasks to complete...");

		await Task.WhenAll(tasks);

		//GD.Print("All cycle tasks completed");

		EmitSignal(nameof(CycleStarted));
		Disabled = false;
	}


	public void _on_pressed()
	{
		EndTurn();		
	}

	private async Task EnemyTurn()
	{
		// group Node "Enemy Turn"
		var groupNodes = GetTree().GetNodesInGroup("Enemy Turn");

		//GD.Print($"Found {groupNodes.Count} nodes in group 'Enemy Turn'");
		var tasks = new List<Task>();

		foreach (var node in groupNodes)
		{
			// Try casting to Node and using reflection to call the method properly
			if (node is Node n)			{
				var method = n.GetType().GetMethod("EnemyTurn");
				if (method != null )
					if (method.ReturnType == typeof(Task)){
						Task task = (Task)method.Invoke(n, null);
						tasks.Add(task);
					} else {
						method.Invoke(n, null);
					}
			}
		}

		//GD.Print($"Waiting for {tasks.Count} tasks to complete...");
		await Task.WhenAll(tasks);		
	}
}
