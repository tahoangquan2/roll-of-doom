using Godot;

public partial class CardPlayZone : Area2D
{
    private CollisionShape2D collisionShape;

    [Export] public EnumGlobal.enumCardTargetLayer playZoneType=EnumGlobal.enumCardTargetLayer.None; // Type of play zone (e.g., Ally, Enemy)
    public override void _Ready()
    {
        collisionShape = GetChild(0) as CollisionShape2D;
        if (collisionShape == null)
        {
            GD.PrintErr("CollisionShape2D not found in CardPlayZone.");
            return;
        }

        // Set the collision mask for the play zone

        SetCollisionLayerValue((int) playZoneType , true); // Enable collision mask for the play zone
        GD.Print($"PlayZoneType: {playZoneType}");
    }

    public void activeCard(Card card, Vector2 actionPoint)
    {
        GD.Print($"Card {card.cardData.CardName} Played");

        // Apply all effects associated with this card
        card.ActivateEffects(this);
    }
}
