using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomConnection
{
    public DungeonNode ConnectedNode { get { return _connectedNode; } }
    public Vector2 Coordinates { get { return _coordinates; } }
    public bool IsConnected { get { return _isConnected; } }

    private DungeonNode _connectedNode;
    private Vector2 _coordinates;
    private bool _isConnected;

    public RoomConnection(DungeonNode connectedNode, Vector2 coordinates, bool isConnected)
    {
        _connectedNode = connectedNode;
        _coordinates = coordinates;
        _isConnected = isConnected;
    }
}
