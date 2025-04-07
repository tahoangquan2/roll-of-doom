using Godot;
using System;

public partial class BuffUI : TextureRect
{
	public EnumGlobal.BuffType Type;
	public EnumGlobal.BuffDuration Duration{ get; private set; }
	// Called when the node enters the scene tree for the first time.
	public int ValueX{ get; private set; } = 0;

	private Label _buffValueLabel;

	public void SetBuff(EnumGlobal.BuffType type, int value)
	{
		Type = type;
		ValueX = value;
		
		_buffValueLabel = GetNode<Label>("Value");
		Texture = BuffDatabase.GetBuffData(type).icon;
		Duration = BuffDatabase.GetBuffData(type).Duration;
		UpdateValue(value);
	}

	public void UpdateValue(int value)
	{
		ValueX = value;
		_buffValueLabel.Text = ValueX.ToString();
	}

	public void AddValue(int value)
	{		
		ValueX += value;
		UpdateValue(ValueX);
	}
}