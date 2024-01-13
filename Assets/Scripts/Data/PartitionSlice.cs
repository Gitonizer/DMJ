using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PartitionSlice
{
    public bool IsHorizontal;
    public int Value;

    public PartitionSlice(bool isHorizontal, int value)
    {
        IsHorizontal = isHorizontal;
        Value = value;
    }
}
