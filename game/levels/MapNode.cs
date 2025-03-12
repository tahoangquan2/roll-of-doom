public class MapNode
{
    public int Floor { get; } // X position in the grid
    public int Pos { get; } // Y position in the grid
    public bool[] Connections { get; } = new bool[3] { false, false, false }; // Connections: left, straight, right of the next floor

	public EnumGlobal.RoomType nodeType; 

    public MapNode(int x, int y)
    {
        Floor = x;
        Pos = y;
    }

	public void SetConnection( int next)
	{
		if (next>Pos) Connections[2] = true;
		else if (next<Pos) Connections[0] = true;
		else Connections[1] = true;
	}

    public override string ToString()
    {
        return $"({Floor}, {Pos})";
    }
}