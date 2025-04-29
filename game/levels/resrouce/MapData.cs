// res://scripts/MapData.cs
using Godot;
using Godot.Collections;

[Tool]
[GlobalClass]
public partial class MapData : Resource
{
    [Export] public int Width;
    [Export] public int FloorCount;
    [Export] public Vector2 Start; // (floor, pos)
    [Export] public Vector2 End;   // (floor, pos)
    [Export] public Array<RoomDataResource> Rooms = new Array<RoomDataResource>();

    //room path
    [Export] public Array<Vector2> nodePath = new Array<Vector2>();

    public override string ToString(){
        string str = $"Width: {Width}, FloorCount: {FloorCount}, Start: {Start}, End: {End}\n";
        str += "Node path:\n";
        foreach (var node in nodePath)
        {
            str += $"({node.X}, {node.Y})\n";
        }
        return str;
    }
}
