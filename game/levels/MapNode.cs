public class MapNode
{
    public int X { get; } // X position in the grid
    public int Y { get; } // Y position in the grid
    public bool[] Connections { get; } = new bool[3] { false, false, false }; // Connections: left, straight, right of the next floor

	public EnumGlobal.RoomType nodeType; 

    public MapNode(int x, int y)
    {
        X = x;
        Y = y;
    }

	public void SetConnection( int next)
	{
		if (next>X) Connections[2] = true;
		else if (next<X) Connections[0] = true;
		else Connections[1] = true;
	}

    public override string ToString()
    {
        return $"({X}, {Y})";
    }
}