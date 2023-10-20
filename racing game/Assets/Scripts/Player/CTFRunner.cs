using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTFRunner : MonoBehaviour
{
    public bool isInHitStun;
    public float hitStunDuration;

    // Update is called once per frame
    void Update()
    {
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
