using System.Threading.Tasks;
using Godot;

[GlobalClass]
public partial class CardEffect : Resource
{
    // public virtual bool ApplyEffect(Node2D target)  {
    //     GD.Print("Applying effect");
    //     return true;
    // } make this task

    public virtual Task<bool> ApplyEffect(Node2D target)
    {
        GD.Print("Applying effect");
        return Task.FromResult(true);
    }
}