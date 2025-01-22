using Godot;
using System;

public partial class Card : Node2D
{
    [Export] public CardData CardData { get; set; }
    public Cardslot CurrentSlot { get; set; }

    [Signal] public delegate void CardHoveredEventHandler(Card card);
    [Signal] public delegate void CardUnhoveredEventHandler(Card card);

    private Label costLbl;
    private Label nameLbl;
    private Label descriptionLbl;
    private Sprite2D baseSprite;

    public Card()
    {
        CardData = new CardData(); // Ensure initialization
    }

    public override void _Ready()
    {
        costLbl = GetNode<Label>("CostDisplay/CostLb");
        nameLbl = GetNode<Label>("NameDisplay/NameLb");
        descriptionLbl = GetNode<Label>("CardEffectLb");
        baseSprite = GetNode<Sprite2D>("BaseCardSprite");

        if (CardData != null)
        {
            GD.Print($"Card Name: {CardData.CardName}");
            GD.Print($"Cost: {CardData.Cost}");
            GD.Print($"Effect: {CardData.Effect}");
        }

        // Connect card to CardManager
        CardManager parentManager = GetParent() as CardManager;
        parentManager?.ConnectCardSignals(this);
        // next Sibiling of cardmanager is Hand

        UpdateGraphics();
    }

    private void UpdateGraphics()
    {
        if (CardData == null) return;

		costLbl.Text = CardData.Cost.ToString();
		nameLbl.Text = CardData.CardName;
		descriptionLbl.Text = CardData.Effect;
    }

    public void Highlight()
    {
        baseSprite.Modulate = new Color(1, 0.5f, 0.1f, 1);
    }

    public void Unhighlight()
    {
        baseSprite.Modulate = new Color(1, 1, 1, 1);
    }
    
    public override void _Process(double delta)
    {
        //UpdateGraphics();
    }

    // 🟢 Emit signals correctly on mouse enter/exit
    public void _on_area_2d_mouse_entered()
    {        
        EmitSignal(nameof(CardHovered), this);
    }

    public void _on_area_2d_mouse_exited()
    {
        EmitSignal(nameof(CardUnhovered), this);
    }
}
