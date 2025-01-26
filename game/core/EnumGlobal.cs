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
    ResetDeck,       // Shuffles all cards back into the deck (e.g., Reset Deck)

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
    Draw=7,
    Discard=8,
    Duplicate=9,
    ResetDeck=10
}

}
