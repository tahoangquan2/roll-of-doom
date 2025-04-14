using System.Threading.Tasks;
using Godot;

[GlobalClass]
public partial class CardEffect : Resource
{
    public virtual Task<bool> ApplyEffect(Node2D target)
    {
        return Task.FromResult(true);
    }
}