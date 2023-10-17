using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject spawnPoint;

    public float numOfAIPlayersToAdd;

    public List<GameObject> allRacers;


    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // add AI players
        for (var i = 0; i < numOfAIPlayersToAdd; i++)
        {
            Instantiate(playerPrefab, spawnPoint.transform.position, Quaternion.identity);
        }

        // get all racers in a list
        foreach (Racer racer in FindObjectsOfType<Racer>())
        {
            allRacers.Add(racer.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // sort all racers by progression value
        allRacers = allRacers.OrderByDescending((obj1) => obj1.GetComponent<Racer>().progressValue).ToList();
    }
}
