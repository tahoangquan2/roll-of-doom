using Godot;

public partial class CardPlayZone : Area2D
{
    private CollisionShape2D collisionShape;

    [Export] public EnumGlobal.enumCardTargetLayer playZoneType=EnumGlobal.enumCardTargetLayer.None; // Type of play zone (e.g., Ally, Enemy)

    [Signal] public delegate void ZoneUpdateEventHandler(bool isEntered, CardPlayZone zone);

    private EnumGlobal.enumCardTargetLayer collisionLayer = EnumGlobal.enumCardTargetLayer.None;

    public override void _Ready()
    {
        collisionShape = GetChild(0) as CollisionShape2D;
        if (collisionShape == null)
        {
            GD.PrintErr("CollisionShape2D not found in CardPlayZone.");
            return;
        }

        SetCollisionLayerValue((int) playZoneType , true); // Enable collision mask for the play zone
        collisionLayer = playZoneType;
        
        GetTree().CurrentScene.GetNodeOrNull<CardManager>(GlobalAccessPoint.cardManagerPath).ConnectPlayZoneSignals(this);
    }

    public void activeCard(Card card, Vector2 actionPoint)
    {
        GD.Print($"Card {card.cardData.CardName} {card} Played ");

        card.ActivateEffects(this);
    }

    public EnumGlobal.enumCardTargetLayer GetPlayZoneType()
    {
        return playZoneType;
    }

    public void _on_mouse_entered()
    {
        EmitSignal(nameof(ZoneUpdate), true, this);
    }
    public void _on_mouse_exited()
    {
        EmitSignal(nameof(ZoneUpdate), false, this);
    }

    public override void _ExitTree()
    {
    }
}
