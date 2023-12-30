using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

    [Header("Character Parents")]
    public GameObject CharacterParent;
    public GameObject EnemyParent;

    private DungeonCell[,] _dungeonCells;
    private float _cellSize;
    private int _minPartitionArea;
    private Vector2 _minPartitionSides;

    private List<DungeonNode> _dungeonNodes;
    private List<DungeonNode> _roomNodes;

    private void Awake()
    {
        _dungeonCells = new DungeonCell[20, 20];
        _cellSize = 16f;
        _minPartitionArea = 50;
        _minPartitionSides = new Vector2(4, 4);

        _dungeonNodes = new List<DungeonNode>();
        _roomNodes = new List<DungeonNode>();
    }

    private void Start()
    {
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

    private IEnumerator GenerateBSP(int number)
    {
        bool createdNewNodes = false;
        List<DungeonNode> tempNodes = new List<DungeonNode>();
        // check node list size
        // if 0, create node with map size
        if (_dungeonNodes.Count == 0)
        {
            _dungeonNodes.Add(new DungeonNode(new Vector4(0, 0, _dungeonCells.GetLength(0), _dungeonCells.GetLength(1)), 0, number));
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
                    number++;
                    Vector4 node1limits = new Vector4(node.PartitionLimits.x, node.PartitionLimits.y, node.PartitionLimits.z, randomY);
                    DungeonNode node1 = new DungeonNode(node1limits, node.Layer + 1, number);

                    number++;
                    Vector4 node2limits = new Vector4(node.PartitionLimits.x, randomY, node.PartitionLimits.z, node.PartitionLimits.w);
                    DungeonNode node2 = new DungeonNode(node2limits, node.Layer + 1, number);

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
                    number++;
                    Vector4 node1limits = new Vector4(node.PartitionLimits.x, node.PartitionLimits.y, randomX, node.PartitionLimits.w);
                    DungeonNode node1 = new DungeonNode(node1limits, node.Layer + 1, number);

                    number++;
                    Vector4 node2limits = new Vector4(randomX, node.PartitionLimits.y, node.PartitionLimits.z, node.PartitionLimits.w);
                    DungeonNode node2 = new DungeonNode(node2limits, node.Layer + 1, number);

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
            print("node generation over");
            GenerateNodeRooms();
            GeneratePaths();
            InstantiatePrefabs();
            InitializeCharacters();
            yield break;
        }

        _dungeonNodes.AddRange(tempNodes);

        // repeat until no node can be cut (recursive)
        StartCoroutine(GenerateBSP(number));
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

            Vector2 vectorbetweenchildren = roomPair[0].RoomCenter - roomPair[1].RoomCenter;

            if (Mathf.Abs(vectorbetweenchildren.x) > Mathf.Abs(vectorbetweenchildren.y)) // connect with east/west
            {
                if (vectorbetweenchildren.x > 0) // connect with child1 east - child0 west
                {
                    int miny = roomPair[0].RoomLimits.y > roomPair[1].RoomLimits.y ? (int)roomPair[0].RoomLimits.y : (int)roomPair[1].RoomLimits.y;
                    int maxy = roomPair[0].RoomLimits.w < roomPair[1].RoomLimits.w ? (int)roomPair[0].RoomLimits.w : (int)roomPair[1].RoomLimits.w;

                    int randomy = UnityRandom.Range(miny, maxy);
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
                        _dungeonCells[randomx, y].DungeonCellType = DungeonCellType.NorthSouthPath;
                        _dungeonCells[randomx, y].Direction = Direction.Down;
                        _dungeonCells[randomx, y].PathNumber = pathnumber;
                    }
                }
            }

            pathnumber++;
            roomPair[0].SetConnected();
            roomPair[1].SetConnected();
        }

        GeneratePath(layer - 1, pathnumber);
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

    private void InitializeCharacters()
    {
        //pick a node room at random
        DungeonCell randomCell = null;
        Vector2 nodeRoom = new Vector2(-1, -1);
        List<DungeonCell> roomCells = new List<DungeonCell>();

        while (randomCell == null || randomCell.DungeonCellType != DungeonCellType.Ground)
        {
            nodeRoom.x = UnityRandom.Range(0, _dungeonCells.GetLength(0));
            nodeRoom.y = UnityRandom.Range(0, _dungeonCells.GetLength(1));

            randomCell = _dungeonCells[(int)nodeRoom.x, (int)nodeRoom.y];
        }

        foreach (var cell in _dungeonCells)
        {
            if (cell.NodeNumber == randomCell.NodeNumber)
            {
                roomCells.Add(cell);
            }
        }

        roomCells.Remove(randomCell);

        //get characters
        Character character = CharacterParent.GetComponentInChildren<Character>();
        Character[] enemies = EnemyParent.GetComponentsInChildren<Character>();

        //set character initial positions
        foreach (var enemy in enemies)
        {
            //pick roomcell at random
            int roomCellIndex = UnityRandom.Range(0, roomCells.Count);
            enemy.Initialize(new Vector3(roomCells[roomCellIndex].transform.position.x, roomCells[roomCellIndex].transform.position.y + 3f, roomCells[roomCellIndex].transform.position.z));
            roomCells.Remove(roomCells[roomCellIndex]);
        }

        character.Initialize(new Vector3(randomCell.transform.position.x, randomCell.transform.position.y + 3f, randomCell.transform.position.z));
    }
}
