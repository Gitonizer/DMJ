[System.Serializable]
public class ObjectiveGoal
{
    public string Description;
    public ObjectiveType Type;
    public Items Item;
    public int CurrentQuantity;
    public int Quantity;
    public bool IsDone;

    public RewardType RewardType;
    public Items ItemReward;

    public ObjectiveGoal(ObjectiveType type)
    {
        Type = type;
    }
    public ObjectiveGoal(ObjectiveType type, int quantity)
    {
        Type = type;
        Quantity = quantity;
    }

    public bool Validate(Items item)
    {
        IsDone = item == Item;
        return item == Item;
    }
    public bool Validate(int quantity)
    {
        IsDone = quantity >= Quantity;
        return quantity >= Quantity;
    }

    public bool Validate()
    {
        CurrentQuantity++;
        IsDone = CurrentQuantity >= Quantity;
        return CurrentQuantity >= Quantity;
    }
}
