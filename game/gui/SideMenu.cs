using Godot;
using System;
using System.Collections.Generic;

public partial class SideMenu : Control
{
	private AnimationPlayer animationPlayer;

	private Label CardTypeLabel;
	private Label CardNameLabel;
	private Label CardEffectLabel;
	private Label CardCostLabel;
	private TextureRect CardTextureRect;

	private List<Control> controlOfCards = new();

	private Dictionary<string, string> keywordDescriptions = new();
	private Dictionary<string, string> buffDescriptions = new();

	private VBoxContainer CardKeywordsContainer;
	enum infoType	{
		CardInfo,BuffInfo,ItemInfo
	}

	PackedScene basicLabelScene = GD.Load<PackedScene>("res://game/gui/side_menu_label.tscn");

	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		CardTypeLabel = GetNode<Label>("ScrollContainer/CardInfoPage/TypeLabel");
		CardNameLabel = GetNode<Label>("ScrollContainer/CardInfoPage/NameLabel");
		CardEffectLabel = GetNode<Label>("ScrollContainer/CardInfoPage/EffectLabel");
		CardCostLabel = GetNode<Label>("ScrollContainer/CardInfoPage/Cost/CostLabel");
		CardTextureRect = GetNode<TextureRect>("ScrollContainer/CardInfoPage/OutlineRect/TextureRect");
		CardKeywordsContainer = GetNode<VBoxContainer>("ScrollContainer/CardInfoPage/Keywords");

		Node container = CardKeywordsContainer.GetParent();

		for (int i=1;i<=4;i++){ //add 2nd to 5th child
			Control control = container.GetChild(i) as Control;
			if (control != null) controlOfCards.Add(control);
		}
		
		CardManager cardManager = GetTree().CurrentScene.GetNodeOrNull<CardManager>("CardManager");

		if (cardManager != null) cardManager.CardSelect += OnCardSelect;

		foreach (Character character in GlobalVariables.allCharacters)
			character.BuffUIClicked += OnBuffUIClicked;

		LoadDescriptions();
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

	private void OnCardSelect(CardData cardData)	{
		foreach (Control control in controlOfCards) control.Visible = true;
		CardTypeLabel.Text = cardData.CardType.ToString()+" Card";
		CardNameLabel.Text = cardData.CardName;
		CardEffectLabel.Text = "Effect: "+cardData.Description;
		CardCostLabel.Text = "Cost: "+cardData.Cost.ToString();
		CardTextureRect.Texture = cardData.CardArt;

		cleanKeywords();

		foreach (var keyword in cardData.Keywords)
		{
			string name = keyword.ToString();
			if (keywordDescriptions.TryGetValue(name, out var desc))
			AddDescriptionLabel(name, desc);
		}
	}

	private void OnBuffUIClicked(BuffUI buffUI)
	{
		foreach (Control control in controlOfCards) control.Visible = false;

		string buffName = buffUI.Type.ToString();
		string value = buffUI.GetValue().ToString();

		CardTypeLabel.Text = $"{buffName} x {value}";
		cleanKeywords();

		if (buffDescriptions.TryGetValue(buffName, out var desc))
			AddDescriptionLabel(buffName, desc.Replace("X", value));

		string durationKey = buffUI.Duration.ToString();
		if (buffDescriptions.TryGetValue(durationKey, out var durationDesc))
			AddDescriptionLabel(durationKey, durationDesc);
	}


	private void AddDescriptionLabel(string title, string description)
	{
		if (string.IsNullOrEmpty(description)) return;

		Label label = basicLabelScene.Instantiate<Label>();
		label.Text = $"{title}: {description}";
		CardKeywordsContainer.AddChild(label);
	}


	private void cleanKeywords() 	{
		foreach (Node child in CardKeywordsContainer.GetChildren())
			child.QueueFree();
	}

	private void LoadDescriptions()
	{
		string path = "res://assets/description/descriptions.json";
		if (!FileAccess.FileExists(path)) return;

		using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
		string content = file.GetAsText();

		var json = new Json();
		var parseResult = json.Parse(content);

		if (parseResult != Error.Ok) return;

		var root = json.Data.As<Godot.Collections.Dictionary>();
		if (root == null) return;

		if (root.ContainsKey("Keywords") && root["Keywords"].VariantType == Variant.Type.Dictionary)
		{	var kwDict = root["Keywords"].As<Godot.Collections.Dictionary>();
			foreach (var entry in kwDict)
				keywordDescriptions[entry.Key.ToString()] = entry.Value.ToString();
		}

		if (root.ContainsKey("Buffs") && root["Buffs"].VariantType == Variant.Type.Dictionary)
		{	var buffDict = root["Buffs"].As<Godot.Collections.Dictionary>();
			foreach (var entry in buffDict)
				buffDescriptions[entry.Key.ToString()] = entry.Value.ToString();
		}
	}
}
