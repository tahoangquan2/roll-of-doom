using System;
using Godot;

public partial class MiddleScreenSelection : Control 
{
	// list of buttons
	private Button[] buttons ;
	// list of 2d nodes
	private Godot.Collections.Array<Node2D> nodes = new Godot.Collections.Array<Node2D>();
	private Vector2 itemSize;
	private HBoxContainer hboxContainer;

	private Action<int> onItemChosenCS=null;
	private Callable onItemChosenGD;
	private bool isInitialized = false;
	private bool isGDscript = true;

	public override void _Ready()
	{
		hboxContainer = GetNode<HBoxContainer>("Container");
		GlobalAccessPoint.GetCardManager().Lock(); // stop card interaction
	}
	public void InitializeSelection(
		Godot.Collections.Array<Node2D> nodes,Vector2 itemSize, 
		Callable onSelection
	)// pass in list of 2d nodes, item size and what to do when item is chosen
	{
		this.nodes = nodes;
		buttons = new Button[nodes.Count];
		this.itemSize = itemSize;

		onItemChosenGD = onSelection;

		setUp();
	}

	public void InitializeSelection(
		Godot.Collections.Array<Node2D> nodes,Vector2 itemSize, 
		Action<int> onSelection
	)
	{
		this.nodes = nodes;
		buttons = new Button[nodes.Count];
		this.itemSize = itemSize;

		onItemChosenCS = onSelection;
		isGDscript = false;

		setUp();
	}

	private void setUp()
	{
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
			button.MouseDefaultCursorShape = CursorShape.PointingHand;
			hboxContainer.AddChild(button);			
		}

		if (nodes.Count==0){
			OnItemChosen(-1);
		}				
		isInitialized = true;
	}

	public override void _Process(double delta)    
	{
		if (!isInitialized) return;
		for (int i = 0; i < nodes.Count; i++)
		{
			nodes[i].GlobalPosition = buttons[i].GlobalPosition+new Vector2(itemSize.X/2, itemSize.Y/2);
			nodes[i].ZIndex = CardGlobal.forCardSelectZindex;
		}
		SetProcess(false);
	}

	private void OnItemChosen(
		int buttonIndex
	)	{
		GlobalAccessPoint.GetCardManager().Unlock(); 

		// this will execute the medthoed that was passed in

		if (isGDscript)
		{
			onItemChosenGD.Call(buttonIndex);
		}
		else
		{
			onItemChosenCS(buttonIndex);
		}
		
		QueueFree();
	}

	public void _on_button_pressed(){
		OnItemChosen(0);
	}
}
