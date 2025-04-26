using System.Collections.Generic;
using Godot;

public partial class IntentUi : Button
{
	public EnumGlobal.IntentType intentType;

	private Label _intentValueLabel ;

	public EnemyActionBase enemyAction;
	
	public void SetIntent(EnemyActionBase enemyAction)	{
		Icon = GlobalVariables.intentTextures[(int)enemyAction.intentType];
		
		intentType = enemyAction.intentType;
		this.enemyAction = enemyAction;
		
		_intentValueLabel = GetNode<Label>("Value");
		UpdateValue(enemyAction.Values);
	}

	public void UpdateValue(List<int> values)
	{
		if (values.Count == 0) {
			_intentValueLabel.Visible = false;
			return;
		}
		_intentValueLabel.Visible = true;
		string valueText = "";
		for (int i = 0; i < values.Count; i++)
		{
			if (i > 0) valueText += " , ";
			valueText += values[i].ToString();
		}
		_intentValueLabel.Text = valueText;
	}
}
