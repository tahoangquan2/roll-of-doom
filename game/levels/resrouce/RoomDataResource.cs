// res://scripts/RoomDataResource.cs
using Godot;

[Tool]
[GlobalClass]
public partial class RoomDataResource : Resource
{
    [Export] public int Floor;
    [Export] public int Pos;
    [Export] public EnumGlobal.RoomType Type;
    // which of the 3 possible connections (left, straight, right) exist
    [Export] public Godot.Collections.Array<bool> Connections = new Godot.Collections.Array<bool> { false, false, false };
}
