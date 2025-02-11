using System.Security.Cryptography.X509Certificates;
using Godot;

public partial class CardGlobal : Node
{
    public const int cardCollisionMask = 256;
    public const int cardSlotMask = 512;

    public const int forCardSelectZindex = 100;

    private static ShaderMaterial[] dissolveMaterials = null;
    private static ShaderMaterial[] burnMaterials = null;

    	// make a function that raycasts and return object with passed in parameters
	public static Godot.Collections.Array<Godot.Collections.Dictionary> RaycastCheckForObjects(Node2D node, Vector2 position, int collisionMask)
    {
        if (node == null) {
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

    // get shader material
    // ready
    public override void _Ready()
    {
        GetDissolveMaterial();
        GetBurnMaterial();
    }

    public static ShaderMaterial GetDissolveMaterial()
    {
        if (dissolveMaterials == null)
        {
            dissolveMaterials = new ShaderMaterial[3];
            for (int i = 0; i < 3; i++)
            {
                dissolveMaterials[i] = GD.Load<ShaderMaterial>($"res://assets/cards/cardDissolve_{i}.tres");
            }
        }
        
        return dissolveMaterials[GlobalVariables.GetRandomNumber(0, 3)];
    }

    public static ShaderMaterial GetBurnMaterial()
    {
        if (burnMaterials == null)
        {
            burnMaterials = new ShaderMaterial[3];
            for (int i = 0; i < 3; i++)
            {
                burnMaterials[i] = GD.Load<ShaderMaterial>($"res://assets/cards/cardBurnUp_{i}.tres");
            }
        }
        
        return burnMaterials[GlobalVariables.GetRandomNumber(0, 3)];
    }

    public static void LockCard(Card card)
    {
        card.canActivate = false;
        card.canBeMoved = false;
		card.ZIndex = forCardSelectZindex;
    }

    public static void UnlockCard(Card card)
    {
        card.canActivate = true;
        card.canBeMoved = true;
        card.ZIndex = 1;
    }
}
