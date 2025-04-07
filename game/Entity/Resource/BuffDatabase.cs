using Godot;
using System.Collections.Generic;

[GlobalClass]

public partial class BuffDatabase : Node
{
    private static Dictionary<EnumGlobal.BuffType, BuffData> buffMap = new();

    public static PackedScene buffScene = ResourceLoader.Load<PackedScene>("res://game/Entity/buffUI.tscn");

    public override void _Ready()
    {
        foreach (var file in DirAccess.GetFilesAt("res://game/Entity/Resource/Buff/"))
        {
            if (file.EndsWith(".tres"))
            {
                var buffData = ResourceLoader.Load<BuffData>("res://game/Entity/Resource/Buff/"+file);
				if (buffData != null)
				{
					buffMap[buffData.Type] = buffData;
					//GD.Print($"Loaded Buff: {buffData.Type} - {buffData.description}");
				}
			}
        }
    }

    public static BuffData GetBuffData(EnumGlobal.BuffType type)
    {
        return buffMap.ContainsKey(type) ? buffMap[type] : null;
    }
}
