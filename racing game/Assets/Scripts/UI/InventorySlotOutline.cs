using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using UnityEngine;

public class InventorySlotOutline : MonoBehaviour
{
    void Update()
    {
        foreach (InventorySlot slot in InventorySystem.Instance.inventorySlots)
        {
            if (slot.isSelected)
            {
                transform.position = slot.transform.position;
            }
        }
    }
}
