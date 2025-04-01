using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Godot;

public partial class CardGlobal : Node
{
    public const int forCardSelectZindex = 100;

    public const int CardCollisionMask = 256; // 1

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
            dissolveMaterials = new ShaderMaterial[6];
            for (int i = 0; i < 6; i++)
            {
                dissolveMaterials[i] = GD.Load<ShaderMaterial>($"res://assets/cards/Card_Shader_Tres/cardDissolve_{i}.tres");
            }
        }
        
        return dissolveMaterials[GlobalVariables.GetRandomNumber(0, 5)];
    }

    public static ShaderMaterial GetBurnMaterial()
    {
        if (burnMaterials == null)
        {
            burnMaterials = new ShaderMaterial[6];
            for (int i = 0; i < 6; i++)
            {
                burnMaterials[i] = GD.Load<ShaderMaterial>($"res://assets/cards/Card_Shader_Tres/cardBurnUp_{i}.tres");
            }
        }
        
        return burnMaterials[GlobalVariables.GetRandomNumber(0, 5)];
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

    public static HashSet<CardData> currentCardDataPool = new HashSet<CardData>();
    // this is the pool of cards that are currently in the game
    public static HashSet<CardData> GetRandomCards(int amount)
    {
        HashSet<CardData> cards = new HashSet<CardData>();
        // get random cards from the pool hashset
        while (cards.Count < amount)
        {
            CardData card = currentCardDataPool.ElementAt(GlobalVariables.GetRandomNumber(0, currentCardDataPool.Count));
            cards.Add(card);
        }
        
        return cards;
    }

    public static Godot.Collections.Array<CardData> deckCurrent=new Godot.Collections.Array<CardData>(); 
    // in game can change, this is the deck that player has outside of the game
}
