using Godot;

public partial class CardPlayZone : Area2D
{
    [Export] public EnumGlobal.enumCardTarget targetType; // Type of target for the play zone
    private CollisionShape2D collisionShape;
    public override void _Ready()
    {
        base._Ready();
        collisionShape = GetChild(0) as CollisionShape2D;
        if (collisionShape == null)
        {
            GD.PrintErr("CollisionShape2D not found in CardPlayZone.");
            return;
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public void activeCard(Card card, Vector2 actionPoint)
    {
        GD.Print($"Card {card.cardData.CardName} Played");

        // Apply all effects associated with this card
        card.ActivateEffects(this);
    }

    public void _on_area_entered(Node2D area)
    {
        Card card = area.GetParent() as Card;
        if (area is Card card)
        {
            card.Scale = new Vector2(1.05f, 1.05f);
            card.ZIndex = 11;
            card.Rotation = 0;
            card.ResetShader();
        }
    }
    public void _on_area_exited(Node2D area)
    {
        if (area is Card card)
        {
            card.Scale = new Vector2(1.0f, 1.0f);
            card.ZIndex = 10;
            card.Rotation = 0;
            card.ResetShader();
        }
    }

}
