using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Elevator : MonoBehaviour
{
    public GameObject linkedElevator;
    public GameObject playerUsingElevator;
    public TMP_Text useElevatorText;
    public Transform useElevatorTextPosition;

    public float teamIdentity;
    public float elevatorDelay;

    private void Start()
    {
        useElevatorText.transform.position = useElevatorTextPosition.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<CTFRunner>() && collision.gameObject.GetComponent<CTFRunner>().teamIdentity == teamIdentity)
        {
            // play elevator opening animation

            playerUsingElevator = collision.gameObject;
            useElevatorText.text = "''<USE>'' TO USE ELEVATOR";
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<CTFRunner>() && collision.gameObject.GetComponent<CTFRunner>().teamIdentity == teamIdentity)
        {
            // play elevator closing animation

            playerUsingElevator = null;
            useElevatorText.text = "";
        }
    }

    private void Update()
    {
        if (playerUsingElevator && playerUsingElevator.GetComponent<CTFPlayer>() && Input.GetKeyDown(playerUsingElevator.GetComponent<CTFPlayerMovement>().useKey))
        {
            StartCoroutine(ElevatePlayer(playerUsingElevator));
        }
    }

    public IEnumerator ElevatePlayer(GameObject player)
    {
        // play elevator closing animation
        yield return new WaitForSeconds(elevatorDelay);

        player.transform.position = linkedElevator.transform.position;

        yield break;
    }
}
