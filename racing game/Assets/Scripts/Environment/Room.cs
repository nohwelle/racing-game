using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Room : MonoBehaviour
{
    public GameObject roomCeiling;

    [SerializeField] private List<GameObject> mountedObstacleSlots;
    [SerializeField] private List<GameObject> unmountedObstacleSlots;
    [SerializeField] private List<GameObject> mountedObstaclesToGenerate;
    [SerializeField] private List<GameObject> unmountedObstaclesToGenerate;


    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject slot in mountedObstacleSlots)
        {
            // this function doesn't work like you'd expect it to, probably because lists suck ass
            GameObject obstacle = Instantiate(mountedObstaclesToGenerate[Random.Range(0, mountedObstaclesToGenerate.Count)]);

            obstacle.transform.SetPositionAndRotation(slot.transform.position + obstacle.transform.position, Quaternion.identity);
            obstacle.transform.parent = slot.transform.parent;

            Destroy(slot);
        }

        foreach (GameObject slot in unmountedObstacleSlots)
        {
            GameObject obstacle = Instantiate(unmountedObstaclesToGenerate[Random.Range(0, unmountedObstaclesToGenerate.Count)]);

            obstacle.transform.SetPositionAndRotation(slot.transform.position + obstacle.transform.position, Quaternion.identity);
            obstacle.transform.parent = slot.transform.parent;

            Destroy(slot);
        }
    }
}
