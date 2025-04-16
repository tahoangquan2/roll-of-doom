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
	private Node2D visual=> GetNode<Node2D>("Visual");

    public override void _Ready()
    {
        base._Ready();
        statInstance = baseStats.CreateInstance();       
        GlobalVariables.allStats.Add(statInstance); 

        statInstance.BuffChanged += AddBuff;
		
        SetupStatVisuals();
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

		foreach (BuffUI buff in statInstance.buffs.Values)
		{
			BuffGrid.AddChild(buff);
		}

        statInstance.StatChanged += UpdateStatsDisplay;
    }

    public void AddBuff(BuffUI buffUI,bool alreadyExists)
    {
        if (!alreadyExists) {
            BuffGrid.AddChild(buffUI);
        }
    }

    public void UpdateStatsDisplay()
    {
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
}