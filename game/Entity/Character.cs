using Godot;
using System;

public partial class Character : CardPlayZone
{
    [Export] public Stats baseStats;
    public Stats statInstance;

    protected TextureRect HealthBar, GuardBar, ShieldBar, ProgressBackground;
    protected Label HealthLabel, GuardLabel, ShieldLabel;
    protected GridContainer BuffGrid;

    public override void _Ready()
    {
        base._Ready();
        statInstance = baseStats.CreateInstance();
        SetupStatVisuals();
        UpdateStatsDisplay();
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

    public void UpdateStatsDisplay()
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

        HealthBar.CustomMinimumSize = new Vector2(hpWidth, 0);
        ShieldBar.CustomMinimumSize = new Vector2(shieldWidth, 0);
        GuardBar.CustomMinimumSize = new Vector2(guardWidth, 0);

        HealthLabel.Text = $"{health}/{max}";
        GuardLabel.Text = $"{guard}";
        ShieldLabel.Text = $"{shield}";

        GuardBar.Visible = guard > 0;
        ShieldBar.Visible = shield > 0;
        GuardLabel.Visible = guard > 0;
        ShieldLabel.Visible = shield > 0;
    }

    public void CycleTurn()
    {
        statInstance.Cycle();
        UpdateStatsDisplay();
    }
}
