using Godot;

public partial class IntentUi : Button
{
	public EnumGlobal.IntentType intentType;
	public int value{get; private set;} = 0;

	private Label _intentValueLabel ;
	
	public void SetIntent(EnemyActionBase enemyAction)	{
		Icon = GlobalVariables.intentTextures[(int)enemyAction.intentType];
		
		intentType = enemyAction.intentType;
		value = enemyAction.value;
		
		//_intentValueLabel = GetNode<Label>("Value");
		UpdateValue(value);
	}

	public void UpdateValue(int value)
	{
		if (value == 0) {
			//_intentValueLabel.Visible = false;
			return;
		}
		//_intentValueLabel.Visible = true;
		this.value = value;
		//_intentValueLabel.Text = value.ToString();
	}
}
