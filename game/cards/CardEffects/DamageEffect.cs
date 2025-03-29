using Godot;
public partial class DamageEffect : CardEffect
{
    public override bool ApplyEffect(Node2D target)
    {
        if (target is Node2D playArea)
        {
            //playArea.AddChild(spellEffect);
            

            return true;
        }
        return false;
    }
}
