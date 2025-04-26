using Godot;
using System;
using System.Collections.Generic;

public partial class Main : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public PlayerStat playerStat;
	public List<EnemyStat> enemyStats = new List<EnemyStat>();
	public override void _Ready()
	{
		GlobalAccessPoint.Instance.UpdateReferences();
		GD.Print("Main _Ready");
		//PrintTree(GetTree().Root, 0);
		GC.Collect();
	}
	

	private void PrintTree(Node node, int depth)
	{
		string indent = "";
		for (int i = 0; i < depth; i++)
		{
			indent += "-   ";
		}
		GD.Print(indent + node.Name);
		foreach (Node child in node.GetChildren())
		{
			PrintTree(child, depth + 1);
		}
	}

	//input
	// public override void _Input(InputEvent @event)
	// {
	// 	if (@event.IsActionPressed("ui_filedialog_refresh"))
	// 	{
	// 		GetTree().ReloadCurrentScene();			
	// 	}

	// 	//GD.Print(GetViewport().GuiGetFocusOwner());
	// }
}
