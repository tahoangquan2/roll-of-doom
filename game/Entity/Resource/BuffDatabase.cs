using Godot;
using System.Collections.Generic;
using System;

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

    public static BuffLogic GetLogicForBuff(EnumGlobal.BuffType type)
    {
        return type switch
        {
            EnumGlobal.BuffType.Fragile => new FragileLogic(),
            EnumGlobal.BuffType.Dodge => new DodgeLogic(),
            EnumGlobal.BuffType.Armed => new ArmedLogic(),
            EnumGlobal.BuffType.Fortify => new FortifyLogic(),
            EnumGlobal.BuffType.Pump => new ArmedLogic(),
            EnumGlobal.BuffType.Vigilant => new FortifyLogic(),
            EnumGlobal.BuffType.Poisoned => new PoisonedLogic(),
            EnumGlobal.BuffType.Exhaust => new ExhaustLogic(),
            EnumGlobal.BuffType.Bounce => new BounceLogic(),

            //EnumGlobal.BuffType.Burn => new BurnLogic(),
            //EnumGlobal.BuffType.Slow => new SlowLogic(),
            //EnumGlobal.BuffType.Critical => new CriticalLogic(),

            // Add other buffs here
            _ => null,
        };
    }

    public static EnumGlobal.BuffDuration GetBuffDuration(EnumGlobal.BuffType type)
    {        
        return buffMap[type].Duration ;
    }

    public static bool TryGetBuffType(string name, out EnumGlobal.BuffType type)
    {
        return Enum.TryParse(name, out type)
            && buffMap.ContainsKey(type);
    }
}
