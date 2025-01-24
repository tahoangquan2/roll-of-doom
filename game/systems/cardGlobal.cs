using Godot;
using System;

public partial class CardGlobal : Node
{
    public const int cardCollisionMask = 256;
    public const int cardSlotMask = 512;
    private static Random random = new Random(); // Ensure only one Random instance is used

    public static int GetRandomInt(int min, int max)
    {
        return random.Next(min, max);
    }

    	// make a function that raycasts and return object with passed in parameters
	public static Godot.Collections.Array<Godot.Collections.Dictionary> RaycastCheckForObjects(Node2D node, Vector2 position, int collisionMask)
    {
        if (node == null) 
        {
            GD.PrintErr("RaycastCheckForObjects: Passed node is null!");
            return new Godot.Collections.Array<Godot.Collections.Dictionary>();
        }

        var spaceState = node.GetWorld2D().DirectSpaceState;
        var parameters = new PhysicsPointQueryParameters2D
        {
            Position = position,
            CollideWithAreas = true,
            CollisionMask = (uint)collisionMask
        };

        return spaceState.IntersectPoint(parameters);
    }
}
