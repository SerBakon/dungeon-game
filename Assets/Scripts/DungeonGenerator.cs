using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] private Vector3Int startPos = Vector3Int.zero;
    [SerializeField] private int walkLength = 10;
    [SerializeField] private int numRooms = 10;
    [SerializeField] private int roomSize = 10;
    [SerializeField] private bool roof = true;

    [SerializeField] private GameObject floorTile;
    [SerializeField] private GameObject starterTile;
    [SerializeField] private GameObject wallTile;
    [SerializeField] private GameObject doorTile;
    [SerializeField] private GameObject doorX;
    [SerializeField] private GameObject doorZ;
    [SerializeField] private GameObject wallX;
    [SerializeField] private GameObject wallZ;

    private HashSet<Vector3Int> visited = new HashSet<Vector3Int>();
    private HashSet<GameObject> tilesTotal = new HashSet<GameObject>();

    private HashSet<Vector3Int> wallsTotal = new HashSet<Vector3Int>();
    private HashSet<Vector3Int> doorsTotal = new HashSet<Vector3Int>();
    private HashSet<Vector3Int> floorsTotal = new HashSet<Vector3Int>();
    private HashSet<Vector3Int> starterWallTotal = new HashSet<Vector3Int>();

    public static List<Vector3Int> cardinalDirectionsList = new List<Vector3Int>()
    {
        new Vector3Int(0,0,1), //UP
        new Vector3Int(0,0,-1), //DOWN
        new Vector3Int(1,0,0), //RIGHT
        new Vector3Int(-1,0,0) //LEFT
    };

    void Start()
    {
        generate();
    }

    public void generate() {
        clear();
        for (int i = -3; i <= 3; i++) {
            if (i == 0) continue;
            starterWallTotal.Add(new Vector3Int(i, 1, 3));
            starterWallTotal.Add(new Vector3Int(i, 1, -3));
            starterWallTotal.Add(new Vector3Int(-3, 1, i));
            starterWallTotal.Add(new Vector3Int(3, 1, i));
        }
        starterWallTotal.Add(new Vector3Int(3, 1, 0));
        //wallsTotal.Add(new Vector3Int(4,0,0));
        starterGen(starterTile);
        roomGen(numRooms, Vector3Int.zero);
        //generate wall
        foreach (var wall in wallsTotal) {
            if (!(doorsTotal.Contains(wall))) {
                GameObject spawnedTile = Instantiate(wallTile, new Vector3Int(wall.x, wall.y, wall.z) + Vector3Int.up, Quaternion.identity);
                tilesTotal.Add(spawnedTile);
                //wallObjectTotal.Add(spawnedTile); // Track the instantiated tiles
            }
            
        }
        //generate walls for starter
        for (int i = -3; i <= 3; i++) {
            if (i == 0) continue;
            Instantiate(wallTile, new Vector3Int(i, 1, 3), Quaternion.identity);
            Instantiate(wallTile, new Vector3Int(i, 1, -3), Quaternion.identity);
            Instantiate(wallTile, new Vector3Int(-3, 1, i), Quaternion.identity);
            Instantiate(wallTile, new Vector3Int(3, 1, i), Quaternion.identity);
            wallsTotal.Add(new Vector3Int(i, 1, 3));
            wallsTotal.Add(new Vector3Int(i, 1, -3));
            wallsTotal.Add(new Vector3Int(-3, 1, i));
            wallsTotal.Add(new Vector3Int(3, 1, i));
        }
        Instantiate(wallTile, new Vector3Int(-3, 1, 0), Quaternion.identity);
        wallsTotal.Add(new Vector3Int(3, 1, 0));
        fillDoors();
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
            GameObject spawnedTile = Instantiate(tile, new Vector3(floor.x, floor.y, floor.z), Quaternion.identity);
            tilesTotal.Add(spawnedTile); // Track the instantiated tiles
        }
    }
    private void roofGen(GameObject tile, HashSet<Vector3Int> floorPos) {
        foreach (var floor in floorPos) {
            GameObject spawnedTile = Instantiate(tile, new Vector3(floor.x, floor.y, floor.z) + Vector3Int.up * 2, Quaternion.identity);
            tilesTotal.Add(spawnedTile); // Track the instantiated tiles
        }
    }
    private void roomGen(int numRooms, Vector3Int center) {
        HashSet<Vector3Int> floorPos = new HashSet<Vector3Int>();
        for (int i = 0; i < numRooms; i++) {
            // doesn't change center for the 3 starter rooms.
            if (center == Vector3Int.zero) {
                center = wallsTotal.ElementAt(Random.Range(0, wallsTotal.Count));
            }
            floorPos = RandomWalk.randomWalk(center, walkLength, roomSize, visited);

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
            if(roof) {
                roofGen(floorTile, floorPos);
            }
            //generates wall
            wallGen(wallTile, floorPos);
        }
    }
    private void doorGen(Vector3Int doorPos) {
        //check to see if up,down,left,right contains either a wall tile or a door tile, if it does, generate a wall, if not, generate a door in the prefab
        doorsTotal.Add(doorPos);
        GameObject door = Instantiate(doorTile, doorPos + Vector3Int.up, Quaternion.identity);
        tilesTotal.Add(door);
    }

    private void fillDoors() {
        //Debug.Log("fillDoors is called");
        foreach (var door in doorsTotal) {
            
            var doorCopy = door + Vector3Int.up;
            if (floorsTotal.Contains(door + cardinalDirectionsList[0])) {
                var doorBlock = Instantiate(doorZ, new Vector3(doorCopy.x, doorCopy.y - 0.2f, doorCopy.z + 0.475f), Quaternion.identity);
                tilesTotal.Add(doorBlock);
            }
            else {
                var wallBlock = Instantiate(wallZ, new Vector3(doorCopy.x, doorCopy.y, doorCopy.z + 0.475f), Quaternion.identity);
                tilesTotal.Add(wallBlock);
            }
            //create door down on Z
            if (floorsTotal.Contains(door + cardinalDirectionsList[1])) {
                var doorBlock = Instantiate(doorZ, new Vector3(doorCopy.x, doorCopy.y - 0.2f, doorCopy.z - 0.475f), Quaternion.identity);
                tilesTotal.Add(doorBlock);
            }
            else {
                var wallBlock = Instantiate(wallZ, new Vector3(doorCopy.x, doorCopy.y, doorCopy.z - 0.475f), Quaternion.identity);
                tilesTotal.Add(wallBlock);
            }
            //create door up on X
            if (floorsTotal.Contains(door + cardinalDirectionsList[2])) {
                var doorBlock = Instantiate(doorX, new Vector3(doorCopy.x + 0.475f, doorCopy.y - 0.2f, doorCopy.z), Quaternion.identity);
                tilesTotal.Add(doorBlock);
            }
            else {
                var wallBlock = Instantiate(wallX, new Vector3(doorCopy.x + 0.475f, doorCopy.y, doorCopy.z), Quaternion.identity);
                tilesTotal.Add(wallBlock);
            }
            //create door down on X
            if (floorsTotal.Contains(door + cardinalDirectionsList[3])) {
                var doorBlock = Instantiate(doorX, new Vector3(doorCopy.x - 0.475f, doorCopy.y - 0.2f, doorCopy.z), Quaternion.identity);
                tilesTotal.Add(doorBlock);
            }
            else {
                var wallBlock = Instantiate(wallX, new Vector3(doorCopy.x - 0.475f, doorCopy.y, doorCopy.z), Quaternion.identity);
                tilesTotal.Add(wallBlock);
            }
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
        doorGen(new Vector3Int(0, 0, 3));
        doorGen(new Vector3Int(0, 0, -3));

        roomGen(1, new Vector3Int(4, 0, 0));
        roomGen(1, new Vector3Int(0, 0, 4));
        roomGen(1, new Vector3Int(0, 0, -4));
        visited.UnionWith(floorPos);
        floorsTotal.UnionWith(floorPos);
        tileGen(tile, floorPos);
        roofGen(tile, floorPos);
    }

    private void wallGen(GameObject tile, HashSet<Vector3Int> floorPos) {
        HashSet<Vector3Int> wallPos = new HashSet<Vector3Int>();
        foreach (var floor in floorPos) {
            var tileUp = floor + cardinalDirectionsList[0];
            var tileDown = floor + cardinalDirectionsList[1];
            var tileRight = floor + cardinalDirectionsList[2];
            var tileLeft = floor + cardinalDirectionsList[3];

            if(!floorPos.Contains(tileUp)) {
                wallPos.Add(tileUp);
            }
            if (!floorPos.Contains(tileDown)) {
                wallPos.Add(tileDown);
            }
            if (!floorPos.Contains(tileRight)) {
                wallPos.Add(tileRight);
            }
            if (!floorPos.Contains(tileLeft)) {
                wallPos.Add(tileLeft);
            }
        }
        visited.UnionWith(wallPos);
        wallsTotal.UnionWith(wallPos);
    }

}
