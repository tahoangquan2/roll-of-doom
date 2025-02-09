using System;
using Godot;

public partial class MiddleScreenSelection : Control 
{
	// list of buttons
	private Button[] buttons ;
	// list of 2d nodes
	private System.Collections.Generic.List<Node2D> nodes = new System.Collections.Generic.List<Node2D>();
	private Vector2 itemSize;
	private HBoxContainer hboxContainer;

	private Action<int> onItemChosenCS=null;
	private Callable onItemChosenGD;

	public override void _Ready()
	{
		hboxContainer = GetChild<HBoxContainer>(1);
		GlobalAccessPoint.GetCardManager().Lock(); // stop card interaction
	}
	public void Init(
		System.Collections.Generic.List<Node2D> nodes,Vector2 itemSize, object onSelection
	)// pass in list of 2d nodes, item size and what to do when item is chosen
	{
		buttons = new Button[nodes.Count];
		this.itemSize = itemSize;

		if (onSelection is Action<int> csharpCallback){
			onItemChosenCS = csharpCallback;
		}
		else if (onSelection is Callable gdscriptCallback){
			onItemChosenGD = gdscriptCallback;
		}

		for (int i = 0; i < nodes.Count; i++)
		{
			Button button = new Button();
			button.Text = "Button " + i;
			int index = i;
			button.ButtonDown += () => OnItemChosen(index);
			buttons[i] = button;

			button.CustomMinimumSize = itemSize;
			button.SizeFlagsHorizontal = SizeFlags.Fill;
			button.SizeFlagsVertical = SizeFlags.Fill;
			
			hboxContainer.AddChild(button);			
		}
	}

	private void OnItemChosen(
		int buttonIndex
	)	{
		GD.Print("Button " + buttonIndex + " pressed");
		GlobalAccessPoint.GetCardManager().Unlock(); 

		// this will execute the medthoed that was passed in

		if (onItemChosenCS != null){
			onItemChosenCS(buttonIndex);
		}
		else {
			onItemChosenGD.Call(buttonIndex);
		}
		
		QueueFree();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) // set up the position of the nodes
	{
		for (int i = 0; i < nodes.Count; i++)
		{
			nodes[i].GlobalPosition = buttons[i].GlobalPosition+new Vector2(itemSize.X/2, itemSize.Y/2);
		}

		SetProcess(false);
	}

	public void _on_button_pressed(){
		GD.Print("Button pressed");
		GlobalAccessPoint.GetCardManager().Unlock();
		QueueFree();
	}
}
