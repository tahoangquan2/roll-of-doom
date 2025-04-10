using Godot;
using System;
using System.Dynamic;

public partial class Character : CardPlayZone
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
		
        SetupStatVisuals();
    }

    protected virtual void SetupStatVisuals()
    {
        ProgressBackground = GetNode<TextureRect>("CharacterTab/StatTab/ProgressBackground");

        HealthBar = ProgressBackground.GetNode<TextureRect>("HBoxContainer/HealthBar");
        GuardBar = ProgressBackground.GetNode<TextureRect>("HBoxContainer/GuardBar");
        ShieldBar = ProgressBackground.GetNode<TextureRect>("HBoxContainer/ShieldBar");

        HealthLabel = GetNode<Label>("CharacterTab/StatTab/HealthLabel");
        GuardLabel = GetNode<Label>("CharacterTab/StatTab/GuardLabel");
        ShieldLabel = GetNode<Label>("CharacterTab/StatTab/ShieldLabel");

        BuffGrid = GetNode<GridContainer>("CharacterTab/GridContainer");

        currentDisplayGuard = statInstance.guard;
        currentDisplayHealth = statInstance.currentHealth;
        currentDisplayShield = statInstance.shield;

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

    public void AddBuff(EnumGlobal.BuffType type, int value)
    {
        bool alreadyExists = statInstance.BuffExists(type);
        BuffUI buff = statInstance.ApplyBuff(type, value);

        if (!alreadyExists)
        {
            BuffGrid.AddChild(buff);
        }
        else
        {
            buff.AddValue(value);
        }
    }

    public async void UpdateStatsDisplay()
    {
        float max = statInstance.maxHealth;
        float health = statInstance.currentHealth;
        float shield = statInstance.shield;
        float guard = statInstance.guard;
        float barSize = ProgressBackground.Size.X;

        float hpWidth = barSize * (health / max);
        float shieldWidth, guardWidth;

        if (health + shield + guard <= max)
        {
            shieldWidth = barSize * (shield / max);
            guardWidth = barSize * (guard / max);
        }
        else
        {
            float remainder = Mathf.Max(0, barSize - hpWidth);
            float total = shield + guard;
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
        if (fromVal != toVal)
        {
            Tween tween = CreateTween();
            bar.Visible = true;
            label.Visible = true;

            tween.TweenProperty(bar, "custom_minimum_size", new Vector2(width, 0), 0.5f)
                .SetTrans(Tween.TransitionType.Sine)
                .SetEase(Tween.EaseType.Out);

            VisualHelper.LabelTween(label, (int)fromVal, (int)toVal, 0.5f,"",$"/{statInstance.maxHealth}");

            tween.Chain().TweenCallback(Callable.From(() => {
                bool shouldHide = toVal <= 0;
                bar.Visible = !shouldHide;
                label.Visible = !shouldHide;
            }));
        }
    }

    public virtual void Cycle()
    {
        statInstance.Cycle();
    }

    public Stats GetStat() {
        return statInstance;
    }
}
