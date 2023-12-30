using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonCell : MonoBehaviour
{
    public DungeonCellType DungeonCellType;
    public Direction Direction;
    public int RoomNumber;
    public int NodeNumber;
    public int PathNumber;
    public Vector2 GridPosition;

    private void Start()
    {
        RoomNumber = -1;
        NodeNumber = -1;
        PathNumber = -1;
    }
}
