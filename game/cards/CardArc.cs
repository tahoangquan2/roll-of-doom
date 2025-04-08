using System.Collections.Generic;
using Godot;

public partial class CardArc : Line2D
{

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Ready()
	{
		SetProcess(false);
	}
	
	public override void _Process(double delta)
	{
		if (snapedToZone) return;
		Points = GetPoint(GetGlobalMousePosition()).ToArray();
	}

	public void ShowArc(){
		SetProcess(true);
		Visible = true;
	}

	public void HideArc(){
		Visible = false;
		snapedToZone = false;
		SetProcess(false);
	}

	private List<Vector2> GetPoint(Vector2 end)
	{
		List<Vector2> points = new List<Vector2>();
		Vector2 start = new Vector2(0, 0);
		end = end - GlobalPosition;

		for (int i = 0; i < 8; i++)
		{
			float t = i / 7.0f;

			// Eased linear X and Y
			float x = Mathf.Lerp(start.X, end.X, t);
			float y = Mathf.Lerp(start.Y, end.Y, t);

			// Arc offset peaking at t = 0.5, zero at t=0 and t=1
			float arcHeight = -50f * Mathf.Sin(t * Mathf.Pi);

			points.Add(new Vector2(x, y + arcHeight));
		}

		return points;
	}

	public void SnapToZone(CardPlayZone zone=null)
	{
		//GD.Print("SnapToZone: "+zone);
		if (zone == null) {
			snapedToZone = false;
			return;
		}
		Vector2 zonePos = zone.GlobalPosition;		

		Points = GetPoint(zonePos).ToArray();
		snapedToZone = true;
	}

	private bool snapedToZone = false;

}
