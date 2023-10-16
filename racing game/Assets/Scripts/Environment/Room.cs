using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public List<GameObject> mountedObstacleSlots = new();
    public List<GameObject> unmountedObstacleSlots = new();
    public List<GameObject> mountedObstaclesToGenerate = new();
    public List<GameObject> unmountedObstaclesToGenerate = new();


    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject slot in mountedObstacleSlots)
        {
            GameObject obstacle = Instantiate(mountedObstaclesToGenerate[Random.Range(0, mountedObstaclesToGenerate.Count - 1)]);

            obstacle.transform.position = slot.transform.position;
            obstacle.transform.rotation = Quaternion.identity;
            obstacle.transform.parent = slot.transform.parent;

            Destroy(slot);
        }

        foreach (GameObject slot in unmountedObstacleSlots)
        {
            GameObject obstacle = Instantiate(unmountedObstaclesToGenerate[Random.Range(0, unmountedObstaclesToGenerate.Count - 1)]);

            obstacle.transform.position = slot.transform.position;
            obstacle.transform.rotation = Quaternion.identity;
            obstacle.transform.parent = slot.transform.parent;

            Destroy(slot);
        }
    }
}
