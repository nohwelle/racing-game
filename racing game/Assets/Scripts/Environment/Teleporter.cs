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
    float numOfRacersEntered;

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
        // teleport racer to linked teleporter if there is one, then increment num of rooms completed
        if (collision.gameObject.GetComponent<Racer>() && linkedTeleporter)
        {
            GameObject racer = collision.gameObject;

            // teleport racer
            racer.transform.position = new Vector2(linkedTeleporter.transform.position.x + collision.gameObject.transform.localScale.x, linkedTeleporter.transform.position.y);

            // increase racer's move speed
            if (racer.GetComponent<PlayerMovement>())
            {
                racer.GetComponent<PlayerMovement>().moveSpeed += 0.25f;
            }
            if (racer.GetComponent<AIMovement>())
            {
                racer.GetComponent<AIMovement>().moveSpeed += 0.25f;
            }

            // increment number of racers that entered this teleporter, kill whoever's last if we're not in room 1
            numOfRacersEntered++;
            if (numOfRacersEntered == GameManager.Instance.allRacers.Count - 1 && racer.GetComponent<Racer>().roomsCompleted != 0)
            {
                Destroy(GameManager.Instance.allRacers[^1]);
                GameManager.Instance.allRacers.RemoveAt(GameManager.Instance.allRacers.Count - 1);
            }

            // increment rooms completed for real players
            racer.GetComponent<Racer>().roomsCompleted++;

            // change background color to color of first entered racer
            if (roomBackground && !hasGeneratedNewRoom)
            {
                roomBackground.GetComponent<SpriteRenderer>().color = racer.GetComponent<SpriteRenderer>().material.GetColor("_Player_Color");

                // fix the alpha
                Color color = roomBackground.GetComponent<SpriteRenderer>().color;
                color.a = 1f;
                roomBackground.GetComponent<SpriteRenderer>().color = color;
            }

            // generate a new room
            if (!hasGeneratedNewRoom)
            {
                LevelGenerator.Instance.GenerateNewRoom(racer);
                hasGeneratedNewRoom = true;
            }
        }
    }
}
