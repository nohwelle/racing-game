using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public List<GameObject> roomPrefabs;

    public Vector2 prefabGenerationPosition;
    public float prefabHeightVariation;
    public string targetType = "Teleporter"; // Class of the object you want to find

    List<GameObject> allRooms = new();
    List<GameObject> allTeleporters = new();
    float prefabGenerationBuffer = 3;
    public float roomsGenerated;

    public static LevelGenerator Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        for (var i = 0; i < prefabGenerationBuffer; i++)
        {
            // create first room at prefab gen position
            allRooms.Add(Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Count - 1)], new Vector2(prefabGenerationPosition.x, prefabGenerationPosition.y + (prefabHeightVariation * i)), Quaternion.identity));
            roomsGenerated++;
        }

        // sort objects by creation time
        allRooms.Sort((obj1, obj2) => obj2.GetInstanceID() - obj1.GetInstanceID());

        for (var i = 0; i < allRooms.Count; i++)
        {
            allRooms[i].name = "Room " + i;
        }

        GetExistingTeleporters();
    }

    void GetExistingTeleporters()
    {
        Teleporter[] foundTeleporters = FindObjectsOfType<Teleporter>();

        foreach (Teleporter teleporter in foundTeleporters)
        {
            if (!allTeleporters.Contains(teleporter.gameObject))
            {
                allTeleporters.Add(teleporter.gameObject);
            }
        }

        LinkNewTeleporters();
    }

    void LinkNewTeleporters()
    {
        // sort objects by creation time
        allTeleporters.Sort((obj1, obj2) => obj2.GetInstanceID() - obj1.GetInstanceID());

        for (var i = 0; i < allTeleporters.Count - 2; i += 2)
        {
            if (!allTeleporters[i].GetComponent<Teleporter>().linkedTeleporter)
            {
                allTeleporters[i].GetComponent<Teleporter>().linkedTeleporter = allTeleporters[i + 3];
            }
        }
    }

    public void GenerateNewRoom(GameObject player)
    {
        // generate new rooms above old ones - y position of room gen based on num of rooms player has completed
        if (roomsGenerated - prefabGenerationBuffer != player.GetComponent<Racer>().roomsCompleted && roomPrefabs.Count != 0)
        {
            allRooms.Add(Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Count - 1)], new Vector2(prefabGenerationPosition.x, prefabGenerationPosition.y + prefabHeightVariation * roomsGenerated), Quaternion.identity));
            roomsGenerated++;

            // sort objects by creation time
            allRooms.Sort((obj1, obj2) => obj2.GetInstanceID() - obj1.GetInstanceID());

            for (var i = 0; i < allRooms.Count; i++)
            {
                allRooms[i].name = "Room " + i;
            }

            GetExistingTeleporters();
        }
    }
}
