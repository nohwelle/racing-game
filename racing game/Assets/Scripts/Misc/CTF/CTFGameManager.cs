using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CTFGameManager : MonoBehaviour
{
    public GameObject AIPrefab;
    public GameObject spawnPoint;

    public float numOfAIToAdd;

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
            Instantiate(AIPrefab, spawnPoint.transform.position, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
