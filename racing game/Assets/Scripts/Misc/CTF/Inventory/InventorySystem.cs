using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    private Dictionary<ItemData, Item> itemDictionary;
    public List<Item> inventory;
    public List<InventorySlot> inventorySlots;

    public static InventorySystem Instance;

    private void Awake()
    {
        Instance = this;
        inventory = new();
        itemDictionary = new();
    }

    public void Add(ItemData referenceData)
    {
        // if the item is already logged in the inventory dict, add to the item's stack amount rather than logging it again, otherwise log it
        if (itemDictionary.TryGetValue(referenceData, out Item value))
        {
            value.AddToStack();
        }
        else
        {
            // look for first available inventory slot to add new item to
            foreach (InventorySlot slot in inventorySlots)
            {
                if (!slot.containsItem)
                {
                    Item newItem = new(referenceData);
                    newItem.AddComponent<ItemUI>();
                    inventory.Add(newItem);
                    itemDictionary.Add(referenceData, newItem);
                    InventorySlot.Instance.Set(newItem);

                    break;
                }
            }
        }
    }

    public void Remove(ItemData referenceData)
    {
        // if the item is already logged in the inventory dict, remove from the item's stack size, otherwise remove the item entry
        if (itemDictionary.TryGetValue(referenceData, out Item value))
        {
            value.RemoveFromStack();
            InventorySlot.Instance.Get(value);

            if (value.stackSize == 0)
            {
                inventory.Remove(value);
                itemDictionary.Remove(referenceData);
            }
        }
    }
}
