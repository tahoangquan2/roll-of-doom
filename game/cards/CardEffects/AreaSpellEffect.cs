using Godot;
public partial class AreaSpellEffect : CardEffect
{
    [Export] public PackedScene SpellEffectPrefab;
    [Export] public float Radius = 100f;

    public override bool ApplyEffect(Node2D target)
    {
        if (target is Node2D playArea)
        {
            Node2D spellEffect = (Node2D)SpellEffectPrefab.Instantiate();
            playArea.AddChild(spellEffect);
            spellEffect.GlobalPosition = playArea.GlobalPosition;
            GD.Print($"Casted {spellEffect.Name} with radius {Radius}");

            return true;
        }
        return false;
    }
}
