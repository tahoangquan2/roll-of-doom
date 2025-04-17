using Godot;
using Godot.Collections;
[GlobalClass]
public partial class EffectLayer : Resource
{
    [Export] public Array<CardEffect> LayerEffects { get; set; } = new Array<CardEffect>();
}
