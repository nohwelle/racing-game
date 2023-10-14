using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    Player player;

    public List<GameObject> roomPrefabs;

    public Vector2 prefabGenerationPosition;
    public float prefabHeightVariation;
    public string targetType = "Teleporter"; // Class of the object you want to find

    List<GameObject> allTeleporters = new List<GameObject>();
    float prefabGenerationBuffer = 3;
    float roomsGenerated;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    private void Start()
    {
        for (var i = 0; i < prefabGenerationBuffer; i++)
        {
            // create first room at prefab gen position
            Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Count - 1)], new Vector2(prefabGenerationPosition.x, prefabGenerationPosition.y + (prefabHeightVariation * i)), Quaternion.identity);
            roomsGenerated++;
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

    // Update is called once per frame
    void Update()
    {
        // generate new rooms above old ones - y position of room gen based on num of rooms player has completed
        if (roomsGenerated - prefabGenerationBuffer + 1 != player.roomsCompleted + 1 && roomPrefabs.Count != 0)
        {
            Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Count - 1)], new Vector2(prefabGenerationPosition.x, prefabGenerationPosition.y + prefabHeightVariation * roomsGenerated), Quaternion.identity);
            roomsGenerated++;
            GetExistingTeleporters();
        }
    }
}
