using Godot;
using System.Linq;
using System.Threading.Tasks;

public partial class Card : Node2D
{
    [Export] public CardData cardData { get; set; }
    public bool canBeHovered = true;

    [Signal] public delegate void CardHoveredEventHandler(Card card);
    [Signal] public delegate void CardUnhoveredEventHandler(Card card);

    private Label costLbl;
    private Label nameLbl;
    private Label descriptionLbl;
    private Sprite2D CardTypeIcon;
    private Sprite2D CardArt;

    private ShaderMaterial shaderMaterial;

    private readonly float AngleXMax = Mathf.DegToRad(7.0f); // Adjust for rotation limits 
    private readonly float AngleYMax = Mathf.DegToRad(7.0f); 

    public Card()
    {
        cardData = new CardData(); // Ensure initialization
    }
    public void SetupCard(CardData cardData)
    {
        this.cardData = cardData;
        UpdateGraphics();
    }

    public override void _Ready()
    {
        costLbl = GetNode<Label>("SubViewport/CostDisplay/CostLb");
        nameLbl = GetNode<Label>("SubViewport/CardDisplay/CardFrontBannerDown/NameDisplay/NameLb");
        descriptionLbl = GetNode<Label>("SubViewport/CardEffectLb");
        CardTypeIcon = GetNode<Sprite2D>("SubViewport/CardTypeIcon");
        CardArt = GetNode<Sprite2D>("SubViewport/CardDisplay/CardArt");

        var subViewport = GetNode<SubViewport>("SubViewport");
        var shaderDisplay = GetNode<TextureRect>("Control/TextureRect"); // This will hold the shader

        shaderDisplay.Texture = subViewport.GetTexture();shaderDisplay.UseParentMaterial = false;

        // Apply the shader material
        if (shaderDisplay.Material is ShaderMaterial mat)
        {
            shaderMaterial = mat;
        }

        // Connect card to CardManager
        CardManager parentManager = GetParent() as CardManager;
        parentManager?.ConnectCardSignals(this);

        UpdateGraphics();
    }

    public void ActivateEffects(Node2D target)
    {
        if (cardData == null) return;

        if (!string.IsNullOrEmpty(cardData.ScriptFilePath))
        {
            // Load and execute the script if assigned
            Script script = GD.Load<Script>(cardData.ScriptFilePath);
            if (script != null)
            {
                GD.Print($"Executing script: {cardData.ScriptFilePath}");
                var scriptInstance = new Node();
                scriptInstance.SetScript(script);
                AddChild(scriptInstance);
                scriptInstance.Call("ApplyEffect", target);
                scriptInstance.QueueFree(); // Remove script node after execution
            }
            else
            {
                GD.PrintErr($"Script at {cardData.ScriptFilePath} not found.");
            }
        }
        else if (cardData.Effects != null)
        {
            foreach (var effect in cardData.Effects)
            {
                GD.Print($"Applying effect: {effect}");
                effect.ApplyEffect(target);
            }
        }

        KillCard();
    }



    private void UpdateGraphics()
    {
        if (cardData == null) return;

		costLbl.Text = cardData.Cost.ToString();
		nameLbl.Text = cardData.CardName;
		descriptionLbl.Text = cardData.Description.ToString();
        CardArt.Texture = cardData.CardArt;
        // Set the card type icon
        switch (cardData.CardType)
        {
            case EnumGlobal.enumCardType.Tower:
                CardTypeIcon.Frame = 1;
                break;
            case EnumGlobal.enumCardType.Spell:
                CardTypeIcon.Frame = 2;
                break;
            case EnumGlobal.enumCardType.Deck:
                CardTypeIcon.Frame = 0;
                break;
            default:
                CardTypeIcon.Frame = 0;
                break;
        }
    }  

    public void Shadering(Vector2 mousePos) 
    {
        if (shaderMaterial == null) return;

        Vector2 size = GlobalVariables.cardSize;

        float lerpValX = Mathf.Remap(mousePos.X, 0.0f, size.X, 0.0f, 1.0f)+0.5f;
        float lerpValY = Mathf.Remap(mousePos.Y, 0.0f, size.Y, 0.0f, 1.0f)+0.5f;

        float rotX =  Mathf.RadToDeg(Mathf.LerpAngle(-AngleXMax, AngleXMax, lerpValX));
        float rotY =  Mathf.RadToDeg(Mathf.LerpAngle(AngleYMax, -AngleYMax, lerpValY));

        shaderMaterial.SetShaderParameter("x_rot", rotY);
        shaderMaterial.SetShaderParameter("y_rot", rotX);
    }

    public void ResetShader(){
        if (shaderMaterial != null){
            shaderMaterial.SetShaderParameter("x_rot", 0f);shaderMaterial.SetShaderParameter("y_rot", 0f);
        }
    }

    // 🟢 Emit signals correctly on mouse enter/exit
    public void _on_area_2d_mouse_entered(){        
        if (!canBeHovered) return;
        EmitSignal(nameof(CardHovered), this);
    }

    public void _on_area_2d_mouse_exited(){
        if (!canBeHovered) return;
        EmitSignal(nameof(CardUnhovered), this);
    }

    public async void KillCard()
    {
        await PlayDissolveAnimation();
        GetParent().RemoveChild(this);
        QueueFree();
    }

    public async Task PlayDissolveAnimation()
    {
        var animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        animPlayer.Play("card_dissolveOrBurn"); // Ensure animation name matches
        await ToSignal(animPlayer, "animation_finished"); // Wait for the animation to end
    }
}