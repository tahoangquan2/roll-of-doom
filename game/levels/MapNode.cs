using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Godot;
public partial class MapNode : TextureButton
{
    public int Floor { get;set; } // X position in the grid
    public int Pos { get;set; } // Y position in the grid
    public bool[] Connections { get; } = new bool[3] { false, false, false }; // Connections: left, straight, right of the next floor

    private HashSet<MapNode> Nexts = new HashSet<MapNode>();

    private MapNode parentRoom=null;

	public EnumGlobal.RoomType nodeType; 

    public static int FloorCount = 15;
    public static int WidthCount = 7;
    public static Vector2 mapBackgroundSize = new Vector2(800, 800);
    private int sizeOfButton =0;

    private static Texture2D[] TextureforNode = new Texture2D[7];    

    private static PackedScene packedLine = GD.Load<PackedScene>("res://game/levels/MapLine.tscn");

    private Control linesParent;

    private static float lineOffset = 30; // offset for the line to be drawn from the center of the button

    public void setUp(int x,int y){
        Floor = x;
        Pos = y;
        TextureNormal = GD.Load<Texture2D>("res://assets/maps/crowned-skull.png");
        linesParent = GetNode<Control>("Lines");
        MouseDefaultCursorShape = CursorShape.PointingHand;

        int offsetLimit = (int) mapBackgroundSize.Y /FloorCount/5;
        int randomOffset = GlobalVariables.GetRandomNumber(-offsetLimit, offsetLimit);

        // set position of the button based on the size of the map background
        Position = new Vector2((y+1)*mapBackgroundSize.X/(WidthCount+1),
			(x+2) * mapBackgroundSize.Y / (FloorCount+3)+randomOffset);	

        Position-= new Vector2(Size.X/2, Size.Y/2);

        sizeOfButton = (int)Size.X;

        assignType();
        Disabled = true;
    }
	public void SetConnection( int next,MapNode nextNode)
	{
		if (next>Pos) Connections[2] = true;
		else if (next<Pos) Connections[0] = true;
		else Connections[1] = true;

        Nexts.Add(nextNode);

        var line = (Line2D) packedLine.Instantiate();

        Vector2 point1 = new Vector2(sizeOfButton/2, sizeOfButton/2);
        Vector2 point2 = nextNode.Position-Position+new Vector2(sizeOfButton/2, sizeOfButton/2);

        // move the two point nearer to each other by the offset
        var angle = point1.AngleToPoint(point2);
        point1 += new Vector2(lineOffset * Mathf.Cos(angle), lineOffset * Mathf.Sin(angle));
        point2 += new Vector2(-lineOffset * Mathf.Cos(angle), -lineOffset * Mathf.Sin(angle));

		line.AddPoint(point1);
		line.AddPoint(point2);
        
        
		linesParent.AddChild(line);	
	}

    public bool isNone()
    {
        return !Connections[0] && !Connections[1] && !Connections[2];
    }

    public override string ToString()
    {
        return $"({Floor}, {Pos})";
    }

    public static void LoadTexture()
    {
        TextureforNode[(int)EnumGlobal.RoomType.Start] = GD.Load<Texture2D>("res://assets/maps/barbute.png") ;
        TextureforNode[(int)EnumGlobal.RoomType.Battle] = GD.Load<Texture2D>("res://assets/maps/dread-skull.png") ;
        TextureforNode[(int)EnumGlobal.RoomType.Boss] = GD.Load<Texture2D>("res://assets/maps/crowned-skull.png") ;
        TextureforNode[(int)EnumGlobal.RoomType.MiniBoss] = GD.Load<Texture2D>("res://assets/maps/daemon-skull.png") ;
        TextureforNode[(int)EnumGlobal.RoomType.CardShop] = GD.Load<Texture2D>("res://assets/maps/letter-bomb.png") ;
        TextureforNode[(int)EnumGlobal.RoomType.Rest] = GD.Load<Texture2D>("res://assets/maps/campfire.png") ;
        TextureforNode[(int)EnumGlobal.RoomType.Treasure] = GD.Load<Texture2D>("res://assets/maps/locked-chest.png") ;
    }

    public void assignType()
    {
        if (Floor == -1)
            nodeType = EnumGlobal.RoomType.Start;
        else if (Floor == FloorCount)
            nodeType = EnumGlobal.RoomType.Boss;
        else {
            nodeType = dungeonFloors[Floor][GlobalVariables.GetRandomNumber(0, dungeonFloors[Floor].Count-1)];
        }
        TextureNormal = TextureforNode[(int)nodeType];
        TextureDisabled = TextureforNode[(int)nodeType];            
    }
    
    //_on_toggled
    public void _on_toggled(bool toggled_on)
    {
        //animation player
        if (toggled_on){
            GetChild<AnimationPlayer>(1).Play("Chosen");

            foreach (var next in Nexts)
            {
                next.Disabled = false;
                next.parentRoom = this;
            }

            toggleLines(true);

            if (parentRoom != null)
            {
                foreach (var next in parentRoom.Nexts)
                {
                    next.Disabled = true;
                }
                parentRoom.toggleLines(false);
            }
        }
    }

    private void toggleLines(bool toggle)
    {        
        foreach (var child in linesParent.GetChildren())
        {
            Line2D line = (Line2D) child;
            ShaderMaterial shaderMat = (ShaderMaterial)line.Material;
            if (toggle ) // set material.shader_paramerter/speed
            {
                shaderMat.SetShaderParameter("speed", 0.2);     
            }
            else
            {
                shaderMat.SetShaderParameter("speed", 0);
            }
        }
    }

    private static List<List<EnumGlobal.RoomType>> dungeonFloors = new List<List<EnumGlobal.RoomType>>
    {
        new List<EnumGlobal.RoomType> { EnumGlobal.RoomType.Battle }, // Floor 0
        new List<EnumGlobal.RoomType> { EnumGlobal.RoomType.Treasure, EnumGlobal.RoomType.CardShop }, // Floor 1
        new List<EnumGlobal.RoomType> { EnumGlobal.RoomType.Battle }, // Floor 2
        new List<EnumGlobal.RoomType> { EnumGlobal.RoomType.Battle }, // Floor 3
        new List<EnumGlobal.RoomType> { EnumGlobal.RoomType.Rest, EnumGlobal.RoomType.Treasure }, // Floor 4
        new List<EnumGlobal.RoomType> { EnumGlobal.RoomType.MiniBoss }, // Floor 5
        new List<EnumGlobal.RoomType> { EnumGlobal.RoomType.CardShop }, // Floor 6
        new List<EnumGlobal.RoomType> { EnumGlobal.RoomType.Rest, EnumGlobal.RoomType.Treasure }, // Floor 7
        new List<EnumGlobal.RoomType> { EnumGlobal.RoomType.Battle }, // Floor 8
        new List<EnumGlobal.RoomType> { EnumGlobal.RoomType.Battle }, // Floor 9
        new List<EnumGlobal.RoomType> { EnumGlobal.RoomType.Treasure }, // Floor 10
        new List<EnumGlobal.RoomType> { EnumGlobal.RoomType.Battle }, // Floor 11
        new List<EnumGlobal.RoomType> { EnumGlobal.RoomType.Battle }, // Floor 12
        new List<EnumGlobal.RoomType> { EnumGlobal.RoomType.CardShop }, // Floor 13
        new List<EnumGlobal.RoomType> { EnumGlobal.RoomType.Rest, EnumGlobal.RoomType.Treasure }, // Floor 14
        new List<EnumGlobal.RoomType> { EnumGlobal.RoomType.Boss }  // Floor 15 (Final Boss)
    };
}