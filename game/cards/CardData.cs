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


    public CardData()
    {
        CardName = "Default Card";
        Cost = 0;
        CardType = EnumGlobal.enumCardType.Tower;
        Effects = new Array<CardEffect>();
    }

    // Constructor
    public CardData(string cardName, int cost, CardEffect effect)
    {
        CardName = cardName;
        Cost = cost;
        Effects = new Array<CardEffect> { effect };
    }    
}