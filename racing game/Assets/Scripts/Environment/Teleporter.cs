using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEditor.Rendering;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public GameObject linkedTeleporter;
    public GameObject roomBackground;

    private void Start()
    {
        List<Teleporter> allTeleporters = new List<Teleporter>();
        foreach (Teleporter teleporter in FindObjectsOfType<Teleporter>())
        {
            allTeleporters.Add(teleporter);
        }

        allTeleporters.Sort((obj1, obj2) => obj2.GetInstanceID() - obj1.GetInstanceID());

        for (var i = 0; i < allTeleporters.Count; i++)
        {
            if (GetComponent<Teleporter>() == allTeleporters[i])
            {
                gameObject.name = "Teleporter " + i;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // teleport player to linked teleporter if there is one, then increment num of rooms completed
        if (collision.gameObject.GetComponent<Player>() && linkedTeleporter)
        {
            GameObject player = collision.gameObject;

            player.transform.position = new Vector2(linkedTeleporter.transform.position.x + collision.gameObject.GetComponent<Player>().transform.localScale.x, linkedTeleporter.transform.position.y);
            player.GetComponent<Player>().roomsCompleted++;
            LevelGenerator.Instance.GenerateNewRoom(collision.gameObject.GetComponent<Player>());

            if (roomBackground)
            {
                roomBackground.GetComponent<SpriteRenderer>().color = player.GetComponent<SpriteRenderer>().color;
            }
        }
    }
}
