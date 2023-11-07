using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTFRunner : MonoBehaviour
{
    public int teamIdentity;

    public bool isInHitStun;
    public float hitStunDuration;
    public float hitRecoveryDuration = 2f;

    // Update is called once per frame
    void Update()
    {
        // start hitstun -- this has to be added in case the obstacle invoking hitstun gets removed for any reason (looking at you, explosives)
        if (isInHitStun)
        {
            StartCoroutine(HitStunCooldown());
        }
    }

    public IEnumerator HitStunCooldown()
    {
        // -- TO DO: make visual indicator for hit recovery e.g. blinking player sprite

        yield return new WaitForSeconds(hitStunDuration);

        isInHitStun = false;

        // hit recovery time should always be greater than hit stun time
        yield return new WaitForSeconds(hitRecoveryDuration - hitStunDuration);

        hitStunDuration = 0;

        yield break;
    }
}
