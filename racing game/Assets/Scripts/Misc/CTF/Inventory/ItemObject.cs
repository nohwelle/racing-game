using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemObject : MonoBehaviour
{
    public static event HandleItemCollected OnItemCollected;
    public delegate void HandleItemCollected(ItemData itemData);
    public ItemData itemData;

    public float allowPickUpDelayTime = 1.0f;
    bool canBePickedUp;

    private void OnEnable()
    {
        StartCoroutine(DisablePickUpOnDrop());
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<CTFRunner>() && canBePickedUp)
        {
            // add check to see if any inventory slots are vacant

            // do collection stuff (i honestly don't know)
            OnItemCollected?.Invoke(itemData);
            Destroy(gameObject);
        }
    }

    public IEnumerator DisablePickUpOnDrop()
    {
        yield return new WaitForSeconds(allowPickUpDelayTime);

        // -- NOTE: this doesn't need to be set to false at any point since it resets to false when instantiated and gets destroyed on pickup
        canBePickedUp = true;

        yield break;
    }
}
