using Godot;

public partial class CardMenuUi : CenterContainer
{
	// Called when the node enters the scene tree for the first time.
	public CardData cardData{ get; private set; } = null;
	private Sprite2D cardArt => GetNode<Sprite2D>("CardVisual/CardArt");
	private Label cardName => GetNode<Label>("CardVisual/CardName");

	private Sprite2D cardRarity => GetNode<Sprite2D>("CardVisual/CardRarity");
	private Label cardCost => GetNode<Label>("CardVisual/CostDisplay/CostLb");
	private Sprite2D cardType => GetNode<Sprite2D>("CardVisual/CardTypeIcon");

	private CardManager cardManager=null;

		private bool isSelected = false;
		public bool IsSelected => isSelected;

	public void ToggleSelection()
	{	
		isSelected = !isSelected;
		if (isSelected)
		{
			cardArt.Modulate = new Color(.7f, 1, .7f, 0.5f);
		}
		else
		{
			cardArt.Modulate = new Color(1, 1, 1, 1);
			cardRarity.Modulate = new Color(1, 1, 1, 1);
		}
	}

	public Button button => GetNode<Button>("CardVisual/Button");

	//set card data
	public void SetCardData(CardData cardData)
	{
		this.cardData = cardData;
		UpdateGraphics();
		cardManager = GetTree().CurrentScene.GetNodeOrNull<CardManager>(GlobalAccessPoint.cardManagerPath);
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
	}

	public void _on_button_2_pressed(){
		cardManager.EmitSignal(nameof(CardManager.CardSelect), cardData);
	}

}
