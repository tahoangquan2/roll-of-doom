using Godot;

public partial class EnumGlobal : Node
{
    public enum enumCardType
    {
        Tower,
        Spell,
        Deck
    }
    public enum enumCardEffect
    {
        // Tower Placement
        PlaceTower,      // Places a tower on the field

        // Spell Effects (Direct Impact on Enemies or Field)
        ZoneSpell,       // Applies an area effect spell (e.g., Fireball, Freeze, Wind Gust)
        DirectDamage,    // Deals direct damage to enemies (e.g., Fireball, Death Beam)
        Stun,            // Temporarily stops enemy movement (e.g., Freezx)
        PushBack,        // Pushes enemies back (e.g., Flush, Wind Gust)
        SummonBarrier,   // Creates a destructible or impassable wall (e.g., Ice Wall, Barrier Wall)
        Trap,            // Places a trap that activates on enemy contact (e.g., Bomb Trap)

        // Card Management (Deck Manipulation)
        Draw,            // Draws one or more cards (e.g., Draw New Card, Triple Draw)
        Discard,         // Forces the player to discard cards (e.g., Trade Off)
        Duplicate,       // Creates copies of a selected card (e.g., Duplicate Card)
        ShuffleDeck,     // Shuffles all cards in the deck (e.g., Shuffle Deck)
        ShuffleCard,     // Shuffles the card back into the deck (e.g., Shuffle Card)

        ShuffleHand,     // Shuffles all cards in the hand (e.g., Shuffle Hand)

        DiscardHand,     // Discards all cards in the hand (e.g., Hand Wipe)



        // Buff and Debuff (Affect Towers or Player)
        Buff,            // Enhances tower or hero attributes (e.g., Haste, Last Stand)
        Debuff,          // Weakens enemy stats (could be a future expansion)

        // Healing and Resource Management
        Heal,            // Restores health (e.g., Nature’s Haven, Healing Spring)
        ResourceGain,    // Generates in-game resources like mana or gold (e.g., Mana Pool)

        // Special Effects
        UpgradeTower,    // Improves tower stats or abilities (e.g., Research Center)
        DelayedEffect    // Activates an effect after a few waves (e.g., Library)
    }
    public enum enumDeckEffect
    {
        Draw=enumCardEffect.Draw,
        Discard=enumCardEffect.Discard,
        Duplicate=enumCardEffect.Duplicate,
        
        ShuffleDeck=enumCardEffect.ShuffleDeck,
        ShuffleCard=enumCardEffect.ShuffleCard,
        ShuffleHand=enumCardEffect.ShuffleHand,
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
