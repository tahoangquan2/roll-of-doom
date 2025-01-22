using Godot;

[GlobalClass]
public partial class CardEffect : Resource
{
    public virtual void ApplyEffect(Node2D target)
    {
        GD.Print("Base effect applied.");
    }
}
