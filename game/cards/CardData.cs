using Godot;
[Tool]
[GlobalClass]
public partial class CardData : Resource
{
    [Export] public string CardName { get; set; }
    [Export] public int Cost { get; set; }
    //[Export] public CardEffect Effect { get; set; }  // Now stores a reference to an effect
    //string for now
    [Export] public string Effect { get; set; }
        // Default Constructor
    public CardData()
    {
        CardName = "Default Card";
        Cost = 0;
        Effect = null;  // No effect by default
    }

    // Constructor
    public CardData(string cardName, int cost, string effect)
    {
        CardName = cardName;
        Cost = cost;
        Effect = effect;
    }
    // Save function (optional, for creating resources in code)
    public void Save(string path)
    {
        ResourceSaver.Save(this, path);
    }
}