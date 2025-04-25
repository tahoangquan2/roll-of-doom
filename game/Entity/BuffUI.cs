using Godot;

public partial class BuffUI : Button
{
	public EnumGlobal.BuffType Type;
	public EnumGlobal.BuffDuration Duration{ get; private set; }
	// Called when the node enters the scene tree for the first time.
	public int ValueX{ get; private set; } = 0;

	public BuffLogic _buffLogic{get; private set;} = null;

	private Label _buffValueLabel;

	public void SetBuff(EnumGlobal.BuffType type, int value)
	{
		Type = type;
		ValueX = value;
		
		_buffValueLabel = GetNode<Label>("Value");
		Icon = BuffDatabase.GetBuffData(type).icon;
		Duration = BuffDatabase.GetBuffData(type).Duration;
		_buffLogic = BuffDatabase.GetLogicForBuff(type);
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

	public int GetValue()	{
		return ValueX;
	}

	public void _on_pressed()
	{
		
	}
}