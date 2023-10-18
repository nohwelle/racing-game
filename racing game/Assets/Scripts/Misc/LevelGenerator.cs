using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> roomPrefabs;

    public Vector2 prefabGenerationPosition;
    public string targetType = "Teleporter";

    List<GameObject> allRooms = new();
    List<GameObject> allTeleporters = new();

    float prefabGenerationBuffer = 2; // how many rooms (excluding room 1) to create at the start of the game
    float roomsGenerated; // how many rooms have been created
    float towerHeight;


    public static LevelGenerator Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // create first room at prefab gen position
        allRooms.Add(Instantiate(roomPrefabs[0], new Vector2(prefabGenerationPosition.x, prefabGenerationPosition.y + towerHeight), Quaternion.identity));
        roomsGenerated++;

        // add height of newest room to total tower height
        towerHeight += roomPrefabs[0].GetComponent<Room>().roomCeiling.transform.position.y;

        // create other rooms to fill the gen buffer
        for (var i = 0; i < prefabGenerationBuffer; i++)
        {
            // get new room to generate
            int roomToGenerate = Random.Range(1, roomPrefabs.Count - 1);

            // add new room to the top of the tower
            allRooms.Add(Instantiate(roomPrefabs[roomToGenerate], new Vector2(prefabGenerationPosition.x, prefabGenerationPosition.y + towerHeight), Quaternion.identity));
            roomsGenerated++;

            towerHeight += roomPrefabs[roomToGenerate].GetComponent<Room>().roomCeiling.transform.position.y;

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
        // generate new rooms above old ones
        if (roomsGenerated - prefabGenerationBuffer + 1 != player.GetComponent<Racer>().roomsCompleted && roomPrefabs.Count != 0)
        {
            // get new room to generate
            int roomToGenerate = Random.Range(1, roomPrefabs.Count - 1);

            allRooms.Add(Instantiate(roomPrefabs[roomToGenerate], new Vector2(prefabGenerationPosition.x, prefabGenerationPosition.y + towerHeight), Quaternion.identity));
            roomsGenerated++;

            towerHeight += roomPrefabs[roomToGenerate].GetComponent<Room>().roomCeiling.transform.position.y;

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
