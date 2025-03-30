using Godot;

public partial class EnumGlobal : Node
{
    public enum enumCardType
    {
        Attack,
        Defense,
        Spell,
    }

    public enum enumCardTargetLayer
    {
        None=1,
        Enemy=2, 
        Ally=3,
    }

    public enum enumCardEffect
    {   DirectDamage,    // Deals direct damage to enemies 
        // Card Management (Deck Manipulation)
        Draw,            // Draws one or more cards (e.g., Draw New Card, Triple Draw)
        ShuffleDeck,     // Shuffles all cards in the deck (e.g., Shuffle Deck)
        DiscardHand,     // Discards all cards in the hand (e.g., Hand Wipe)
    }
    public enum enumDeckEffect
    {
        Draw=enumCardEffect.Draw,
        ShuffleDeck=enumCardEffect.ShuffleDeck,
        DiscardHand=enumCardEffect.DiscardHand
    }

    public enum HandSelectionPurpose
    {
        None,      // Default (no selection),
        Choose, // Selecting cards for a specific purpose
        Discard,   // Selecting cards to discard
        Duplicate, // Selecting a card to duplicate
        Upgrade,    // Selecting a card to upgrade
        
    }

public enum CardKeywords
{
    // The card does not got to discard pile at Cycle.
    Keep, 

    /// <summary>The card cannot be played.</summary>
    Unplayable,

    /// <summary>When <b>Cycle</b> while this is in hand, <b>Forget</b>.</summary>
    Needy,

    /// <summary>When played, <b>Forget</b>.</summary>
    Ephemeral,

    /// <summary>When drawn, draw an additional card.</summary>
    Bundled,

    /// <summary>When drawn, activates without costing mana.</summary>
    Auto,

    /// <summary>While in hand, grants an ongoing effect.</summary>
    Aura,

    /// <summary>Cannot be <b>Discarded</b> or <b>Forgotten</b>; can only be played once per turn.</summary>
    Eternal,

    /// <summary>Always starts in your opening hand.</summary>
    Fated,

    /// <summary>When <b>Restock</b>, automatically moves to your hand.</summary>
    Magnetic,

    /// <summary>Temporary cards added to weaken the player (e.g., burns, wounds, curses).</summary>
    Status,

    /// <summary>Cannot be <b>Dodge</b>.</summary>
    Precise,

    /// This effect activates whenever the card is put into the <b>Discard</b> or <b>Forgotten</b> pile.
    Overwork
}



    public enum State
    {
        MainMenu,
        PreGame,
        GameMap,
        Load,
        GamePlay,
        Settings,
        GameOver
    }

    public enum RoomType
    {
        Start,
        Battle,
        Boss,
        MiniBoss,
        CardShop,
        Rest,
        Treasure,        
    }
}
