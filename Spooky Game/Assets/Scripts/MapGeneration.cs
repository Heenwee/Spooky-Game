using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class MapGeneration : MonoBehaviour
{
    public Transform grid;

    public static bool generated;
    bool mainBranchGenerated, branchesGenerated, tilesPlaced;

    public int loops;
    public float sideSectionLength, upSectionLength;
    public int branchLenghtMin, branchLenghtMax;
    public int branchNr = 5;
    Vector3[] points;
    List<Vector2> pointPos;

    //List<Vector2> branchPoints;
    List<List<Vector2>> branches;

    int lastDir; // 1 = left, 2 = right, 3 = up

    [Header("Tiles")]
    public RuleTile tile;
    public Tilemap tilemap;
    public int edgeOffset;
    public int distanceFromLine = 5;
    public Tilemap[] rooms;

    // Start is called before the first frame update
    void Start()
    {
        generated = false;
        mainBranchGenerated = false;
        branchesGenerated = false;
        tilesPlaced = false;

        pointPos = new List<Vector2>();
        branches = new List<List<Vector2>>();

        points = new Vector3[loops];

        lastDir = 0;

        points[0] = new Vector2(0, 0);
        GenerationOrder();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Generate(Vector3[] p, int lenght, List<Vector2> branchP)
    {
        bool stop = false;
        int generatedPoints = 1;
        for (int i = 1; i < lenght; i++)
        {
            if(!stop)
            {
                int dir = Random.Range(1, 4);

                if ((dir == 1 && lastDir != 2) ||
                    (dir == 2 && lastDir != 1) ||
                    dir == 3)
                {
                    Vector3 change = Vector3.zero;
                    if (dir == 1)
                    {
                        change = Vector2.left * sideSectionLength;
                    }
                    if (dir == 2)
                    {
                        change = Vector2.right * sideSectionLength;
                    }
                    if (dir == 3)
                    {
                        change = Vector2.up * upSectionLength;
                    }
                    p[i] = p[i - 1] + change;

                    lastDir = dir;

                    foreach (Vector3 pos in pointPos)
                    {
                        if (p[i] == pos) stop = true;
                    }

                    generatedPoints++;
                    pointPos.Add(p[i]);
                    branchP.Add(p[i]);
                }
                else i--;
            }
        }
        /*LineRenderer line = new GameObject().AddComponent<LineRenderer>();
        line.positionCount = generatedPoints;
        for (int j = 0; j < line.positionCount; j++)
        {
            line.SetPosition(j, p[j]);
        }*/
    }
    void GenerationOrder()
    {
        List<Vector2> mainBranchList = new List<Vector2>();
        mainBranchList.Add(points[0]);
        pointPos.Add(points[0]);
        Generate(points, loops, mainBranchList);
        branches.Add(mainBranchList);
        for (int i = 0; i < branchNr; i++) GenerateBranches();
        InsertBricks();
    }
    void GenerateBranches()
    {
        List<Vector2> branchList = new List<Vector2>();
        int branchLenght = Random.Range(branchLenghtMin, branchLenghtMax + 1);

            Vector3[] bPoints = new Vector3[branchLenght];
            bPoints[0] = points[Random.Range(0, points.Length)];
        branchList.Add(bPoints[0]);

            Generate(bPoints, branchLenght, branchList);

        branches.Add(branchList);
    }

    void InsertBricks()
    {
        Vector2 bottom = Vector3.zero;
        Vector2 top = Vector3.zero;
        Vector2 left = Vector3.zero;
        Vector2 right = Vector3.zero;

        foreach (Vector3 point in pointPos)
        {
            if (point.y < bottom.y) bottom  = point;
            if (point.y > top.y)    top     = point;
            if (point.x < left.x)   left    = point;
            if (point.x > right.x)  right   = point;
        }
        bottom += new Vector2(0, -edgeOffset);
        top += new Vector2(0, edgeOffset);
        left += new Vector2(-edgeOffset, 0);
        right += new Vector2(edgeOffset, 0);

        int startX = tilemap.WorldToCell(left).x;
        int startY = tilemap.WorldToCell(bottom).y;
        int endX = tilemap.WorldToCell(right).x;
        int endY = tilemap.WorldToCell(top).y;

        tilemap.SetTile(new Vector3Int(startX, startY, 0), tile);
        tilemap.SetTile(new Vector3Int(endX, startY, 0), tile);
        tilemap.SetTile(new Vector3Int(startX, endY, 0), tile);
        tilemap.SetTile(new Vector3Int(endX, endY, 0), tile);

        tilemap.BoxFill(Vector3Int.zero, tile, startX, startY, endX, endY);
        Debug.Log("Tiles Set");

        foreach(List<Vector2> list in branches) GenerateCorridors(list);
        SpawnRooms();

        generated = true;
    }

    void GenerateCorridors(List<Vector2> p)
    {
        for (int x = tilemap.cellBounds.min.x; x < tilemap.cellBounds.max.x; x++)
        {
            for (int y = tilemap.cellBounds.min.y; y < tilemap.cellBounds.max.y; y++)
            {
                for (int z = tilemap.cellBounds.min.z; z < tilemap.cellBounds.max.z; z++)
                {
                    Vector3 pos = tilemap.CellToWorld(new Vector3Int(x, y, z));

                    for (int i = 1; i < p.Count; i++)
                    {
                        if (HandleUtility.DistancePointLine(pos, p[i - 1], p[i]) < distanceFromLine)
                        {
                            tilemap.SetTile(new Vector3Int(x, y, z), null);
                        }
                    }
                }
            }
        }
    }

    void SpawnRooms()
    {
        foreach(Vector2 p in pointPos)
        {
            var room = Instantiate(rooms[0], Vector2.zero, Quaternion.identity);
            room.transform.parent = grid;
            room.transform.position = p;

            Tilemap roomTiles = room.GetComponent<Tilemap>();
            for (int x = roomTiles.cellBounds.min.x; x < roomTiles.cellBounds.max.x; x++)
            {
                for (int y = roomTiles.cellBounds.min.y; y < roomTiles.cellBounds.max.y; y++)
                {
                    for (int z = roomTiles.cellBounds.min.z; z < roomTiles.cellBounds.max.z; z++)
                    {
                        if(tilemap.HasTile(new Vector3Int(x, y, z)))
                        {
                            tilemap.SetTile(new Vector3Int(x, y, z), null);
                        }
                    }
                }
            }
            Destroy(room.gameObject);
        }
    }
}
