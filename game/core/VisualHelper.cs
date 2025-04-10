using Godot;
using System;
using System.Threading.Tasks;
public partial class VisualHelper : Node
{
	// tween the label to a value
    public static async void LabelTween(Label label,int start, int end, float duration,string prefix="", string suffix="")
    {
		// Create tween
		Tween tween = label.CreateTween();

		tween.TweenMethod(Callable.From((float val) =>
		{
			label.Text = $"{prefix}{Mathf.RoundToInt(val)}{suffix}";
		}), start, end, duration)
		.SetTrans(Tween.TransitionType.Sine)
		.SetEase(Tween.EaseType.Out);

		await label.ToSignal(tween, "finished");   
    }
}
