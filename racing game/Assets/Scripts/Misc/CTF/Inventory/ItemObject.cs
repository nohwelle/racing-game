using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<CTFRunner>())
        {
            print("guh");

            // only pick up item if there is an available inventory slot
            foreach (InventorySlot slot in InventorySystem.Instance.inventorySlots)
            {
                print("wuh");

                if (!slot.containsItem)
                {
                    print("buh");

                    InventorySystem.Instance.Add(GetComponent<Item>().data);
                    transform.SetParent(slot.transform);

                    break;
                }
            }
        }
    }
}
