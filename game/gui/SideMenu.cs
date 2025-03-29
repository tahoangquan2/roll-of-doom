using Godot;
using System;

public partial class SideMenu : Control
{
	private AnimationPlayer animationPlayer;

	private Label CardTypeLabel;
	private Label CardNameLabel;
	private Label CardEffectLabel;
	private Label CardCostLabel;
	private TextureRect CardTextureRect;
	enum infoType
	{
		CardInfo,
		TowerInfo,
		ItemInfo
	}

	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		CardTypeLabel = GetNode<Label>("CardInfoPage/TypeLabel");
		CardNameLabel = GetNode<Label>("CardInfoPage/NameLabel");
		CardEffectLabel = GetNode<Label>("CardInfoPage/EffectLabel");
		CardCostLabel = GetNode<Label>("CardInfoPage/Cost/CostLabel");
		CardTextureRect = GetNode<TextureRect>("CardInfoPage/OutlineRect/TextureRect");
		
		CardManager cardManager = GetTree().CurrentScene.GetNodeOrNull<CardManager>("CardManager");

		if (cardManager != null)
        {
            // Connect signal dynamically
            cardManager.CardSelect += OnCardSelect;
        }
	}

	public bool toggle(bool button_pressed)
	{
		
		return button_pressed;
	}
	
	public void _on_info_toggle_toggled(bool button_pressed)
	{
		if (button_pressed)
		{
			animationPlayer.Play("Toggle");
		}
		else
		{
			animationPlayer.PlayBackwards("Toggle");
		}
	}

	private void OnCardSelect(CardData cardData)
	{
		//CardTypeLabel.Text = card.GetCardData();
		CardNameLabel.Text = cardData.CardName;
		CardEffectLabel.Text = "Effect: "+cardData.Description;
		CardCostLabel.Text = "Cost: "+cardData.Cost.ToString();
		CardTextureRect.Texture = cardData.CardArt;
	}

}
