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

    private ShaderMaterial shaderMaterial;

    private const float AngleXMax = 10.0f; // Adjust for rotation limits
    private const float AngleYMax = 10.0f;


    public Card()
    {
        CardData = new CardData(); // Ensure initialization
    }

    public override void _Ready()
    {
        costLbl = GetNode<Label>("CostDisplay/CostLb");
        nameLbl = GetNode<Label>("NameDisplay/NameLb");
        descriptionLbl = GetNode<Label>("CardEffectLb");
        baseSprite = GetNode<Sprite2D>("CardMockup");

        if (baseSprite.Material is ShaderMaterial mat)
        {
            shaderMaterial = mat; // Store shader material reference
        }


        if (CardData != null)
        {
            // GD.Print($"Card Name: {CardData.CardName}");
            // GD.Print($"Cost: {CardData.Cost}");
            // GD.Print($"Effect: {CardData.Effect}");
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

    //reset shader

    
    
    public override void _Process(double delta)
    {   
       
    }

    // public void Shadering(Vector2 mousePos) 
    // {
        
    //     if (shaderMaterial == null) return;

    //     Vector2 size = GetViewportRect().Size;

    //     float lerpValX = Mathf.Remap(mousePos.X, 0.0f, size.X, 0, 1);
    //     float lerpValY = Mathf.Remap(mousePos.Y, 0.0f, size.Y, 0, 1);

    //     float rotX = Mathf.RadToDeg(LerpAngle(-AngleXMax, AngleXMax, lerpValX));
    //     float rotY = Mathf.RadToDeg(LerpAngle(AngleYMax, -AngleYMax, lerpValY));

    //     shaderMaterial.Set("x_rot", rotY);
    //     shaderMaterial.Set("y_rot", rotX);

    //     GD.Print($"x_rot: {shaderMaterial.Get("x_rot")}, y_rot: {shaderMaterial.Get("y_rot")}"); // Debugging
    // }

    // public void ResetShader()
    // {
    //     if (shaderMaterial == null) return;

    //     shaderMaterial.Set("x_rot", 0.0f);
    //     shaderMaterial.Set("y_rot", 0.0f);
    // }

    // private float LerpAngle(float from, float to, float weight)
    // {
    //     return from + (to - from) * weight;
    // }

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
