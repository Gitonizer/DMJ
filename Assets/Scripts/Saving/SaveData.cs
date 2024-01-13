using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public int Level;
    public List<PartitionSlice> PartitionSlices;
    public int CurrentHealth;
}
