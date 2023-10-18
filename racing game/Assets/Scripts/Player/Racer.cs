using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Racer : MonoBehaviour
{
    public float roomsCompleted;
    public float currentPlacement;
    public float progressValue;

    public bool isInHitStun;
    public float hitStunDuration;

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

        // start hitstun -- this has to be added in case the obstacle invoking hitstun gets removed for any reason
        if (isInHitStun)
        {
            StartCoroutine(HitStunCooldown());
        }
    }


    public IEnumerator HitStunCooldown()
    {
        yield return new WaitForSeconds(hitStunDuration);

        hitStunDuration = 0;
        isInHitStun = false;

        yield break;
    }
}
