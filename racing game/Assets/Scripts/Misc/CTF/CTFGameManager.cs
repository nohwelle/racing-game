using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CTFGameManager : MonoBehaviour
{
    public GameObject AIPrefab;
    [SerializeField] private List<GameObject> baseSpawnPoints;

    public float numOfAIToAdd;
    public float numOfTeams = 4;

    public static CTFGameManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // add AI players
        for (var i = 0; i < numOfAIToAdd; i++)
        {
            Instantiate(AIPrefab);
        }

        // assign all runners to a team
        // red - 0, blue - 1, green - 2, yellow - 3
        int teamToAssign = 0;

        for (var i = 0; i < FindObjectsOfType<CTFRunner>().Length; i++)
        {
            FindObjectsOfType<CTFRunner>()[i].teamIdentity = teamToAssign;

            teamToAssign++;
            if (teamToAssign > numOfTeams - 1)
            {
                teamToAssign = 0;
            }
        }

        // move AI players to their spawns
        foreach (CTFAI AI in FindObjectsOfType<CTFAI>())
        {
            AI.gameObject.transform.position = baseSpawnPoints[AI.GetComponent<CTFRunner>().teamIdentity].transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
