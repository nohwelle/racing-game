using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemObject : MonoBehaviour
{
    public static event HandleItemCollected OnItemCollected;
    public delegate void HandleItemCollected(ItemData itemData);
    public ItemData itemData;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<CTFRunner>())
        {
            // do collection stuff (i honestly don't know)
            OnItemCollected?.Invoke(itemData);
            Destroy(gameObject);
        }
    }
}
