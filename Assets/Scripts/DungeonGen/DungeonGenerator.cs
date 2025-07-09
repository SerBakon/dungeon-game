using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.UI;

public class DungeonGenerator : MonoBehaviour
{
    //[SerializeField] private Vector3Int startPos = Vector3Int.zero;
    [Header("RoomEdits")]
    [SerializeField] private int walkLength = 10;
    [SerializeField] private int numRooms = 10;
    [SerializeField] private int roomSize = 10;
    [SerializeField] private int numEnemies = 5;
    [SerializeField] private bool roof = true;
    [SerializeField] private float enemySpawnRates = 300f;

    [Header("GameObjects")]
    [SerializeField] private GameObject floorTile;
    [SerializeField] private GameObject roofTile;
    [SerializeField] private GameObject starterTile;
    [SerializeField] private GameObject wallTile;
    [SerializeField] private GameObject doorTile;
    [SerializeField] private GameObject doorX;
    [SerializeField] private GameObject doorZ;
    [SerializeField] private GameObject wallX;
    [SerializeField] private GameObject wallZ;
    [SerializeField] private GameObject enemy;

    [Header("Transforms")]
    public Transform wallParent;
    public Transform floorParent;
    public Transform roofParent;
    public Transform doorParent;
    public Transform enemyParent;

    private HashSet<Vector3Int> visited = new HashSet<Vector3Int>();
    private HashSet<GameObject> tilesTotal = new HashSet<GameObject>();

    private HashSet<Vector3Int> wallsTotal = new HashSet<Vector3Int>();
    private HashSet<Vector3Int> doorsTotal = new HashSet<Vector3Int>();
    private HashSet<Vector3Int> floorsTotal = new HashSet<Vector3Int>();
    private HashSet<Vector3Int> starterWallTotal = new HashSet<Vector3Int>();
    private HashSet<Vector3Int> starterFloorTotal = new HashSet<Vector3Int>();

    private BuildNavMesh buildNavMesh;

    private float timer;

    public static readonly Vector3Int[] cardinalDirectionsList = new Vector3Int[]
    {
        new Vector3Int(0,0,1), //UP
        new Vector3Int(0,0,-1), //DOWN
        new Vector3Int(1,0,0), //RIGHT
        new Vector3Int(-1,0,0) //LEFT
    };

    void Start()
    {
        Application.targetFrameRate = 100;
        buildNavMesh = GetComponent<BuildNavMesh>();
        generate();
        buildNavMesh.buildMesh();
        generateEnemies();
    }

    private void Update() {
        timer += Time.deltaTime;
        if(timer >= enemySpawnRates) {
            timer = 0;
            generateEnemies();
        }
    }

    public void generate() {
        clear();
        InitializeStarterWalls();
        starterGen(starterTile);
        roomGen(numRooms, Vector3Int.zero);
        //generate wall
        foreach (var wall in wallsTotal) {
            if (!(doorsTotal.Contains(wall))) {
                GameObject spawnedTile = Instantiate(wallTile, new Vector3Int(wall.x, wall.y, wall.z) + Vector3Int.up, Quaternion.identity, wallParent);
                tilesTotal.Add(spawnedTile);
                //wallObjectTotal.Add(spawnedTile); // Track the instantiated tiles
            }
            
        }
        GenStarterWalls();
        fillDoors();
    }

    private void InitializeStarterWalls() {
        for (int i = -3; i <= 3; i++) {
            if (i == 0) continue;
            starterWallTotal.Add(new Vector3Int(i, 0, 3));
            starterWallTotal.Add(new Vector3Int(i, 0, -3));
            starterWallTotal.Add(new Vector3Int(-3, 0, i));
            starterWallTotal.Add(new Vector3Int(3, 0, i));
        }
        //starterWallTotal.Add(new Vector3Int(-3, 1, 0));
    }

    private void GenStarterWalls() {
        //generate walls for starter
        for (int i = -3; i <= 3; i++) {
            if (i == 0) continue;
            Instantiate(wallTile, new Vector3Int(i, 1, 3), Quaternion.identity, wallParent);
            Instantiate(wallTile, new Vector3Int(i, 1, -3), Quaternion.identity, wallParent);
            Instantiate(wallTile, new Vector3Int(-3, 1, i), Quaternion.identity, wallParent);
            Instantiate(wallTile, new Vector3Int(3, 1, i), Quaternion.identity, wallParent);
            wallsTotal.Add(new Vector3Int(i, 0, 3));
            wallsTotal.Add(new Vector3Int(i, 0, -3));
            wallsTotal.Add(new Vector3Int(-3, 0, i));
            wallsTotal.Add(new Vector3Int(3, 0, i));
        }
        //Instantiate(wallTile, new Vector3Int(-3, 1, 0), Quaternion.identity);
        //wallsTotal.Add(new Vector3Int(-3, 0, 0));
    }

    private void clear() {
        foreach (var tile in tilesTotal) {
            Destroy(tile); // Destroy old tiles from the scene
        }
        tilesTotal.Clear(); // Clear the list
        visited.Clear();
        wallsTotal.Clear();
        doorsTotal.Clear();
        floorsTotal.Clear();
    }

    private void tileGen(GameObject tile, HashSet<Vector3Int> floorPos) {
        foreach (var floor in floorPos) {
            tilesTotal.Add(Instantiate(tile, floor, Quaternion.identity, floorParent));
        }
    }
    private void roofGen(GameObject tile, HashSet<Vector3Int> floorPos) {
        if (!roof) return;
        foreach (var floor in floorPos) {
            tilesTotal.Add(Instantiate(tile, floor + (Vector3Int.up * 2), Quaternion.identity, roofParent));
        }
    }
    private void roomGen(int numRooms, Vector3Int center) {
        for (int i = 0; i < numRooms; i++) {
            // doesn't change center for the 3 starter rooms.
            if (center == Vector3Int.zero) {
                center = wallsTotal.ElementAt(Random.Range(0, wallsTotal.Count));
            }
            var floorPos = RandomWalk.randomWalk(center, walkLength, roomSize, visited);

            if (floorPos.Count == 0) {
                //Debug.Log("couldn't find a good start pos");
                continue;
            }
            if (numRooms > 1) {
                var doorPos = RandomWalk.startReal + (RandomWalk.prevMove * -1);
                if (!starterWallTotal.Contains(doorPos)) {
                    doorGen(doorPos);
                }
            }
            visited.UnionWith(floorPos);
            floorsTotal.UnionWith(floorPos);
            //generates floor
            tileGen(floorTile, floorPos);
            //generates roof
            roofGen(roofTile, floorPos);
            //generates wall
            wallGen(floorPos);
        }
    }
    private void doorGen(Vector3Int doorPos) {
        //check to see if up,down,left,right contains either a wall tile or a door tile, if it does, generate a wall, if not, generate a door in the prefab
        doorsTotal.Add(doorPos);
        tilesTotal.Add(Instantiate(doorTile, doorPos + Vector3Int.up, Quaternion.identity, doorParent));
    }

    private void fillDoors() {
        foreach (var door in doorsTotal) {
            var doorCopy = door + Vector3Int.up;

            // Cache direction checks
            bool hasUp = floorsTotal.Contains(door + cardinalDirectionsList[0]);
            bool hasDown = floorsTotal.Contains(door + cardinalDirectionsList[1]);
            bool hasRight = floorsTotal.Contains(door + cardinalDirectionsList[2]);
            bool hasLeft = floorsTotal.Contains(door + cardinalDirectionsList[3]);

            // Z-axis (up/down)
            tilesTotal.Add(Instantiate(
                hasUp ? doorZ : wallZ,
                new Vector3(doorCopy.x, hasUp ? doorCopy.y - 0.2f : doorCopy.y, doorCopy.z + 0.475f),
                Quaternion.identity, doorParent));

            tilesTotal.Add(Instantiate(
                hasDown ? doorZ : wallZ,
                new Vector3(doorCopy.x, hasDown ? doorCopy.y - 0.2f : doorCopy.y, doorCopy.z - 0.475f),
                Quaternion.identity, doorParent));

            // X-axis (right/left)
            tilesTotal.Add(Instantiate(
                hasRight ? doorX : wallX,
                new Vector3(doorCopy.x + 0.475f, hasRight ? doorCopy.y - 0.2f : doorCopy.y, doorCopy.z),
                Quaternion.identity, doorParent));

            tilesTotal.Add(Instantiate(
                hasLeft ? doorX : wallX,
                new Vector3(doorCopy.x - 0.475f, hasLeft ? doorCopy.y - 0.2f : doorCopy.y, doorCopy.z),
                Quaternion.identity, doorParent));
        }
    }
    private void starterGen(GameObject tile) {
        HashSet<Vector3Int> floorPos = new HashSet<Vector3Int>();
        for (int i = -2; i <= 2; i++) {
            for (int j = -2; j <= 2; j++) {
                floorPos.Add(new Vector3Int(i, 0, j));
            }
        }
        doorGen(new Vector3Int(3, 0, 0));
        doorGen(new Vector3Int(-3, 0, 0));
        doorGen(new Vector3Int(0, 0, 3));
        doorGen(new Vector3Int(0, 0, -3));


        roomGen(1, new Vector3Int(4, 0, 0));
        roomGen(1, new Vector3Int(-4, 0, 0));
        roomGen(1, new Vector3Int(0, 0, 4));
        roomGen(1, new Vector3Int(0, 0, -4));
        visited.UnionWith(floorPos);
        floorsTotal.UnionWith(floorPos);
        starterFloorTotal.UnionWith(floorPos);
        tileGen(tile, floorPos);
        roofGen(roofTile, floorPos);
    }

    private void wallGen(HashSet<Vector3Int> floorPos) {
        //HashSet<Vector3Int> wallPos = new HashSet<Vector3Int>();
        foreach (var floor in floorPos) {
            foreach (var dir in cardinalDirectionsList) {
                var neighbor = floor + dir;
                if (!floorPos.Contains(neighbor)) {
                    wallsTotal.Add(neighbor);
                    visited.Add(neighbor);
                }
            }
        }
        //visited.UnionWith(wallPos);
        //wallsTotal.UnionWith(wallPos);
    }

    private void generateEnemies() {
        for (int i = 0; i < numEnemies; i++) {
            //Debug.Log("generating enemies");
            var randomCoordinate = floorsTotal.ElementAt(Random.Range(0, floorsTotal.Count));
            if(!starterFloorTotal.Contains(randomCoordinate)) 
                tilesTotal.Add(Instantiate(enemy, randomCoordinate, Quaternion.identity, enemyParent));
        }
    }
}
