using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Racer : MonoBehaviour
{
    public float roomsCompleted;
    public float currentPlacement;
    public float progressValue;

    public bool isInHitStun;

    // Update is called once per frame
    void Update()
    {
        // determine current placement
        progressValue = transform.position.x - GameManager.Instance.spawnPoint.transform.position.x + roomsCompleted * 100;
        
        for (var i = 0; i < GameManager.Instance.allRacers.Count; i++)
        {
            if (GameManager.Instance.allRacers[i] == gameObject)
            {
                currentPlacement = i + 1;
            }
        }
    }

    public IEnumerator HitStunCooldown(float hitStunTime)
    {
        isInHitStun = true;

        yield return new WaitForSeconds(hitStunTime);

        isInHitStun = false;

        yield break;
    }
}
