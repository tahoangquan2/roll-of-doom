using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;

public partial class LevelMap : Control
{
	private NinePatchRect mapBackground;
	private Camera2D camera;
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

	bool isLoading = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		mapBackground = GetNode<NinePatchRect>("MapBackground");
		camera = GetNode<Camera2D>("Camera2D");
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
			}
		}

		startNode = (MapNode) packedRoom.Instantiate();
		startNode.setUp(-1,(Width-1)/2);
		endNode = (MapNode) packedRoom.Instantiate();
		endNode.setUp(Floor,(Width-1)/2);
		
		mapBackground.AddChild(startNode);
		mapBackground.AddChild(endNode);		

		setCameraLimit();

		if (FileAccess.FileExists("user://map_data.tres"))		{
			GD.Print("Loading map data from file " +OS.GetUserDataDir());
			isLoading = true;
			var mapData = ResourceLoader.Load<MapData>("user://map_data.tres");
			if (mapData != null)			{
				RestoreFromMapData(mapData);

				setStartEnd();
				return; // skip GenerateMap()
			}
		}

		GenerateMap();
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
		SaveMapTRES();
	}

	private void setStartEnd()
	{
		// path to the start to the first floor
		for (int i=0;i<Width;i++) if (Rooms[0,i]!=null)
		{
			startNode.SetConnection(i,Rooms[0,i]);			
		}

		// path to last floor to the end
		for (int i=0;i<Width;i++) if (Rooms[Floor-1,i]!=null)
		{
			Rooms[Floor-1,i].SetConnection((Width-1)/2,endNode);
		}

		if (!isLoading)
			startNode._on_toggled(true);
	}
	
	private void AssignRooms()
	{
		// assign room type
		for (int i = 0; i < Floor; i++)
		{
			for (int j = 0; j < Width; j++)			{
				if (Rooms[i,j].isNone())				{
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

	// check for input to reload the map
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_filedialog_refresh"))
		{
			GetTree().ReloadCurrentScene();			
		}
	}

	public void SaveMapTRES()
    {
        var mapData = new MapData {
            Width      = Width,        
            FloorCount = Floor,        
            Start      = new Vector2(startNode.Floor, startNode.Pos), 
            End        = new Vector2(endNode.Floor,   endNode.Pos),
        };

        // collect each room’s data
        for (int x = 0; x < Floor; x++)
        for (int y = 0; y < Width; y++)
        {
            var node = Rooms[x,y];
            if (node == null) continue;

            var rd = new RoomDataResource {
                Floor       = node.Floor,          
                Pos         = node.Pos,            
                Type        = node.nodeType,       

               	Connections = new Array<bool> {
                	node.Connections[0],
                	node.Connections[1],
                	node.Connections[2]
            	},
            };
            mapData.Rooms.Add(rd);
        }

		mapData.nodePath = getPath();

        // actually write the .tres
        ResourceSaver.Save(mapData,"user://map_data.tres");
    }

	private void RestoreFromMapData(MapData data)
	{
		GD.Print("Restoring map data from file "+data.ToString());
		 // 1) Quickly build a HashSet of the saved positions
		var saved = new HashSet<(int floor, int pos)>();
		foreach (var rd in data.Rooms)
        	saved.Add((rd.Floor, rd.Pos));

		// 2) First, remove any node *not* in that set
		for (int x = 0; x < Floor; x++)
		for (int y = 0; y < Width; y++)
		{
			if (!saved.Contains((x, y)) && Rooms[x,y] != null)
			{
				mapBackground.RemoveChild(Rooms[x,y]);
				Rooms[x,y].QueueFree();
				Rooms[x,y] = null;
			}
		}
		
		// 3) Now, for every saved position, restore type + connections
		foreach (var rd in data.Rooms)
		{
			var node = Rooms[rd.Floor, rd.Pos];
			node.nodeType = rd.Type;
			node.assignType(rd.Type);}
		foreach (var rd in data.Rooms)
		{
			var node = Rooms[rd.Floor, rd.Pos];
			if (node == null) continue;

			for (int i = 0; i < 3; i++)
			{
				if (!rd.Connections[i]) continue;
				int nf = rd.Floor + 1;
				int np = Mathf.Clamp(rd.Pos + (i-1), 0, Width-1);
				var nn = (nf < Floor) ? Rooms[nf,np] : endNode;
				if (nn != null)
					node.SetConnection(i, nn);
			}
		}

		// (C) highlight the player’s branch
		foreach (var v in data.nodePath)
		{
			var n = Rooms[(int)v.X, (int)v.Y];
			if (n != null)
			{
				n.pathed = true;
				n.toggleNoEffect();
			}
		}

		// (D) re‐attach start/end links & show first floor
		setStartEnd();
	}

	private Array<Vector2> getPath()
	{
		var path = new Array<Vector2>();
		foreach (var node in Rooms) if (node != null && node.pathed)
		{
			path.Add(new Vector2(node.Floor, node.Pos));
		}
		return path;
	}

}
