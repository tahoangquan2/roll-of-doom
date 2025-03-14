using Godot;
using System;
using System.Collections.Generic;

public partial class LevelMap : Control
{
	private NinePatchRect mapBackground;
	private Camera2D camera;
	private Control lines;
	// Map Template: A predetermined set of positions where Rooms can be generated or not. (A Grid)
	// Rooms: Each individual place you can visit. (Also known as a Nodes).
	// Paths: Lines connecting Rooms. (A connection between two Nodes).
	// Floors: Rooms on the same horizontal level. (Nodes on the same X Axis).
	// Locations: Specifies what can be expected when visited.

	//Initially the game generates a 7x15 Irregular Isometric Grid (a Grid formed by triangles). 
	// Then it randomly chooses one of the Rooms on the 1st Floor (at the bottom of the Grid). 
	// It then connects it with a Path to one of the 3 closest Rooms on the 2nd Floor. It continues this pattern to the next Floor Floor.
	// It repeats this procces 6 time obeying the following rules 2:
	// -The First 2 Rooms randomly chosen at the 1rst Floor cannot be the same. Ensuring that there are always at least 2 different starting locations.
	// -Paths Cannot cross over each other.


	[Export] public int Width { get; set; } = 7;
	[Export] public int Floor { get; set; } = 14;
	[Export] public int PathCnt { get; set; } = 6;

	private PackedScene packedLine = GD.Load<PackedScene>("res://game/levels/MapLine.tscn");
	private PackedScene packedRoom = GD.Load<PackedScene>("res://game/levels/MapNode.tscn");

	// path only connect from current floor to the next floor
	// path can only connect to the 3 closest rooms on the next floor (left, right, center)
	// path cannot cross over each other ie cannot have [0,0] -> [1,1] and [0,1] -> [1,0]
	private HashSet<int> startingPoint = new HashSet<int>();
	private MapNode[,] Rooms;
	private MapNode startNode;
	private MapNode endNode;
	private bool mapGenerated = false;

	private int sizeOfButton =0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		mapBackground = GetNode<NinePatchRect>("MapBackground");
		camera = GetNode<Camera2D>("Camera2D");
		lines = GetNode<Control>("MapBackground/Lines");
		Rooms = new MapNode[Floor,Width];
		Vector2 mapBackgroundSize= mapBackground.Size;
		

		MapNode.FloorCount = Floor;
		MapNode.WidthCount = Width;
		MapNode.mapBackgroundSize = mapBackgroundSize;
		MapNode.LoadTexture();

		//generate 15*7 button inside grid container and set to size expand fill
		for (int i = 0; i < Floor; i++)
		{
			for (int j = 0; j < Width; j++)
			{
				Rooms[i,j]=(MapNode) packedRoom.Instantiate();
				Rooms[i,j].setUp(i,j);
				mapBackground.AddChild(Rooms[i,j]);
				// set position of the button based on the size of the map background
				sizeOfButton = (int)Rooms[i,j].Size.X;							
			}
		}

		startNode = (MapNode) packedRoom.Instantiate();
		startNode.setUp(-1,(Width-1)/2);
		endNode = (MapNode) packedRoom.Instantiate();
		endNode.setUp(Floor,(Width-1)/2);
		
		mapBackground.AddChild(startNode);
		mapBackground.AddChild(endNode);
		

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

		// assign Rooms
		AssignRooms();

		setStartEnd();
	}

	private void setStartEnd()
	{
		// path to the start to the first floor
		for (int i=0;i<Width;i++) if (Rooms[0,i]!=null)
		{
			startNode.SetConnection(i,Rooms[0,i]);
			var line = (Line2D) packedLine.Instantiate();
			line.AddPoint(startNode.Position+new Vector2(sizeOfButton/2,sizeOfButton/2));
			line.AddPoint(Rooms[0,i].Position+new Vector2(sizeOfButton/2,sizeOfButton/2));
			lines.AddChild(line);	
		}

		// path to last floor to the end
		for (int i=0;i<Width;i++) if (Rooms[Floor-1,i]!=null)
		{
			var line = (Line2D) packedLine.Instantiate();
			line.AddPoint(Rooms[Floor-1,i].Position+new Vector2(sizeOfButton/2,sizeOfButton/2));
			line.AddPoint(endNode.Position+new Vector2(sizeOfButton/2,sizeOfButton/2));
			lines.AddChild(line);	
			Rooms[Floor-1,i].SetConnection((Width-1)/2,endNode);
		}
	}
	
	private void AssignRooms()
	{
		// assign room type
		for (int i = 0; i < Floor; i++)
		{
			for (int j = 0; j < Width; j++)
			{
				// Rooms[i,j].nodeType = (EnumGlobal.RoomType)GlobalVariables.GetRandomNumber(0, 3);
				// // remmove that button that is not connected to any path
				if (Rooms[i,j].isNone())
				{
					mapBackground.RemoveChild(Rooms[i,j]);
					Rooms[i,j].QueueFree();
					Rooms[i,j]=null;
				} else{

				}
			}
		}
	}

	private void GeneratePath(int floor, int pos)
	{	
		// get random position on the next floor left, right, center
		int nextFloor = floor+1;
		int random = GlobalVariables.GetRandomNumber(-1, 1);
		int nextPos =  Math.Clamp(pos + random, 0, Width - 1);

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

		
		if (nextFloor>=Floor) 
		{
			// add path to the end
			Rooms[floor,pos].SetConnection(nextPos,endNode);
			return;
		}
		// add path
		Rooms[floor,pos].SetConnection(nextPos,Rooms[nextFloor,nextPos]);	
		//add Line2D to indicate path		
		var line = (Line2D) packedLine.Instantiate();
		line.AddPoint(Rooms[floor,pos].Position+new Vector2(sizeOfButton/2,sizeOfButton/2));
		line.AddPoint(Rooms[nextFloor,nextPos].Position+new Vector2(sizeOfButton/2,sizeOfButton/2));
		lines.AddChild(line);		
		
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
