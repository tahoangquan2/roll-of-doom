using Godot;

public static class ScriptExecutor
{
    public static void ExecuteScript(Node context, string script)
    {
        if (string.IsNullOrEmpty(script))
        {
            GD.Print("No script to execute.");
            return;
        }

        GD.Print($"Executing script: {script}");

        // Use Godot's Expression to evaluate simple scripts
        var expression = new Expression();
        Error error = expression.Parse(script, new string[] { "hand", "deck" });

        if (error != Error.Ok)
        {
            GD.PrintErr($"Script error: {error}");
            return;
        }

        // Get references to relevant objects
        Hand hand = context.GetNode<Hand>("../Hand");  // Adjust the path as necessary
        Deck deck = context.GetNode<Deck>("../Deck");

        // Execute script with provided variables
        //expression.Execute(new Variant[] { hand, deck }, null);
        
    }
}
