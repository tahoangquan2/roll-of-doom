using Godot;
using System;
using System.Dynamic;

public partial class Character : CardPlayZone //important that player alway the first to be add to the scene tree 
{
    [Export] public Stats baseStats;
    public Stats statInstance;

    protected TextureRect HealthBar, GuardBar, ShieldBar, ProgressBackground;
    protected Label HealthLabel, GuardLabel, ShieldLabel;
    private int currentDisplayHealth, currentDisplayGuard, currentDisplayShield;
    protected GridContainer BuffGrid;
	public Node2D visual=> GetNode<Node2D>("Visual");
    private AnimationPlayer animationPlayer => GetNode<AnimationPlayer>("AnimationPlayer");
    [Signal] public delegate void BuffUIClickedEventHandler(BuffUI buffUI);

    public override void _Ready()
    {
        base._Ready();
        statInstance = baseStats.CreateInstance();       
        GlobalVariables.allStats.Add(statInstance); 

        statInstance.BuffChanged += AddBuff;
		
        SetupStatVisuals();
        GlobalVariables.allCharacters.Add(this);    
        GlobalVariables.allCharacterStats.Add(statInstance,this);
    }

    protected virtual void SetupStatVisuals()
    {
        ProgressBackground = GetNode<TextureRect>("CharacterTab/StatTab/ProgressBackground");

        HealthBar = ProgressBackground.GetNode<TextureRect>("HBoxContainer/HealthBar");
        GuardBar = ProgressBackground.GetNode<TextureRect>("HBoxContainer/GuardBar");
        ShieldBar = ProgressBackground.GetNode<TextureRect>("HBoxContainer/ShieldBar");

        HealthLabel = GetNode<Label>("CharacterTab/StatTab/HealthLabel");
        GuardLabel = GetNode<Label>("CharacterTab/StatTab/Defends/GuardLabel");
        ShieldLabel = GetNode<Label>("CharacterTab/StatTab/Defends/ShieldLabel");

        BuffGrid = GetNode<GridContainer>("CharacterTab/GridContainer");

        currentDisplayGuard = -1;
        currentDisplayHealth = -1;
        currentDisplayShield = -1;

		if (statInstance.CharacterVisualScene != null){
			Node2D characterVisual = statInstance.CharacterVisualScene.Instantiate<Node2D>();
			visual.AddChild(characterVisual);
		}

        statInstance.StatChanged += UpdateStatsDisplay;

        statInstance.AttackAni += AttackAnimation;

    }

    public void AddBuff(BuffUI buffUI,bool alreadyExists)
    {
        if (!alreadyExists) {
            buffUI.Pressed+= () => EmitSignal(nameof(BuffUIClicked), buffUI);
            BuffGrid.AddChild(buffUI);
        }
    }

    public void UpdateStatsDisplay()
    {           
        if (statInstance.currentHealth<currentDisplayHealth) animationPlayer.Play("Hurt");
        float max = statInstance.maxHealth;
        float health = statInstance.currentHealth;
        float shield = statInstance.shield;
        float guard = statInstance.guard;
        float barSize = ProgressBackground.Size.X;

        float hpWidth,shieldWidth, guardWidth;

        if (health + shield + guard <= max)
        {
            hpWidth = barSize * (health / max);
            shieldWidth = barSize * (shield / max);
            guardWidth = barSize * (guard / max);
        }
        else
        {
            float remainder =  barSize ;
            float total = shield + guard+ health;
            hpWidth = barSize * (health / total);
            shieldWidth = remainder * (shield / total);
            guardWidth = remainder * (guard / total);
        }

        AnimateBarAndLabel( HealthBar, HealthLabel, currentDisplayHealth, health, hpWidth);
        AnimateBarAndLabel( ShieldBar, ShieldLabel, currentDisplayShield, shield, shieldWidth);
        AnimateBarAndLabel( GuardBar, GuardLabel, currentDisplayGuard, guard, guardWidth);

        currentDisplayGuard = statInstance.guard;
        currentDisplayHealth = statInstance.currentHealth;
        currentDisplayShield = statInstance.shield;

        if (statInstance.currentHealth <= 0) Die(); 
    }
    private void AnimateBarAndLabel(TextureRect bar, Label label, float fromVal, float toVal, float width)
    {
        if (fromVal == toVal && fromVal==0) return;
        Tween tween = CreateTween();
        bar.Visible = true;
        label.Visible = true;
        string suffix ="";

        tween.TweenProperty(bar, "custom_minimum_size", new Vector2(width, 0), 0.5f)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.Out);

        if (bar==HealthBar) suffix = $"/{statInstance.maxHealth}";
        

        VisualHelper.LabelTween(label, (int)fromVal, (int)toVal, 0.5f,"",suffix);

        tween.Chain().TweenCallback(Callable.From(() => {
            bool shouldHide = toVal <= 0;
            bar.Visible = !shouldHide;
            label.Visible = !shouldHide;
        }));        
    }

    public virtual void Cycle()
    {
        statInstance.Cycle();
    }

    public Stats GetStat() {
        return statInstance;
    }

    public async void Die()
    {
        statInstance.Die();
        GlobalVariables.allCharacters.Remove(this);             
        animationPlayer.Play("Die");
    }

    public async void AttackAnimation(Stats target)    {
        // tween position of the character to the target position
        Character targetCharacter = GlobalVariables.allCharacterStats[target];
        Vector2 startPos = visual.GlobalPosition;
        Vector2 attackPos = targetCharacter.visual.GlobalPosition + new Vector2(0, -50); // move up 50 pixels

        Tween tween = CreateTween();
        tween.TweenProperty(visual, "global_position", attackPos, 0.3f)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.Out);

        tween.Chain().TweenProperty(visual, "global_position", startPos, 0.3f)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.Out);
    }
}