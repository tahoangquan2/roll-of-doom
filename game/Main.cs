using Godot;
using System;

public partial class Main : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GlobalAccessPoint.Instance.UpdateReferences();
		GD.Print("Main _Ready");
		GC.Collect();
		fpsLabel = GetNode<Label>("FPS");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		fpsLabel.Text = "FPS: " + Engine.GetFramesPerSecond();
	}

	private void DeferredPrintTree()
	{
		//PrintTree(GetTree().Root, 0);
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
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_filedialog_refresh"))
		{
			GetTree().ReloadCurrentScene();			
		}

		//GD.Print(GetViewport().GuiGetFocusOwner());
	}

	private Label fpsLabel;
}
