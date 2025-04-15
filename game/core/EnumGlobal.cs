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
        None=2,
        Enemy=3, 
        Ally=4,
        Self=5,
    }

public enum enumCardEffect
{
    DirectDamage, DealAoE, DealRandom,
    CheckEnemyStat, CheckSelfStat,
    Heal,
    GainShield, GainGuard,
    GainMana, GainSpellMana,
    ApplyBuff, ApplyDebuff, ConditionalEffect,
    Draw, Discard, Forget, Restock, DiscardUpTo,
    EndTurn, ShuffleHandtoDeck, ShuffleDeck, DiscardHand
}
    public enum enumNonTargetedEffect
    {
        DealAoE = enumCardEffect.DealAoE,
        DealRandom = enumCardEffect.DealRandom,
        CheckSelfStat = enumCardEffect.CheckSelfStat,
        GainShield = enumCardEffect.GainShield,
        GainGuard = enumCardEffect.GainGuard,
        GainMana = enumCardEffect.GainMana,
        GainSpellMana = enumCardEffect.GainSpellMana,
        //these are inherited from enumNonTargetedEffect
        //ApplyBuff = enumCardEffect.ApplyBuff,     // apply buff to self  
        //ApplyDebuff = enumCardEffect.ApplyDebuff, // apply debuff to all enemies
        Heal = enumCardEffect.Heal,

        EndTurn = enumCardEffect.EndTurn,
        Draw=enumCardEffect.Draw,
        Discard=enumCardEffect.Discard,  // must discard the amount
        DiscardUpTo=enumCardEffect.DiscardUpTo, // discard up to X cards
        Forget=enumCardEffect.Forget,
        ShuffleDeck=enumCardEffect.ShuffleDeck,
        DiscardHand=enumCardEffect.DiscardHand,
        Restock=enumCardEffect.Restock,
        ShuffleHandtoDeck=enumCardEffect.ShuffleHandtoDeck, 
    }

    public enum PileSelectionPurpose
    {
        None,      // Default (no selection),
        Choose, // Selecting cards for a specific purpose
        Discard,   // Selecting cards to discard
        Forget,   // Selecting cards to forget
        Shuffle,  // Selecting cards to shuffle
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

	public enum BuffType {
		Dodge,Bounce,Fortify, Armed, Vigilant, Pump, Exhaust, Fragile, Poisoned
	}

	public enum BuffDuration { 
		Volatile, // Disappears at Cycle
        Diminishing, // Loses 1 value at Cycle
        Permanent
	}	

    public enum Charms
    {
        None,
        Attack,
        Defense,
        Spell,
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
