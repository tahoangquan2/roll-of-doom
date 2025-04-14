using Godot;
using Godot.Collections;

[Tool]
[GlobalClass]
public partial class CardData : Resource
{
    [Export] public string CardName { get; set; }
    [Export] public int Cost { get; set; }
    [Export] public EnumGlobal.enumCardType CardType { get; set; }
    [Export] public Array<CardEffect> Effects { get; set; } = new Array<CardEffect>();
    [Export] public Script card_script { get; set; } = null; // Store script instance
    [Export] public string Description { get; set; }
    // card art, and art offset
    [Export] public Texture2D CardArt { get; set; } = null;
    [Export] public Vector2 ArtOffset { get; set; }
    [Export] public Array<EnumGlobal.CardKeywords> Keywords { get; set; } = new Array<EnumGlobal.CardKeywords>();
    [Export] public EnumGlobal.enumCardTargetLayer TargetMask { get; set; } = EnumGlobal.enumCardTargetLayer.None;    

    public CardData()
    {
        CardName = "Default Card";
        Cost = 0;
        CardType = EnumGlobal.enumCardType.Spell;
        Description = "This card has no description.";
        Effects = new Array<CardEffect>();
        CardArt = null;        
        ArtOffset = new Vector2(0, 0);
        Keywords = new Array<EnumGlobal.CardKeywords>();
        TargetMask = EnumGlobal.enumCardTargetLayer.None;
    }

    // Constructor
    public CardData(string cardName, int cost,string description, CardEffect effect,Script script,Texture2D cardArt, Vector2 artOffset)
    {
        CardName = cardName;
        Cost = cost;
        Description = description;
        Effects = new Array<CardEffect> { effect };
        card_script = script;
        CardArt = cardArt;
        ArtOffset = artOffset;
        Keywords = new Array<EnumGlobal.CardKeywords>();
        TargetMask = EnumGlobal.enumCardTargetLayer.None;
    }
}