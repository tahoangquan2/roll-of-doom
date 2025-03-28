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
	private Callable onItemChosenGD;
	private bool isInitialized = false;

	private string buttonPath = "";

	public override void _Ready()
	{
		hboxContainer = GetNode<HBoxContainer>("Container");
		GlobalAccessPoint.GetCardManager().Lock(); // stop card interaction
	}
	public void InitializeSelection(
		Godot.Collections.Array<Node2D> nodes,Vector2 itemSize, 
		Callable onSelection,string buttonPath=""
	)// pass in list of 2d nodes, item size and what to do when item is chosen
	{
		this.nodes = nodes;
		buttons = new Button[nodes.Count];
		this.itemSize = itemSize;

		onItemChosenGD = onSelection;
		this.buttonPath = buttonPath;

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
			
			button.Modulate = new Color(1, 1, 1, 0.0f);
			button.CustomMinimumSize = itemSize;
			button.SizeFlagsHorizontal = SizeFlags.Fill;
			button.SizeFlagsVertical = SizeFlags.Fill;
			button.MouseDefaultCursorShape = CursorShape.PointingHand;
			hboxContainer.AddChild(button);			
			// mouse pass
			button.MouseFilter = MouseFilterEnum.Pass;
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
		for (int i=0;i<nodes.Count;i++){
			// remove button from the container then add it to nodes
			hboxContainer.RemoveChild(buttons[i]);
			nodes[i].GetNode(buttonPath).AddChild(buttons[i]);
			buttons[i].Position = new Vector2(0, 0);
		}
		SetProcess(false);
	}

	private void OnItemChosen(
		int buttonIndex
	)	{
		// this will execute the medthoed that was passed in

		onItemChosenGD.Call(buttonIndex);

		GlobalAccessPoint.GetCardManager().Unlock(); 
		
		QueueFree();
	}

	public void _on_button_pressed(){ // cancel and choose the first item
		OnItemChosen(0);
		for (int i = 0; i < nodes.Count; i++)
		{
			buttons[i].QueueFree();
			nodes[i].ZIndex = 0;
		}
	}
}
