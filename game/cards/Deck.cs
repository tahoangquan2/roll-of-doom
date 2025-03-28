using Godot;

public partial class Deck : CardPile
{ 
    private DeckVisual deckVisual => GetNode<DeckVisual>("DeckVisual");
    
    // when the decksize is updated cardcount is updated

    public override void _Ready() { 
        cardCountOffset = new Vector2(20, -50);
        base._Ready();
        cardManager = GetTree().CurrentScene.GetNodeOrNull<CardManager>(GlobalAccessPoint.cardManagerPath);
        
        for (int i = 0; i < deckSize; i++)
        {
            CardData card = new CardData();
            card.CardName = $"Card {i}";
            card.Description = $"Description {i}";
            card.CardType = (EnumGlobal.enumCardType)GD.RandRange(0, 3);
            card.CardArt = GD.Load<CompressedTexture2D>($"res://assets/cards/Details/Backgrounds/Others/BackgroundCard{GD.RandRange(81, 93)}.png");
            card.Cost = GD.RandRange(1, 10);
            deck.Add(card);
        }
        CardCount.Text = deck.Count.ToString(); 
        EmitSignal(nameof(DeckUpdated), deck.Count);     
    }

    public Card DrawCard(int index){
        if (index < 0 || index >= deck.Count) return null;
        CardData card = deck[index];
        deck.RemoveAt(index);
        Card newCard=cardManager.createCard(card);
        newCard.Position = GlobalPosition;
        EmitSignal(nameof(DeckUpdated), deck.Count); 
        CardCount.Text = deck.Count.ToString(); 
        return newCard;
    }
    public Godot.Collections.Array<Card> DrawCards(int amount){
        Godot.Collections.Array<Card> drawnCards = new Godot.Collections.Array<Card>();
        amount = Mathf.Clamp(amount, 0, deck.Count);
        for (int i = 0; i < amount; i++)
        {   
            if (deck.Count == 0)
            {
                GD.Print("Deck is empty.");
                break;
            }
            CardData card = deck[0];            
            deck.RemoveAt(0);
            Card newCard = cardManager.createCard(card);
            drawnCards.Add(newCard);
            newCard.Position = deckVisual.getTopCardPosition();            
        }
        CardCount.Text = deck.Count.ToString(); 
        EmitSignal(nameof(DeckUpdated), deck.Count); 
        
        return drawnCards;
    }
  
}