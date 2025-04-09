using System.Threading.Tasks;
using Godot;
public partial class DamageEffect : CardEffect
{
    public override Task<bool> ApplyEffect(Node2D target)
    {
        if (target is Node2D playArea)
        {
            //playArea.AddChild(spellEffect);
            

            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
}
