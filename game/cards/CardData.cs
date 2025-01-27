using System;
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
    [Export] public string Description { get; set; }
    // card art, and art offset
    [Export] public Texture2D CardArt { get; set; }
    [Export] public Vector2 ArtOffset { get; set; }




    public CardData()
    {
        CardName = "Default Card";
        Cost = 0;
        CardType = EnumGlobal.enumCardType.Tower;
        Description = "This card has no description.";
        Effects = new Array<CardEffect>();
        CardArt = null;        
        ArtOffset = new Vector2(0, 0);
    }

    // Constructor
    public CardData(string cardName, int cost,string description, CardEffect effect,Texture2D cardArt, Vector2 artOffset)
    {
        CardName = cardName;
        Cost = cost;
        Description = description;
        Effects = new Array<CardEffect> { effect };
        CardArt = cardArt;
        ArtOffset = artOffset;
    }
}