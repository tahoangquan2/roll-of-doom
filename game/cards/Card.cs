using Godot;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public partial class Card : Node2D
{
    [Export] public CardData cardData { get; set; }
    public bool canBeHovered = true;
    public bool canActivate = true;
    public bool canBeMoved = true;
    [Signal] public delegate void CardHoveredEventHandler(Card card);
    [Signal] public delegate void CardUnhoveredEventHandler(Card card);
    public bool continueAfterEffect = false;
    private Label costLbl;
    private Label nameLbl;
    //private Label descriptionLbl;
    private Sprite2D CardTypeIcon;
    private Sprite2D CardManaIcon;
    private Sprite2D CardArt;
    private ShaderMaterial shaderMaterial;
    private Control display;
    private AnimationPlayer animPlayer;
    private CardManager parentManager;
    private Deck deck;
    private DiscardPile discardPile;
    private readonly float AngleMax = Mathf.DegToRad(7.0f);  
    public Tween cardTween=null;
    public Card()
    {
        cardData = new CardData(); // Ensure initialization
    }
    public void SetupCard(CardData cardData)
    {
        this.cardData = cardData.Duplicate() as CardData; 
        UpdateGraphics();
    }

    public override void _Ready()
    {
        var subViewport = GetNode<SubViewport>("SubViewport");
        
        costLbl = subViewport.GetNode<Label>("CostDisplay/CostLb");
        nameLbl = subViewport.GetNode<Label>("CardEffectLb");
        CardTypeIcon = subViewport.GetNode<Sprite2D>("CardTypeIcon");
        CardManaIcon = subViewport.GetNode<Sprite2D>("CostDisplay/ManaSlot");
        CardArt = subViewport.GetNode<Sprite2D>("CardDisplay/CardArt");
        
        var shaderDisplay = GetNode<Button>("Control/TextureRect"); // This will hold the shader

        shaderDisplay.Icon = subViewport.GetTexture();
        shaderDisplay.UseParentMaterial = false;
        display = GetNode<Control>("Control");
        animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");     
        
        if (shaderDisplay.Material is ShaderMaterial mat) shaderMaterial = mat;// Apply the shader material

        if (cardData.Keywords.Contains(EnumGlobal.CardKeywords.Auto)) canBeHovered = false; // Disable hover for auto cards

        parentManager = GetParent() as CardManager;
        parentManager.ConnectCardSignals(this);
        deck = GetTree().CurrentScene.GetNodeOrNull<Deck>(GlobalAccessPoint.deckPath);
        discardPile = GetTree().CurrentScene.GetNodeOrNull<DiscardPile>(GlobalAccessPoint.discardPilePath);

        UpdateGraphics();
    }

    public bool IsLayerNone()
    {
        return cardData.TargetMask == EnumGlobal.enumCardTargetLayer.None;
    }

    public async Task ActivateEffects(CardPlayZone target)
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
            foreach (var effectLayer in cardData.Effects)
            {
                if (effectLayer == null) continue;

                var tasks = new List<Task<bool>>();
                foreach (var effect in effectLayer.LayerEffects)                {
                    if (effect != null) tasks.Add(effect.ApplyEffect(target));
                }

                var results = await Task.WhenAll(tasks);

                if (results.Any(success => !success))
                {
                    GD.PrintErr("Effect Layer failed — interrupting.");
                    break;
                }
            }
            
            await ToSignal(GetTree(), "process_frame"); // Ensure frame update before deletion           
            EffectFinished();
        }        
    }

    private void UpdateGraphics()
    {
        if (cardData == null) return;

		costLbl.Text = cardData.Cost.ToString();
		nameLbl.Text = cardData.CardName;
        // a default texture for the card
        //if (cardData.CardArt == null) CardArt.Texture = CardGlobal.GetCardArtTextureDefault(); else 
        CardArt.Texture = cardData.CardArt;
        switch (cardData.CardType)
        {
            case EnumGlobal.enumCardType.Attack:
                CardTypeIcon.Frame = 2;                
                break;
            case EnumGlobal.enumCardType.Defense:
                CardTypeIcon.Frame = 1;
                break;
            case EnumGlobal.enumCardType.Spell:
                CardTypeIcon.Frame = 0;
                CardManaIcon.Frame = 1;
                break;
            default:
                CardTypeIcon.Frame = 0;
                CardManaIcon.Frame = 0;
                break;
        }
    }  
    public void Shadering(Vector2 mousePos) {
        if (shaderMaterial == null) return;

        Vector2 size = GlobalVariables.cardSize;

        float lerpValX = Mathf.Remap(mousePos.X, 0.0f, size.X, 0.0f, 1.0f)+0.5f;
        float lerpValY = Mathf.Remap(mousePos.Y, 0.0f, size.Y, 0.0f, 1.0f)+0.5f;

        float rotX =  Mathf.RadToDeg(Mathf.LerpAngle(-AngleMax, AngleMax, lerpValX));
        float rotY =  Mathf.RadToDeg(Mathf.LerpAngle(AngleMax, -AngleMax, lerpValY));

        shaderMaterial.SetShaderParameter("x_rot", rotY);
        shaderMaterial.SetShaderParameter("y_rot", rotX);
    }
    public void ResetShader(){
        if (shaderMaterial != null){
            shaderMaterial.SetShaderParameter("x_rot", 0f);shaderMaterial.SetShaderParameter("y_rot", 0f);}
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
    public void _on_texture_rect_pressed(){
        //GD.Print("Card right pressed");
        parentManager.EmitSignal(nameof(CardManager.CardSelect), cardData);
    }

    public async void BurnCard()    {   
        await AnimateAndDestroy(CardGlobal.GetBurnMaterial(), "card_dissolve_or_burn");
    }
    public async void KillCard()    {        
        await AnimateAndDestroy(CardGlobal.GetDissolveMaterial(), "card_dissolve_or_burn");
    }
    private async Task AnimateAndDestroy(ShaderMaterial material, string animationName)
    {   CardKeywordSystem.OnDiscardOrForget(this);  
        canBeHovered = false; 
        EmitSignal(nameof(CardUnhovered), this);

        display.Material = material; animPlayer.SpeedScale = (float)GD.RandRange(0.95f, 1.0f);
        animPlayer.Play(animationName);
        await ToSignal(animPlayer, "animation_finished");

        obliterateCard(); 
    }
   
    public void putToDiscardPile() { 
        canBeHovered = false;
        discardPile.AddCard(this);
    }
    public void EffectFinished() {  if (continueAfterEffect) return;
        //GD.Print("Card effect finished "+cardData.CardName + " "+this);
        CardKeywordSystem.OnPlay(this); // Call the keyword system
    }
    public void obliterateCard() {       
        //GD.Print("Card obliterate "+cardData.CardName + " "+this);
        parentManager.checkChange(this); 
        QueueFree();
    }
    public async Task FlipCard(bool flipUp=false)    {
        if (flipUp) animPlayer.PlayBackwards("card_flip"); 
        else animPlayer.Play("card_flip"); 
        await ToSignal(animPlayer, "animation_finished"); // Wait for the animation to end
    }
    public void TransformCard(Vector2 targetPosition, float targetRotation, float duration=0.15f){
		if (cardTween!= null && cardTween.IsRunning())
		{
			cardTween.Kill();
			cardTween = null;
		}
		cardTween = CreateTween().SetLoops(1);
        //canBeHovered = false; // Disable hover during animation

        if (!Position.IsEqualApprox(targetPosition)) 
            cardTween.TweenProperty(this, "position", targetPosition, duration)
            .SetTrans(Tween.TransitionType.Circ)
            .SetEase(Tween.EaseType.Out);

        cardTween.TweenProperty(this, "rotation_degrees", targetRotation, duration)
            .SetTrans(Tween.TransitionType.Back)
            .SetEase(Tween.EaseType.Out);

        cardTween.Finished += () => 
        {
            //canBeHovered = true; // Re-enable hover after animation
            cardTween.Kill();  // Stop animation (optional)
            cardTween = null;  // Remove reference
        };
    }
    public CardData GetCardData()    {
        return cardData;
    }
}