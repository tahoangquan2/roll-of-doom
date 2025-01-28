using Godot;
public partial class PlaceTowerEffect : CardEffect
{
    [Export] public PackedScene TowerToPlace;  // Assign different tower scenes
    [Export] public Vector2 PlacementOffset = new Vector2(0, 0);

    public override void ApplyEffect(Node2D target)
    {
        if (target is Node2D playArea)
        {
            Node2D newTower = (Node2D)TowerToPlace.Instantiate();
            playArea.AddChild(newTower);
            newTower.GlobalPosition = playArea.GlobalPosition + PlacementOffset;
            GD.Print($"Placed tower: {newTower.Name} at {newTower.GlobalPosition}");
        }
    }
}
