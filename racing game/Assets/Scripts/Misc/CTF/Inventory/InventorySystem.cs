using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    public List<InventorySlot> inventorySlots = new();

    public static InventorySystem Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        ItemObject.OnItemCollected += Add;
    }

    private void OnDisable()
    {
        ItemObject.OnItemCollected -= Add;
    }

    public void Add(ItemData itemData)
    {
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.itemObject && slot.itemObject.activeInHierarchy && slot.itemObject.GetComponent<ItemUI>().itemData == itemData)
            {
                slot.itemObject.GetComponent<ItemUI>().itemStackSize++;
                slot.itemObject.GetComponent<ItemUI>().itemStackSizeText.text = slot.itemObject.GetComponent<ItemUI>().itemStackSize.ToString();

                break;
            }
            else
            {
                if (slot.itemObject && !slot.itemObject.activeInHierarchy)
                {
                    // enable slot's child and give it the new item's data
                    slot.itemObject.SetActive(true);
                    slot.itemObject.GetComponent<ItemUI>().itemData = itemData;

                    // if an empty slot cloned from a dropped item is filled, re-enable its raycast target
                    if (!slot.itemObject.GetComponent<ItemUI>().image.raycastTarget)
                    {
                        slot.itemObject.GetComponent<ItemUI>().image.raycastTarget = true;
                    }

                    print($"{itemData.itemName} was added to {slot.name}!");

                    break;
                }
            }
        }
    }
}
