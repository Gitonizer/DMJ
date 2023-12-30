using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonNode
{
    public int Number;
    public Vector4 PartitionLimits { get { return _partitionLimits; } }
    public Vector4 RoomLimits { get { return _roomLimits; } }
    public List<DungeonNode> ChildNodes { get { return _childNodes; } }
    public int Layer { get { return _layer; } }
    public int NodeArea { get { return (int)((_partitionLimits.z - _partitionLimits.x) * (_partitionLimits.w - _partitionLimits.y)); } }
    public List<DungeonNode> RoomNodes { get { return _roomNodes; } }
    public bool IsConnected { get { return _isConnected; } }
    public bool HasRoom { get { return _hasRoom; } }

    public Vector2 RoomCenter { get { return new Vector2(Mathf.Lerp(RoomLimits.x, RoomLimits.z, 0.5f),
                                                Mathf.Lerp(RoomLimits.y, RoomLimits.w, 0.5f)); } }

    public int PartitionWith { get { return (int)(_partitionLimits.z - _partitionLimits.x); } }
    public int PartitionHeight { get { return (int)(_partitionLimits.w - _partitionLimits.y); } }

    private Vector4 _partitionLimits;
    private Vector4 _roomLimits;
    private List<DungeonNode> _childNodes;
    private List<DungeonNode> _roomNodes;

    private int _layer;
    private bool _hasRoom;

    private bool _isConnected;

    public DungeonNode(Vector4 limits, int layer, int number)
    {
        _partitionLimits = limits;
        _childNodes = new List<DungeonNode>();
        _layer = layer;
        _hasRoom = false;
        _isConnected = false;
        Number = number;
    }

    public void SetLimits(int minX, int minY, int maxX, int maxY)
    {
        _partitionLimits = new Vector4(minX, minY, maxX, maxY);
    }

    public void SetConnected()
    {
        _isConnected = true;
    }

    public void AddChildNode(DungeonNode node)
    {
        _childNodes.Add(node);
    }

    public void GenerateRoomLimits()
    {
        _hasRoom = true;

        int roomlimitMinX = Random.Range((int)Mathf.Lerp(_partitionLimits.x, _partitionLimits.z, 0.15f), (int)Mathf.Lerp(_partitionLimits.x, _partitionLimits.z, 0.2f));
        int roomlimitMaxX = Random.Range((int)Mathf.Lerp(_partitionLimits.x, _partitionLimits.z, 0.85f), (int)Mathf.Lerp(_partitionLimits.x, _partitionLimits.z, 0.9f));
        int roomlimitMinY = Random.Range((int)Mathf.Lerp(_partitionLimits.y, _partitionLimits.w, 0.15f), (int)Mathf.Lerp(_partitionLimits.y, _partitionLimits.w, 0.2f));
        int roomlimitMaxY = Random.Range((int)Mathf.Lerp(_partitionLimits.y, _partitionLimits.w, 0.85f), (int)Mathf.Lerp(_partitionLimits.y, _partitionLimits.w, 0.9f));

        _roomLimits = new Vector4(roomlimitMinX, roomlimitMinY, roomlimitMaxX, roomlimitMaxY);
    }
}
