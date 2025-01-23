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

    private readonly float AngleXMax = Mathf.DegToRad(7.0f); // Adjust for rotation limits 
    private readonly float AngleYMax = Mathf.DegToRad(7.0f); 

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

        if (baseSprite.Material is ShaderMaterial mat){
            shaderMaterial = mat;
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

        UpdateGraphics();
    }

    private void UpdateGraphics()
    {
        if (CardData == null) return;

		costLbl.Text = CardData.Cost.ToString();
		nameLbl.Text = CardData.CardName;
		descriptionLbl.Text = CardData.Effect;
    }  
    
    public override void _Process(double delta)
    {         
    }

    public void Shadering(Vector2 mousePos) 
    {
        if (shaderMaterial == null) return;

        Vector2 size = baseSprite.Texture.GetSize();

        float lerpValX = Mathf.Remap(mousePos.X, 0.0f, size.X, 0, 1)+0.5f;
        float lerpValY = Mathf.Remap(mousePos.Y, 0.0f, size.Y, 0, 1)+0.5f;

        float rotX =  Mathf.RadToDeg(Mathf.LerpAngle(-AngleXMax, AngleXMax, lerpValX));
        float rotY =  Mathf.RadToDeg(Mathf.LerpAngle(AngleYMax, -AngleYMax, lerpValY));

        shaderMaterial.SetShaderParameter("x_rot", rotY);
        shaderMaterial.SetShaderParameter("y_rot", rotX);
    }

    public void ResetShader()
    {   if (shaderMaterial == null) return;
        shaderMaterial.SetShaderParameter("x_rot", 0.0f);
        shaderMaterial.SetShaderParameter("y_rot", 0.0f);
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
