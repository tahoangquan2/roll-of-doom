using Godot;
using System.Threading.Tasks;

public partial class Card : Node2D
{
    [Export] public CardData cardData { get; set; }
    public bool canBeHovered = true;
    public bool canActivate = true;
    public bool canBeMoved = true;
    [Signal] public delegate void CardHoveredEventHandler(Card card);
    [Signal] public delegate void CardUnhoveredEventHandler(Card card);

    private Label costLbl;
    private Label nameLbl;
    private Label descriptionLbl;
    private Sprite2D CardTypeIcon;
    private Sprite2D CardArt;
    private ShaderMaterial shaderMaterial;
    private Control display;
    private AnimationPlayer animPlayer;
    private readonly float AngleXMax = Mathf.DegToRad(7.0f); 
    private readonly float AngleYMax = Mathf.DegToRad(7.0f); 

    public Tween cardTween=null;
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
        display = GetNode<Control>("Control");
        animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");        

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

    public virtual async void ActivateEffects(Node2D target)
    {
        if (cardData == null || !canActivate) return;

        if (cardData.card_script!=null)
        {            
            var scriptInstance = new Node();
            scriptInstance.SetScript(cardData.card_script);
            AddChild(scriptInstance);
            scriptInstance.Call("ApplyEffect", target); // Execute the effect then kill the card from the script                
        }
        else if (cardData.Effects != null)
        {
            foreach (var effect in cardData.Effects)
            {
                if (effect != null)
                {
                    bool effectFinished = await EffectExecution(effect, target);
                    if (!effectFinished) break; // Stop if effect fails
                }
            }

            await ToSignal(GetTree(), "process_frame"); // Ensure frame update before deletion
            KillCard();
        }        
    }

    private async Task<bool> EffectExecution(CardEffect effect, Node2D target)
    {
        if (effect == null) return false;
        
        GD.Print($"Executing effect: {effect}");
        bool result = effect.ApplyEffect(target);
        
        await ToSignal(GetTree(), "process_frame"); // Allow processing between effects
        return result;
    }

    private void UpdateGraphics()
    {
        if (cardData == null) return;

		costLbl.Text = cardData.Cost.ToString();
		nameLbl.Text = cardData.CardName;
		descriptionLbl.Text = cardData.Description.ToString();
        CardArt.Texture = cardData.CardArt;
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

    public void Shadering(Vector2 mousePos) {
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

    public async void BurnCard() 
    { 
        await AnimateAndDestroy(CardGlobal.GetBurnMaterial(), "card_dissolve_or_burn");
    }

    public async void KillCard() 
    { 
        await AnimateAndDestroy(CardGlobal.GetDissolveMaterial(), "card_dissolve_or_burn");
    }

    private async Task AnimateAndDestroy(ShaderMaterial material, string animationName)
    {
        _on_area_2d_mouse_exited();  // Ensure mouse exit state

        display.Material = material; animPlayer.SpeedScale = (float)GD.RandRange(0.95f, 1.0f);
        animPlayer.Play(animationName);
        await ToSignal(animPlayer, "animation_finished");

        obliterateCard(); 
    }

    public void obliterateCard() {       
        GlobalAccessPoint.GetCardManager().checkChange(this); 
        GetParent().RemoveChild(this);
        QueueFree();
    }


    public async Task FlipCard(bool flipUp=false)    {
        if (flipUp) animPlayer.PlayBackwards("card_flip"); 
        else animPlayer.Play("card_flip"); 
        await ToSignal(animPlayer, "animation_finished"); // Wait for the animation to end
    }

    public void TransformCard(Vector2 targetPosition, float targetRotation, float duration){
		if (cardTween!= null && cardTween.IsRunning())
		{
			cardTween.Kill();
			cardTween = null;
		}
		cardTween = GetTree().CreateTween().SetLoops(1);

        if (!Position.IsEqualApprox(targetPosition)) 
            cardTween.TweenProperty(this, "position", targetPosition, duration).SetEase(Tween.EaseType.Out);

        cardTween.TweenProperty(this, "rotation_degrees", targetRotation, duration).SetEase(Tween.EaseType.OutIn);

        cardTween.TweenCallback(Callable.From(() => 
        {
            cardTween = null;
        }));
    }

    public CardData GetCardData()    {
        return cardData;
    }
}