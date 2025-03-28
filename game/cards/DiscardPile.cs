using Godot;

public partial class DiscardPile : CardPile
{    
    // when the decksize is updated cardcount is updated
    public override void _Ready() { 
        cardCountOffset = new Vector2(20, -50);
        base._Ready();
        cardManager = GetTree().CurrentScene.GetNodeOrNull<CardManager>(GlobalAccessPoint.cardManagerPath);        
    }

}
