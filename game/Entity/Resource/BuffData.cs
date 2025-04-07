using Godot;
[Tool]
[GlobalClass]
public partial class BuffData : Resource
{

	[Export] public EnumGlobal.BuffType Type;
	[Export] public EnumGlobal.BuffDuration Duration; // Volatile, Diminishing, Permanent
	[Export] public Texture2D icon = null; // icon for the buff
	[Export] public string description = "This buff has no description.";
	public Vector2 iconCoordinates = new Vector2(0,0);
}
