using Godot;

public class PoisonedLogic : BuffLogic
{
    public override void OnCycle(Stats target, ref int value)
    {
        target.heal(-value); // Apply damage over time
    }
}
