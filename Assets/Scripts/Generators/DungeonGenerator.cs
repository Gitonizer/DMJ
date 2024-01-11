using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityRandom = UnityEngine.Random;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Dungeon Cell Prefab")]
    public DungeonCell DungeonCellPrefab;

    [Header("Ground prefab")]
    public GameObject GroundPrefab;

    [Header("Path prefab")]
    public GameObject NorthSouthPathPrefab;
    public GameObject EastWestPathPrefab;

    [Header("Wall prefabs")]
    public GameObject WestWallPrefab;
    public GameObject EastWallPrefab;
    public GameObject NorthWallPrefab;
    public GameObject SouthWallPrefab;

    [Header("Corner prefabs")]
    public GameObject NorthWestWallPrefab;
    public GameObject NorthEastWallPrefab;
    public GameObject SouthWestWallPrefab;
    public GameObject SouthEastWallPrefab;

    [Header("Door prefabs")]
    public GameObject DoorPrefab;

    [Header("Character Parents")]
    public GameObject CharacterParent;
    public GameObject EnemyParent;
    public GameObject NPCParent;

    [Header("Items Parent")]
    public GameObject ItemsParent;

    [Header("Exit Door")]
    public Door ExitDoor;

    private DungeonCell[,] _dungeonCells;
    private float _cellSize;
    private int _minPartitionArea;
    private Vector2 _minPartitionSides;

    private List<DungeonNode> _dungeonNodes;
    private List<DungeonNode> _roomNodes;

    private List<DungeonNode> _leftLeafRooms;
    private List<DungeonNode> _rightLeafRooms;

    private DungeonNode _entranceRoom;
    private DungeonNode _exitRoom;
    private DungeonNode _middleRoom;

    private Character _player;
    private EnemyManager _enemyManager;
    private NPCManager _npcManager;
    private ItemsManager _itemsManager;

    private QuestScriptable _quest;

    private void Awake()
    {
        _dungeonCells = new DungeonCell[20, 20]; // this size could be set by Quest
        _cellSize = 16f;
        _minPartitionArea = 60;
        _minPartitionSides = new Vector2(6, 6);

        _dungeonNodes = new List<DungeonNode>();
        _roomNodes = new List<DungeonNode>();

        _leftLeafRooms = new List<DungeonNode>();
        _rightLeafRooms = new List<DungeonNode>();

        _player = CharacterParent.GetComponentInChildren<Character>();
        _enemyManager = EnemyParent.GetComponent<EnemyManager>();
        _npcManager = NPCParent.GetComponent<NPCManager>();

        _itemsManager = ItemsParent.GetComponent<ItemsManager>();
    }

    public void Initialize(QuestScriptable quest)
    {
        _quest = quest;
        InitializeCells(); //create cells on map
        StartCoroutine(GenerateBSP(0)); //instantiate prefabs after deciding all celltypes
    }

    private void InitializeCells()
    {
        for (int y = 0; y < _dungeonCells.GetLength(1); y++)
        {
            for (int x = 0; x < _dungeonCells.GetLength(0); x++)
            {
                _dungeonCells[x, y] = Instantiate(DungeonCellPrefab, new Vector3(x * _cellSize, 0, y * _cellSize), Quaternion.identity, transform);
                _dungeonCells[x, y].DungeonCellType = DungeonCellType.Empty;
                _dungeonCells[x, y].GridPosition = new Vector2(x, y);
            }
        }
    }

    private void InstantiatePrefabs()
    {
        for (int y = 0; y < _dungeonCells.GetLength(1); y++)
        {
            for (int x = 0; x < _dungeonCells.GetLength(0); x++)
            {
                switch (_dungeonCells[x, y].DungeonCellType)
                {
                    case DungeonCellType.Ground:
                        Instantiate(GroundPrefab, _dungeonCells[x, y].gameObject.transform);
                        break;
                    case DungeonCellType.NorthSouthPath:
                        Instantiate(NorthSouthPathPrefab, _dungeonCells[x, y].gameObject.transform);
                        break;
                    case DungeonCellType.EastWestPath:
                        Instantiate(EastWestPathPrefab, _dungeonCells[x, y].gameObject.transform);
                        break;
                    case DungeonCellType.WestWall:
                        Instantiate(WestWallPrefab, _dungeonCells[x, y].gameObject.transform);
                        break;
                    case DungeonCellType.EastWall:
                        Instantiate(EastWallPrefab, _dungeonCells[x, y].gameObject.transform);
                        break;
                    case DungeonCellType.NorthWall:
                        Instantiate(NorthWallPrefab, _dungeonCells[x, y].gameObject.transform);
                        break;
                    case DungeonCellType.SouthWall:
                        Instantiate(SouthWallPrefab, _dungeonCells[x, y].gameObject.transform);
                        break;
                    case DungeonCellType.NorthWestWall:
                        Instantiate(NorthWestWallPrefab, _dungeonCells[x, y].gameObject.transform);
                        break;
                    case DungeonCellType.NorthEastWall:
                        Instantiate(NorthEastWallPrefab, _dungeonCells[x, y].gameObject.transform);
                        break;
                    case DungeonCellType.SouthWestWall:
                        Instantiate(SouthWestWallPrefab, _dungeonCells[x, y].gameObject.transform);
                        break;
                    case DungeonCellType.SouthEastWall:
                        Instantiate(SouthEastWallPrefab, _dungeonCells[x, y].gameObject.transform);
                        break;
                    case DungeonCellType.Empty:
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private IEnumerator GenerateBSP(int nodeNumber)
    {
        bool createdNewNodes = false;
        List<DungeonNode> tempNodes = new List<DungeonNode>();
        // check node list size
        // if 0, create node with map size
        if (_dungeonNodes.Count == 0)
        {
            _dungeonNodes.Add(new DungeonNode(new Vector4(0, 0, _dungeonCells.GetLength(0), _dungeonCells.GetLength(1)), 0, nodeNumber));
        }

        // check all nodes on the latest layer
        // for each node that is big enough, pick a random direction to cut and pick a random spot to cut
        // create nodes and assign them to parent node (layer equals parent layer+1)
        // have nodes create their areas
        foreach (var node in _dungeonNodes)
        {
            yield return null;

            if (node.NodeArea > _minPartitionArea && node.PartitionWith > _minPartitionSides.x && node.PartitionHeight > _minPartitionSides.y && node.ChildNodes.Count == 0)
            {
                createdNewNodes = true;

                bool horizontalCut = UnityRandom.Range(1, 101) > 50;

                if (horizontalCut)
                {
                    int randomY;

                    if (node.PartitionHeight > 8) //make sure we don't cut partitions too small
                    {
                        //find random y
                        int minrandomY = (int)Mathf.Lerp(node.PartitionLimits.y, node.PartitionLimits.w, 0.3f);
                        int maxrandomY = (int)Mathf.Lerp(node.PartitionLimits.y, node.PartitionLimits.w, 0.7f);

                        randomY = UnityRandom.Range(minrandomY, maxrandomY);
                    }
                    else
                    {
                        randomY = (int)Mathf.Lerp(node.PartitionLimits.y, node.PartitionLimits.w, 0.5f);
                    }

                    // create nodes
                    nodeNumber++;
                    Vector4 node1limits = new Vector4(node.PartitionLimits.x, node.PartitionLimits.y, node.PartitionLimits.z, randomY);
                    DungeonNode node1 = new DungeonNode(node1limits, node.Layer + 1, nodeNumber);

                    nodeNumber++;
                    Vector4 node2limits = new Vector4(node.PartitionLimits.x, randomY, node.PartitionLimits.z, node.PartitionLimits.w);
                    DungeonNode node2 = new DungeonNode(node2limits, node.Layer + 1, nodeNumber);

                    node.AddChildNode(node1);
                    node.AddChildNode(node2);

                    tempNodes.Add(node1);
                    tempNodes.Add(node2);
                }
                else
                {
                    int randomX;

                    if (node.PartitionWith > 8) //make sure we don't cut partitions too small
                    {
                        //find random x
                        int minrandomX = (int)Mathf.Lerp(node.PartitionLimits.x, node.PartitionLimits.z, 0.3f);
                        int maxrandomX = (int)Mathf.Lerp(node.PartitionLimits.x, node.PartitionLimits.z, 0.7f);

                        randomX = UnityRandom.Range(minrandomX, maxrandomX);
                    }
                    else
                    {
                        randomX = (int)Mathf.Lerp(node.PartitionLimits.x, node.PartitionLimits.z, 0.5f);
                    }

                    // create nodes
                    nodeNumber++;
                    Vector4 node1limits = new Vector4(node.PartitionLimits.x, node.PartitionLimits.y, randomX, node.PartitionLimits.w);
                    DungeonNode node1 = new DungeonNode(node1limits, node.Layer + 1, nodeNumber);

                    nodeNumber++;
                    Vector4 node2limits = new Vector4(randomX, node.PartitionLimits.y, node.PartitionLimits.z, node.PartitionLimits.w);
                    DungeonNode node2 = new DungeonNode(node2limits, node.Layer + 1, nodeNumber);

                    node.AddChildNode(node1);
                    node.AddChildNode(node2);

                    tempNodes.Add(node1);
                    tempNodes.Add(node2);
                }
            }
        }

        // if no nodes are created, stop recursivity
        if (!createdNewNodes)
        {
            GenerateNodeRooms(); 
            GeneratePaths(); // validating generated map here
            InstantiatePrefabs();
            SetEntranceAndExitRooms();
            InstantiateDoors(_quest);
            yield return StartCoroutine(PlacePlayer());
            yield return StartCoroutine(PlaceEnemies());
            yield return StartCoroutine(PlaceNPCs());
            yield return StartCoroutine(PlaceItems());

            EventManager.OnLevelLoaded?.Invoke();

            yield break;
        }

        _dungeonNodes.AddRange(tempNodes);

        // repeat until no node can be cut (recursive)
        StartCoroutine(GenerateBSP(nodeNumber));
    }

    private void GenerateNodeRooms()
    {
        for (int i = 0; i < _dungeonNodes.Count; i++)
        {
            if (_dungeonNodes[i].ChildNodes.Count == 0)
            {
                _dungeonNodes[i].GenerateRoomLimits();
                _roomNodes.Add(_dungeonNodes[i]);

                for (int y = (int)_dungeonNodes[i].RoomLimits.y; y < (int)_dungeonNodes[i].RoomLimits.w; y++)
                {
                    for (int x = (int)_dungeonNodes[i].RoomLimits.x; x < (int)_dungeonNodes[i].RoomLimits.z; x++)
                    {
                        if (y == (int)_dungeonNodes[i].RoomLimits.y) //top
                        {
                            if (x == (int)_dungeonNodes[i].RoomLimits.x) //top right corner
                            {
                                _dungeonCells[x, y].DungeonCellType = DungeonCellType.NorthEastWall;
                            }
                            else if (x == (int)_dungeonNodes[i].RoomLimits.z - 1) //top left corner
                            {
                                _dungeonCells[x, y].DungeonCellType = DungeonCellType.NorthWestWall;
                            }
                            else //top wall
                            {
                                _dungeonCells[x, y].DungeonCellType = DungeonCellType.NorthWall;
                            }
                        }
                        else if (y == (int)_dungeonNodes[i].RoomLimits.w - 1) //bot
                        {
                            if (x == (int)_dungeonNodes[i].RoomLimits.x) //bot right corner
                            {
                                _dungeonCells[x, y].DungeonCellType = DungeonCellType.SouthEastWall;
                            }
                            else if (x == (int)_dungeonNodes[i].RoomLimits.z - 1) //bot left corner
                            {
                                _dungeonCells[x, y].DungeonCellType = DungeonCellType.SouthWestWall;
                            }
                            else //bot wall
                            {
                                _dungeonCells[x, y].DungeonCellType = DungeonCellType.SouthWall;
                            }
                        }
                        else if (x == (int)_dungeonNodes[i].RoomLimits.x && y != (int)_dungeonNodes[i].RoomLimits.y && y != (int)_dungeonNodes[i].RoomLimits.w - 1) //left wall
                        {
                            _dungeonCells[x, y].DungeonCellType = DungeonCellType.EastWall;
                        }
                        else if (x == (int)_dungeonNodes[i].RoomLimits.z - 1 && y != (int)_dungeonNodes[i].RoomLimits.y && y != (int)_dungeonNodes[i].RoomLimits.w - 1) //right wall
                        {
                            _dungeonCells[x, y].DungeonCellType = DungeonCellType.WestWall;
                        }
                        else
                        {
                            _dungeonCells[x, y].DungeonCellType = DungeonCellType.Ground;
                        }

                        _dungeonCells[x, y].RoomNumber = _dungeonNodes[i].Number;
                        _dungeonCells[x, y].NodeNumber = i;
                    }
                }
            }
        }
    }

    private void GeneratePaths()
    {
        int highestLayer = -1;

        foreach (var roomNode in _roomNodes)
        {
            highestLayer = roomNode.Layer > highestLayer ? roomNode.Layer : highestLayer;
        }

        //start with higher layer rooms
        //find all rooms with previous layer
        //connect child rooms to each other
        GeneratePath(highestLayer, 0);
    }

    private void GeneratePath(int layer, int pathnumber)
    {
        if (layer <= 0)
            return;

        List<DungeonNode> parentNodes = new List<DungeonNode>();

        for (int i = 0; i < _dungeonNodes.Count; i++) // get nodes so we can connect their children
        {
            if (_dungeonNodes[i].Layer == layer - 1 && _dungeonNodes[i].ChildNodes.Count != 0)
            {
                parentNodes.Add(_dungeonNodes[i]);
            }
        }

        //go through parent nodes and connect their children
        foreach (var node in parentNodes)
        {
            //get leaf rooms
            List<DungeonNode> leftRoomNodes = GetNodeChildRooms(node.ChildNodes[0]);
            List<DungeonNode> rightRoomNodes = GetNodeChildRooms(node.ChildNodes[1]);

            //get closest rooms
            List<DungeonNode> roomPair = GetClosestRooms(leftRoomNodes, rightRoomNodes);

            //Entrance coordinates
            List<Vector2> entranceCoordinates = new List<Vector2>();

            Vector2 vectorbetweenchildren = roomPair[0].RoomCenter - roomPair[1].RoomCenter;

            bool pathSuccess = false;

            if (Mathf.Abs(vectorbetweenchildren.x) > Mathf.Abs(vectorbetweenchildren.y)) // connect with east/west
            {
                if (vectorbetweenchildren.x > 0) // connect with child1 east - child0 west
                {
                    int miny = roomPair[0].RoomLimits.y > roomPair[1].RoomLimits.y ? (int)roomPair[0].RoomLimits.y : (int)roomPair[1].RoomLimits.y;
                    int maxy = roomPair[0].RoomLimits.w < roomPair[1].RoomLimits.w ? (int)roomPair[0].RoomLimits.w : (int)roomPair[1].RoomLimits.w;

                    int randomy = UnityRandom.Range(miny, maxy);

                    entranceCoordinates.Add(new Vector2((int)roomPair[0].RoomLimits.z - 1, randomy));
                    entranceCoordinates.Add(new Vector2((int)roomPair[1].RoomLimits.x, randomy));

                    //room entrances
                    if (_dungeonCells[(int)roomPair[0].RoomLimits.z - 1, randomy].DungeonCellType == DungeonCellType.NorthWestWall)
                    {
                        _dungeonCells[(int)roomPair[0].RoomLimits.z - 1, randomy].DungeonCellType = DungeonCellType.NorthWall;
                    }
                    else if (_dungeonCells[(int)roomPair[0].RoomLimits.z - 1, randomy].DungeonCellType == DungeonCellType.SouthWestWall)
                    {
                        _dungeonCells[(int)roomPair[0].RoomLimits.z - 1, randomy].DungeonCellType = DungeonCellType.SouthWall;
                    }
                    else if (_dungeonCells[(int)roomPair[0].RoomLimits.z - 1, randomy].DungeonCellType == DungeonCellType.WestWall)
                    {
                        _dungeonCells[(int)roomPair[0].RoomLimits.z - 1, randomy].DungeonCellType = DungeonCellType.Ground;
                    }

                    if (_dungeonCells[(int)roomPair[1].RoomLimits.x, randomy].DungeonCellType == DungeonCellType.NorthEastWall)
                    {
                        _dungeonCells[(int)roomPair[1].RoomLimits.x, randomy].DungeonCellType = DungeonCellType.NorthWall;
                    }
                    else if (_dungeonCells[(int)roomPair[1].RoomLimits.x, randomy].DungeonCellType == DungeonCellType.SouthEastWall)
                    {
                        _dungeonCells[(int)roomPair[1].RoomLimits.x, randomy].DungeonCellType = DungeonCellType.SouthWall;
                    }
                    else if (_dungeonCells[(int)roomPair[1].RoomLimits.x, randomy].DungeonCellType == DungeonCellType.EastWall)
                    {
                        _dungeonCells[(int)roomPair[1].RoomLimits.x, randomy].DungeonCellType = DungeonCellType.Ground;
                    }

                    //path
                    for (int x = (int)roomPair[1].RoomLimits.z; x < roomPair[0].RoomLimits.x; x++)
                    {
                        pathSuccess = true;

                        _dungeonCells[x, randomy].DungeonCellType = DungeonCellType.EastWestPath;
                        _dungeonCells[x, randomy].Direction = Direction.Left;
                        _dungeonCells[x, randomy].PathNumber = pathnumber;
                    }
                }
                else // connect with child0 east - child1 west
                {
                    int miny = roomPair[0].RoomLimits.y > roomPair[1].RoomLimits.y ? (int)roomPair[0].RoomLimits.y : (int)roomPair[1].RoomLimits.y;
                    int maxy = roomPair[0].RoomLimits.w < roomPair[1].RoomLimits.w ? (int)roomPair[0].RoomLimits.w : (int)roomPair[1].RoomLimits.w;

                    int randomy = UnityRandom.Range(miny, maxy);

                    entranceCoordinates.Add(new Vector2((int)roomPair[0].RoomLimits.z - 1, randomy));
                    entranceCoordinates.Add(new Vector2((int)roomPair[1].RoomLimits.x, randomy));

                    //room entrances
                    if (_dungeonCells[(int)roomPair[0].RoomLimits.z - 1, randomy].DungeonCellType == DungeonCellType.NorthWestWall)
                    {
                        _dungeonCells[(int)roomPair[0].RoomLimits.z - 1, randomy].DungeonCellType = DungeonCellType.NorthWall;
                    }
                    else if (_dungeonCells[(int)roomPair[0].RoomLimits.z - 1, randomy].DungeonCellType == DungeonCellType.SouthWestWall)
                    {
                        _dungeonCells[(int)roomPair[0].RoomLimits.z - 1, randomy].DungeonCellType = DungeonCellType.SouthWall;
                    }
                    else if (_dungeonCells[(int)roomPair[0].RoomLimits.z - 1, randomy].DungeonCellType == DungeonCellType.WestWall)
                    {
                        _dungeonCells[(int)roomPair[0].RoomLimits.z - 1, randomy].DungeonCellType = DungeonCellType.Ground;
                    }

                    if (_dungeonCells[(int)roomPair[1].RoomLimits.x, randomy].DungeonCellType == DungeonCellType.NorthEastWall)
                    {
                        _dungeonCells[(int)roomPair[1].RoomLimits.x, randomy].DungeonCellType = DungeonCellType.NorthWall;
                    }
                    else if (_dungeonCells[(int)roomPair[1].RoomLimits.x, randomy].DungeonCellType == DungeonCellType.SouthEastWall)
                    {
                        _dungeonCells[(int)roomPair[1].RoomLimits.x, randomy].DungeonCellType = DungeonCellType.SouthWall;
                    }
                    else if (_dungeonCells[(int)roomPair[1].RoomLimits.x, randomy].DungeonCellType == DungeonCellType.EastWall)
                    {
                        _dungeonCells[(int)roomPair[1].RoomLimits.x, randomy].DungeonCellType = DungeonCellType.Ground;
                    }

                    //path
                    for (int x = (int)roomPair[0].RoomLimits.z; x < roomPair[1].RoomLimits.x; x++)
                    {
                        pathSuccess = true;

                        _dungeonCells[x, randomy].DungeonCellType = DungeonCellType.EastWestPath;
                        _dungeonCells[x, randomy].Direction = Direction.Right;
                        _dungeonCells[x, randomy].PathNumber = pathnumber;
                    }
                }
            }
            else // connect with north/south
            {
                if (vectorbetweenchildren.y > 0) // connect with child1 south - child0 north
                {
                    int minx = roomPair[0].RoomLimits.x > roomPair[1].RoomLimits.x ? (int)roomPair[0].RoomLimits.x : (int)roomPair[1].RoomLimits.x;
                    int maxx = roomPair[0].RoomLimits.z < roomPair[1].RoomLimits.z ? (int)roomPair[0].RoomLimits.z : (int)roomPair[1].RoomLimits.z;

                    int randomx = UnityRandom.Range(minx, maxx);

                    entranceCoordinates.Add(new Vector2(randomx, (int)roomPair[0].RoomLimits.w - 1));
                    entranceCoordinates.Add(new Vector2(randomx, (int)roomPair[1].RoomLimits.y));

                    //room entrances
                    if (_dungeonCells[randomx, (int)roomPair[0].RoomLimits.w - 1].DungeonCellType == DungeonCellType.SouthEastWall)
                    {
                        _dungeonCells[randomx, (int)roomPair[0].RoomLimits.w - 1].DungeonCellType = DungeonCellType.EastWall;
                    }
                    else if (_dungeonCells[randomx, (int)roomPair[0].RoomLimits.w - 1].DungeonCellType == DungeonCellType.SouthWestWall)
                    {
                        _dungeonCells[randomx, (int)roomPair[0].RoomLimits.w - 1].DungeonCellType = DungeonCellType.WestWall;
                    }
                    else if (_dungeonCells[randomx, (int)roomPair[0].RoomLimits.w - 1].DungeonCellType == DungeonCellType.SouthWall)
                    {
                        _dungeonCells[randomx, (int)roomPair[0].RoomLimits.w - 1].DungeonCellType = DungeonCellType.Ground;
                    }

                    if (_dungeonCells[randomx, (int)roomPair[1].RoomLimits.y].DungeonCellType == DungeonCellType.NorthEastWall)
                    {
                        _dungeonCells[randomx, (int)roomPair[1].RoomLimits.y].DungeonCellType = DungeonCellType.EastWall;
                    }
                    else if (_dungeonCells[randomx, (int)roomPair[1].RoomLimits.y].DungeonCellType == DungeonCellType.NorthWestWall)
                    {
                        _dungeonCells[randomx, (int)roomPair[1].RoomLimits.y].DungeonCellType = DungeonCellType.WestWall;
                    }
                    else if (_dungeonCells[randomx, (int)roomPair[1].RoomLimits.y].DungeonCellType == DungeonCellType.NorthWall)
                    {
                        _dungeonCells[randomx, (int)roomPair[1].RoomLimits.y].DungeonCellType = DungeonCellType.Ground;
                    }

                    //path
                    for (int y = (int)roomPair[1].RoomLimits.w; y < roomPair[0].RoomLimits.y; y++)
                    {
                        pathSuccess = true;

                        _dungeonCells[randomx, y].DungeonCellType = DungeonCellType.NorthSouthPath;
                        _dungeonCells[randomx, y].Direction = Direction.Up;
                        _dungeonCells[randomx, y].PathNumber = pathnumber;
                    }
                }
                else // connect with child0 south - child1 north
                {
                    int minx = roomPair[0].RoomLimits.x > roomPair[1].RoomLimits.x ? (int)roomPair[0].RoomLimits.x : (int)roomPair[1].RoomLimits.x;
                    int maxx = roomPair[0].RoomLimits.z < roomPair[1].RoomLimits.z ? (int)roomPair[0].RoomLimits.z : (int)roomPair[1].RoomLimits.z;

                    int randomx = UnityRandom.Range(minx, maxx);

                    entranceCoordinates.Add(new Vector2(randomx, (int)roomPair[0].RoomLimits.w - 1));
                    entranceCoordinates.Add(new Vector2(randomx, (int)roomPair[1].RoomLimits.y));

                    //room entrances
                    if (_dungeonCells[randomx, (int)roomPair[0].RoomLimits.w - 1].DungeonCellType == DungeonCellType.SouthEastWall)
                    {
                        _dungeonCells[randomx, (int)roomPair[0].RoomLimits.w - 1].DungeonCellType = DungeonCellType.EastWall;
                    }
                    else if (_dungeonCells[randomx, (int)roomPair[0].RoomLimits.w - 1].DungeonCellType == DungeonCellType.SouthWestWall)
                    {
                        _dungeonCells[randomx, (int)roomPair[0].RoomLimits.w - 1].DungeonCellType = DungeonCellType.WestWall;
                    }
                    else if (_dungeonCells[randomx, (int)roomPair[0].RoomLimits.w - 1].DungeonCellType == DungeonCellType.SouthWall)
                    {
                        _dungeonCells[randomx, (int)roomPair[0].RoomLimits.w - 1].DungeonCellType = DungeonCellType.Ground;
                    }

                    if (_dungeonCells[randomx, (int)roomPair[1].RoomLimits.y].DungeonCellType == DungeonCellType.NorthEastWall)
                    {
                        _dungeonCells[randomx, (int)roomPair[1].RoomLimits.y].DungeonCellType = DungeonCellType.EastWall;
                    }
                    else if (_dungeonCells[randomx, (int)roomPair[1].RoomLimits.y].DungeonCellType == DungeonCellType.NorthWestWall)
                    {
                        _dungeonCells[randomx, (int)roomPair[1].RoomLimits.y].DungeonCellType = DungeonCellType.WestWall;
                    }
                    else if (_dungeonCells[randomx, (int)roomPair[1].RoomLimits.y].DungeonCellType == DungeonCellType.NorthWall)
                    {
                        _dungeonCells[randomx, (int)roomPair[1].RoomLimits.y].DungeonCellType = DungeonCellType.Ground;
                    }

                    //path
                    for (int y = (int)roomPair[0].RoomLimits.w; y < roomPair[1].RoomLimits.y; y++)
                    {
                        pathSuccess = true;

                        _dungeonCells[randomx, y].DungeonCellType = DungeonCellType.NorthSouthPath;
                        _dungeonCells[randomx, y].Direction = Direction.Down;
                        _dungeonCells[randomx, y].PathNumber = pathnumber;
                    }
                }
            }

            if (!pathSuccess) // not a valid map, reset generation
            {
                Scene scene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(scene.name);
            }

            pathnumber++;

            roomPair[0].SetConnected(roomPair[1], entranceCoordinates[0],true);
            roomPair[1].SetConnected(roomPair[0], entranceCoordinates[1], true);
        }

        GeneratePath(layer - 1, pathnumber);
    }

    public void InstantiateDoors(QuestScriptable quest) // use quest data later
    {
        // generate uninteractable door on entrance room (opposed to room entrance wall)
        PlaceDoorOnOppositeSide(_entranceRoom, false);
        // generate interactable door on exit room (random place opposed to room entrance wall)
        PlaceDoorOnOppositeSide(_exitRoom, true);
        // generate interactable door on path between the two biggest nodes (hardcorded quest door)
        PlaceDoor(GetLinkingRoom(_dungeonNodes[0]));
    }

    private void PlaceDoor(DungeonNode room)
    {
        DungeonCell cell = null;

        foreach (var roomConnection in room.RoomConnections)
        {
            foreach (var roominroomconnection in roomConnection.ConnectedNode.RoomConnections)
            {
                if (roominroomconnection.ConnectedNode == room)
                    cell = _dungeonCells[(int)roomConnection.Coordinates.x, (int)roomConnection.Coordinates.y];
            }
        }

        if (cell == null)
        {
            print("not able to find cell to place door");
            return;
        }

        Quaternion localRotation = Quaternion.identity;

        //rotate if necessary
        if (_dungeonCells[(int)cell.GridPosition.x, (int)cell.GridPosition.y - 1].DungeonCellType == DungeonCellType.NorthSouthPath)
            localRotation = Quaternion.Euler(0, 180, 0);
        else if (_dungeonCells[(int)cell.GridPosition.x - 1, (int)cell.GridPosition.y].DungeonCellType == DungeonCellType.EastWestPath)
            localRotation = Quaternion.Euler(0, 270, 0);
        else if (_dungeonCells[(int)cell.GridPosition.x + 1, (int)cell.GridPosition.y].DungeonCellType == DungeonCellType.EastWestPath)
            localRotation = Quaternion.Euler(0, 90, 0);

        // instantiate door
        GameObject door = Instantiate(DoorPrefab, cell.transform);
        door.transform.rotation = localRotation;
        door.GetComponentInChildren<Door>().Initialize(true, false);
    }
    private Vector2 PlaceDoorOnOppositeSide(DungeonNode room, bool isExit)
    {
        Vector2 entranceDoorCoordinates = room.RoomConnections[0].Coordinates;
        Quaternion localRotation = Quaternion.identity;

        if (room.RoomConnections[0].Coordinates.x == room.RoomLimits.x) // opposing X
        {
            entranceDoorCoordinates.x = room.RoomLimits.z - 1;
            localRotation = Quaternion.Euler(0, 90, 0);
        }
        else if (room.RoomConnections[0].Coordinates.x + 1 == room.RoomLimits.z)
        {
            entranceDoorCoordinates.x = room.RoomLimits.x;
            localRotation = Quaternion.Euler(0, 270, 0);
        }

        if (room.RoomConnections[0].Coordinates.y == room.RoomLimits.y) // opposing Y
        {
            entranceDoorCoordinates.y = room.RoomLimits.w - 1;
        }
        else if (room.RoomConnections[0].Coordinates.y + 1 == room.RoomLimits.w)
        {
            entranceDoorCoordinates.y = room.RoomLimits.y;
            localRotation = Quaternion.Euler(0, 180, 0);
        }

        //determine cell to instantiate door on
        DungeonCell cell = _dungeonCells[(int)entranceDoorCoordinates.x, (int)entranceDoorCoordinates.y];

        // destroy wall maybe
        Destroy(cell.transform.GetChild(0).GetChild(0).gameObject);

        // instantiate door
        GameObject door = Instantiate(DoorPrefab, cell.transform);
        door.transform.rotation = localRotation;
        door.GetComponentInChildren<Door>().Initialize(false, isExit);

        if (isExit)
            ExitDoor = door.GetComponentInChildren<Door>();

        return entranceDoorCoordinates;
    }

    private List<DungeonNode> GetNodeChildRooms(DungeonNode node)
    {
        if (node.HasRoom)
            return new List<DungeonNode>() { node };

        List<DungeonNode> nodeRooms = new List<DungeonNode>();
        List<DungeonNode> nodesToCheck = new List<DungeonNode>();
        List<DungeonNode> checkedNodes = new List<DungeonNode>();
        List<DungeonNode> nodesToCheckLater = new List<DungeonNode>();

        nodesToCheck.Add(node);

        while(nodesToCheck.Count != 0)
        {
            nodesToCheckLater.Clear();

            foreach (var currentNode in nodesToCheck)
            {
                foreach (var childNode in currentNode.ChildNodes)
                {
                    if (childNode.HasRoom)
                    {
                        nodeRooms.Add(childNode);
                    }
                    else
                    {
                        nodesToCheckLater.Add(childNode);
                    }
                }

                checkedNodes.Add(currentNode);
            }

            //manage checked nodes
            foreach (var checkedNode in checkedNodes)
            {
                nodesToCheck.Remove(checkedNode);
            }
            foreach (var nodeToCheck in nodesToCheckLater)
            {
                nodesToCheck.Add(nodeToCheck);
            }
        }

        return nodeRooms;
    }

    private List<DungeonNode> GetClosestRooms(List<DungeonNode> leftRooms, List<DungeonNode> rightRooms)
    {
        float minDistance = Mathf.Infinity;
        List<DungeonNode> closestRooms = new List<DungeonNode>();

        foreach (var leftRoom in leftRooms)
        {
            foreach (var rightRoom in rightRooms)
            {
                if (Vector2.Distance(leftRoom.RoomCenter, rightRoom.RoomCenter) < minDistance)
                {
                    closestRooms.Clear();

                    minDistance = Vector2.Distance(leftRoom.RoomCenter, rightRoom.RoomCenter);

                    closestRooms.Add(leftRoom);
                    closestRooms.Add(rightRoom);
                }
            }
        }
        return closestRooms;
    }

    private List<DungeonNode> GetFarthestRooms(List<DungeonNode> leftRooms, List<DungeonNode> rightRooms)
    {
        float maxDistance = 0;
        List<DungeonNode> farthestRooms = new List<DungeonNode>();

        foreach (var leftRoom in leftRooms)
        {
            foreach (var rightRoom in rightRooms)
            {
                if (Vector2.Distance(leftRoom.RoomCenter, rightRoom.RoomCenter) > maxDistance)
                {
                    farthestRooms.Clear();

                    maxDistance = Vector2.Distance(leftRoom.RoomCenter, rightRoom.RoomCenter);

                    farthestRooms.Add(leftRoom);
                    farthestRooms.Add(rightRoom);
                }
            }
        }
        return farthestRooms;
    }

    private void SetEntranceAndExitRooms()
    {
        //check the two biggest nodes and determine the farthest two rooms between them
        _leftLeafRooms = GetNodeChildRooms(_dungeonNodes[0].ChildNodes[0]);
        _rightLeafRooms = GetNodeChildRooms(_dungeonNodes[0].ChildNodes[1]);

        List<DungeonNode> farthestRooms = new List<DungeonNode>(GetFarthestRooms(_leftLeafRooms, _rightLeafRooms));

        foreach (var room in farthestRooms)
        {
            if (_leftLeafRooms.Contains(room))
                _entranceRoom = room;
            if (_rightLeafRooms.Contains(room))
                _exitRoom = room;
        }
    }

    private DungeonNode GetLinkingRoom(DungeonNode node)
    {
        if (node.HasRoom)
            return null;

        if (node.ChildNodes[0].HasRoom) return node.ChildNodes[0];
        if (node.ChildNodes[1].HasRoom) return node.ChildNodes[1];

        List<DungeonNode> leafRooms = GetNodeChildRooms(node.ChildNodes[0]);

        foreach (var room in leafRooms)
        {
            foreach (var connectedroom in room.RoomConnections)
            {
                if (!leafRooms.Contains(connectedroom.ConnectedNode))
                    return room;
            }
        }

        print("no linking room");
        return null;
    }

    private IEnumerator GetRandomCellFromRoom(DungeonNode room, Action<DungeonCell> returnCell)
    {
        int tries = 0;
        if (!room.HasRoom)
        {
            Debug.LogError("Make sure the node has a room");
            yield break;
        }

        DungeonCell foundCell = null;

        while (foundCell == null || foundCell.IsOcupied)
        {
            int randomX = UnityRandom.Range((int)room.RoomLimits.x, (int)room.RoomLimits.z);
            int randomY = UnityRandom.Range((int)room.RoomLimits.y, (int)room.RoomLimits.w);

            foundCell = _dungeonCells[randomX, randomY];

            tries++;
            if (tries > 1000)
                break;

            yield return null;
        }

        foundCell.IsOcupied = true;
        returnCell(tries > 1000 ? null : foundCell);
    }

    private IEnumerator PlacePlayer()
    {
        //after setting entrance and exit rooms, set player on a random tile on entrance room
        DungeonCell randomCell = null;
        yield return StartCoroutine(GetRandomCellFromRoom(_entranceRoom, (resultCell) => randomCell = resultCell));
        yield return null;
        _player.Initialize(new Vector3(randomCell.transform.position.x, randomCell.transform.position.y + 3f, randomCell.transform.position.z));
    }

    private IEnumerator PlaceEnemies()
    {
        //determine how many rooms will have an enemy
        int numberToPopulate = UnityRandom.Range(_roomNodes.Count / 2, _roomNodes.Count);

        //find random rooms to populate
        List<DungeonNode> allRooms = new List<DungeonNode>(_roomNodes);
        List<DungeonNode> roomsToPopulate = new List<DungeonNode>();

        // don't spawn enemies on entrance room
        allRooms.Remove(_entranceRoom);

        yield return null;

        for (int i = 0; i < numberToPopulate; i++)
        {
            int randomNumber = UnityRandom.Range(0, allRooms.Count);
            roomsToPopulate.Add(allRooms[randomNumber]);
            allRooms.RemoveAt(randomNumber);
        }

        //populate rooms
        foreach (var room in roomsToPopulate)
        {
            DungeonCell cell = null;
            yield return GetRandomCellFromRoom(room, (resultCell) => cell = resultCell);

            if (cell == null)
                break;

            Vector3 cellPosition = cell.transform.position;
            Vector3 enemyPosition = new Vector3(cellPosition.x, cellPosition.y + 3f, cellPosition.z);
            yield return null;
            _enemyManager.SpawnEnemy(enemyPosition);
        }

        // for now, enemy number defines quest requirement
        // later maybe change to be the other way around
        foreach (var goal in _quest.ObjectiveGoals)
        {
            if (goal.Type == ObjectiveType.Defeat)
            {
                StartCoroutine(_enemyManager.QueryEnemyCount((result) =>
                {
                    goal.Quantity = result;
                    EventManager.OnEnemiesPlaced?.Invoke(result);
                }));

                break;
            }
        }
    }

    private IEnumerator PlaceNPCs()
    {
        int requiredNPCs = _quest.RequiredNPCs;

        if (requiredNPCs <= 0)
            yield break;

        //place NPCs only on rooms that are on the left node of the BSP, starting with entrance room
        List<DungeonNode> leftNodeRooms = new List<DungeonNode>(_leftLeafRooms);

        DungeonCell randomCell = null;
        yield return StartCoroutine(GetRandomCellFromRoom(_entranceRoom, (resultCell) => randomCell = resultCell));
        yield return null;
        _npcManager.SpawnNPC(new Vector3(randomCell.transform.position.x, randomCell.transform.position.y + 3f, randomCell.transform.position.z));
        randomCell.IsOcupied = true;

        requiredNPCs--;

        if (requiredNPCs > 0)
        {
            leftNodeRooms.Remove(_entranceRoom);

            for (int i = 0; i < requiredNPCs; i++)
            {
                //pick random left node room
                int randomRoom = UnityRandom.Range(0, leftNodeRooms.Count);

                //populate room
                yield return StartCoroutine(GetRandomCellFromRoom(leftNodeRooms[randomRoom], (resultCell) => randomCell = resultCell));
                
                if (randomCell == null)
                    break;

                yield return null;
                _npcManager.SpawnNPC(new Vector3(randomCell.transform.position.x, randomCell.transform.position.y + 3f, randomCell.transform.position.z));
                randomCell.IsOcupied = true;

                //remove room from pool
                leftNodeRooms.RemoveAt(randomRoom);
            }
        }
    }

    private IEnumerator PlaceItems()
    {
        DungeonCell randomCell = null;

        //place quest items outside of entrance room within the left node
        List<DungeonNode> leftNodeRooms = new List<DungeonNode>(_leftLeafRooms);

        //remove entrance room
        if (leftNodeRooms.Count > 1)
            leftNodeRooms.Remove(_entranceRoom);

        foreach (var goal in _quest.ObjectiveGoals)
        {
            if (goal.Type == ObjectiveType.Collect)
            {
                int randomRoom = UnityRandom.Range(0, leftNodeRooms.Count);
                yield return StartCoroutine(GetRandomCellFromRoom(leftNodeRooms[randomRoom], (resultCell) => randomCell = resultCell));
                yield return null;
                StartCoroutine(_itemsManager.SpawnItem(randomCell.transform.position, goal.Item));
                randomCell.IsOcupied = true;
            }
        }

        //place random items on all rooms
        int itemsPerRoom = 2;

        if (_roomNodes.Count <= 0)
        {
            yield break;
        }

        foreach (var room in _roomNodes)
        {
            for (int i = 0; i < itemsPerRoom; i++)
            {
                int randomRoom = UnityRandom.Range(0, _roomNodes.Count);
                yield return StartCoroutine(GetRandomCellFromRoom(room, (resultCell) => randomCell = resultCell));
                yield return null;
                StartCoroutine(_itemsManager.SpawnItem(randomCell.transform.position));
                randomCell.IsOcupied = true;
            }
        }
    }
}
