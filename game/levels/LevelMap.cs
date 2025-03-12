using Godot;
using System;
using System.Collections.Generic;

public partial class LevelMap : Control
{
	private NinePatchRect mapBackground;
	private Camera2D camera;
	private GridContainer mapContainer;
	// Map Template: A predetermined set of positions where Rooms can be generated or not. (A Grid)
	// Rooms: Each individual place you can visit. (Also known as a Nodes).
	// Paths: Lines connecting Rooms. (A connection between two Nodes).
	// Floors: Rooms on the same horizontal level. (Nodes on the same X Axis).
	// Locations: Specifies what can be expected when visited.
	// Boss Room: Additional Room (or Node) added to the top post generation.

	//Initially the game generates a 7x15 Irregular Isometric Grid (a Grid formed by triangles). 
	// Then it randomly chooses one of the Rooms on the 1st Floor (at the bottom of the Grid). 
	// It then connects it with a Path to one of the 3 closest Rooms on the 2nd Floor. It continues this pattern to the next Floor Floor.
	// It repeats this procces 6 time obeying the following rules 2:
	// -The First 2 Rooms randomly chosen at the 1rst Floor cannot be the same. Ensuring that there are always at least 2 different starting locations.
	// -Paths Cannot cross over each other.


	[Export] public int Width { get; set; } = 7;
	[Export] public int Floor { get; set; } = 15;
	[Export] public int PathCnt { get; set; } = 6;

	// path only connect from current floor to the next floor
	// path can only connect to the 3 closest rooms on the next floor (left, right, center)
	// path cannot cross over each other ie cannot have [0,0] -> [1,1] and [0,1] -> [1,0]
	private HashSet<int> startingPoint = new HashSet<int>();
	private MapNode[,] Rooms;
	private bool mapGenerated = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		mapBackground = GetNode<NinePatchRect>("MapBackground");
		camera = GetNode<Camera2D>("Camera2D");
		mapContainer = GetNode<GridContainer>("MapBackground/GridContainer");
		Rooms = new MapNode[Floor,Width];

		//generate 15*7 button inside grid container and set to size expand fill
		for (int i = 0; i < Floor; i++)
		{
			for (int j = 0; j < Width; j++)
			{
				var button = new Button();
				button.Text = $"({j},{i})";
				button.SizeFlagsHorizontal= SizeFlags.ExpandFill;
				button.SizeFlagsVertical = SizeFlags.ExpandFill;
				mapContainer.AddChild(button);
				Rooms[i,j]=new MapNode(i,j);
			}
		}

		setCameraLimit();
	}

	private void GenerateMap() // at least two on the first floor
	{
		int random = GlobalVariables.GetRandomNumber(0, Width-1);
		startingPoint.Add(random);
		GeneratePath(0,random);
		
		while (true){
			random = GlobalVariables.GetRandomNumber(0, Width-1);
			if (!startingPoint.Contains(random)){
				startingPoint.Add(random);
				GeneratePath(0,random);
				break;
			}
		}

		for (int i=2;i<PathCnt;i++){
			random = GlobalVariables.GetRandomNumber(0, Width-1);
			startingPoint.Add(random);
			GeneratePath(0,random);
		}
	}
	
	private void GeneratePath(int floor, int pos)
	{	
		// get random position on the next floor left, right, center
		int nextFloor = floor+1;
		int random = GlobalVariables.GetRandomNumber(-1, 1);
		int nextPos =  Math.Min(Math.Max(pos + random, 0), Width - 1);

		// check if the path is not crossing over each other
		// check for adjacent node
		
		int adjacentPos=pos-1;
		if (adjacentPos>=0 && Rooms[floor,adjacentPos].Connections[2] && nextPos==adjacentPos){
			nextPos=Math.Min(pos + GlobalVariables.GetRandomNumber(0, 1), Width - 1);	
		}

		adjacentPos=pos+1;
		if (adjacentPos<Width && Rooms[floor,adjacentPos].Connections[0] && nextPos==adjacentPos){
			nextPos=Math.Max(pos + GlobalVariables.GetRandomNumber(-1, 0), 0);		
		}

		// add path
		Rooms[floor,pos].SetConnection(nextPos);
		//set color of the button to indicate path :modulate to red
		mapContainer.GetChild<Button>(pos+floor*Width).Modulate=Color.Color8(255,0,0,255);
		
		GD.Print($"({floor},{pos}) -> ({nextFloor},{nextPos})");
		//add Line2D to indicate path
		if (nextFloor>=Floor) return;
		var line = new Line2D();
		line.AddPoint(mapContainer.GetChild<Button>(pos+floor*Width).Position);
		line.AddPoint(mapContainer.GetChild<Button>(nextPos+nextFloor*Width).Position);
		mapBackground.AddChild(line);		
		
		GeneratePath(nextFloor,nextPos);
	}

	private void setCameraLimit() // based on the mapbackground size
	{
		var mapSize = mapBackground.Size;
		var mapPosition = mapBackground.Position;
		camera.LimitLeft = -500;
		//camera.LimitRight = (int)mapBackground.Size.X;
		camera.LimitTop = -100;
		camera.LimitBottom = (int)(mapBackground.Size.Y+mapPosition.Y+200);
	}

	// process 
	public override void _Process(double delta)
	{
		if (!mapGenerated) {
			mapGenerated = true;
			GenerateMap();
		}
	}
}
