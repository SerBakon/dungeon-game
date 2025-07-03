using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class RandomWalk {
    public static Vector3Int startReal;
    public static Vector3Int prevMove;

    public static List<Vector3Int> cardinalDirectionsList = new List<Vector3Int>()
    {
        new Vector3Int(0,0,1), //UP
        new Vector3Int(0,0,-1), //DOWN
        new Vector3Int(1,0,0), //RIGHT
        new Vector3Int(-1,0,0) //LEFT
    };
    public static HashSet<Vector3Int> randomWalk(Vector3Int startPos, int walkLength, int maxLen, HashSet<Vector3Int> walkedSet) {
        HashSet<Vector3Int> floorPos = new HashSet<Vector3Int>();

        int attempts1 = 0;
        //if start pos is in a taken space, move up, down, right, or left until you reach a free space.
        while ((walkedSet.Contains(startPos) || Mathf.Abs(startPos.x) <= 3 && Mathf.Abs(startPos.z) <= 3) && attempts1 < 100) {
            //Debug.Log("Started in a taken space");
            Vector3Int shift = cardinalDirectionsList[Random.Range(0, 3)];
            startPos += shift;
            prevMove = shift;
            attempts1++;
        }

        if(attempts1 >= 100) {
            return floorPos;
        }
        floorPos.Add(startPos);
        startReal = startPos;

        //Debug.Log("start pos: " + startPos);

        Vector3Int prevPos = startPos;

        for (int i = 0; i < walkLength; i++) {
            int attempts = 0;
            Vector3Int newPos;
            do {
                Vector3Int direction = cardinalDirectionsList[Random.Range(0, cardinalDirectionsList.Count)];
                newPos = prevPos + direction;
                attempts++;
            } while ((walkedSet.Contains(newPos) || Mathf.Abs(newPos.x - startPos.x) > maxLen || Mathf.Abs(newPos.z - startPos.z) > maxLen || Mathf.Abs(newPos.x) <= 3 && Mathf.Abs(newPos.z) <= 3) && attempts < 100);

            if (attempts < 100) {
                floorPos.Add(newPos);
                prevPos = newPos;
                //Debug.Log("Added newPos at: " + newPos);
            }
            else {
                // Dead-end, choose a random previous floor tile to resume from
                prevPos = floorPos.ElementAt(Random.Range(0, floorPos.Count));
                //Debug.Log("Too many attempts. Jumping to random floorPos.");
            }
        }

        return floorPos;
    }
}