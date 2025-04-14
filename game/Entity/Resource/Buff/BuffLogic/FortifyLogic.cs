public class FortifyLogic : BuffLogic
{
	public override void OnDefend(Stats target, ref int number, ref int value)
	{
		number += value;
	}
}
