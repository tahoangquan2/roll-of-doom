using Godot;
using Godot.Collections;

[Tool]
[GlobalClass]
public partial class CardData : Resource
{
    [Export] public string CardName { get; set; }
    [Export] public int Cost { get; set; }
    [Export] public EnumGlobal.enumCardType CardType { get; set; }
    [Export] public Array<EffectLayer> Effects { get; set; } = new Array<EffectLayer>();
    [Export] public Script card_script { get; set; } = null; // Store script instance
    [Export] public string Description { get; set; }
    // card art, and art offset
    [Export] public Texture2D CardArt { get; set; } = null;
    [Export] public Vector2 ArtOffset { get; set; }
    [Export] public Array<EnumGlobal.CardKeywords> Keywords { get; set; } = new Array<EnumGlobal.CardKeywords>();
    [Export] public EnumGlobal.enumCardTargetLayer TargetMask { get; set; } = EnumGlobal.enumCardTargetLayer.None;   

    [Export] public Array<AdditionalExplanation> AdditionalExplanations { get; set; } = new Array<AdditionalExplanation>();
    [Export] public int CardID { get; set; } = -1; // index of the card in the card pool
    public enum AdditionalExplanation    {
        // all the mechanics of the combat
        None,Guard,Shield,Cycle,Draw,Discard,Restock,EndTurn,Scry,
        // all the buff 
        Dodge,Bounce,Fortify,Armed,Vigilant,Pump,Exhaust,Fragile,Poisoned, Forget
    } 

    public CardData()
    {
        CardName = "Default Card";
        Cost = 0;
        CardType = EnumGlobal.enumCardType.Spell;
        Description = "This card has no description.";
        //Effects = new Array<CardEffect>();
        CardArt = null;        
        ArtOffset = new Vector2(0, 0);
        Keywords = new Array<EnumGlobal.CardKeywords>();
        TargetMask = EnumGlobal.enumCardTargetLayer.None;
    }
}