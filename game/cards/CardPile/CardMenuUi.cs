using Godot;

public partial class CardMenuUi : CenterContainer
{
	// Called when the node enters the scene tree for the first time.
	private CardData cardData;
	private Sprite2D cardArt => GetNode<Sprite2D>("CardVisual/CardArt");
	private Label cardName => GetNode<Label>("CardVisual/CardName");

	private Sprite2D cardRarity => GetNode<Sprite2D>("CardVisual/CardRarity");
	private Label cardCost => GetNode<Label>("CardVisual/CostDisplay/CostLb");
	private Sprite2D cardType => GetNode<Sprite2D>("CardVisual/CardTypeIcon");

	private CardManager cardManager=null;

	Button button => GetNode<Button>("CardVisual/Button");

	//set card data
	public void SetCardData(CardData cardData)
	{
		this.cardData = cardData;
		UpdateGraphics();
		cardManager = GetTree().CurrentScene.GetNodeOrNull<CardManager>(GlobalAccessPoint.cardManagerPath);
	}

	public void SetFunctionForButtonPressed(Callable onSelection)
	{
		button.Connect("pressed", onSelection);
	}

	private void UpdateGraphics()
    {
        if (cardData == null) return;

		cardName.Text = cardData.CardName;
		cardCost.Text = cardData.Cost.ToString();
		cardArt.Texture = cardData.CardArt;
        switch (cardData.CardType)
        {
            case EnumGlobal.enumCardType.Attack:
                cardType.Frame = 1;
                break;
            case EnumGlobal.enumCardType.Defense:
                cardType.Frame = 2;
                break;
            case EnumGlobal.enumCardType.Spell:
                cardType.Frame = 0;
                break;
            default:
                cardType.Frame = 0;
                break;
        }
    }  

	public void _on_button_2_mouse_entered(){
		cardRarity.Modulate = new Color(.9f, .9f, 0.9f, 1);
	}

	public void _on_button_2_mouse_exited(){
		cardRarity.Modulate = new Color(1, 1, 1, 1);
	}

	public void _on_button_pressed()
	{
		GD.Print("Card pressed button 1");
	}

	public void _on_button_2_pressed(){
		GD.Print("Card pressed button 2");
		cardManager.EmitSignal(nameof(CardManager.CardSelect), cardData);
	}

}
