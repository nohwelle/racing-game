using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public GameObject linkedTeleporter;
    public GameObject roomBackground;

    bool hasGeneratedNewRoom;

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
        if ((collision.gameObject.GetComponent<Player>() || collision.gameObject.GetComponent<AI>()) && linkedTeleporter)
        {
            GameObject player = collision.gameObject;

            // teleport player
            player.transform.position = new Vector2(linkedTeleporter.transform.position.x + collision.gameObject.transform.localScale.x, linkedTeleporter.transform.position.y);

            // increment rooms completed for real players
            player.GetComponent<Racer>().roomsCompleted++;

            // change background color to color of first entered player
            if (roomBackground && !hasGeneratedNewRoom)
            {
                roomBackground.GetComponent<SpriteRenderer>().color = player.GetComponent<SpriteRenderer>().material.GetColor("_Player_Color");

                // fix the alpha
                Color color = roomBackground.GetComponent<SpriteRenderer>().color;
                color.a = 1f;
                roomBackground.GetComponent<SpriteRenderer>().color = color;
            }

            // generate a new room
            if (!hasGeneratedNewRoom)
            {
                LevelGenerator.Instance.GenerateNewRoom(player);
                hasGeneratedNewRoom = true;
            }
        }
    }
}
