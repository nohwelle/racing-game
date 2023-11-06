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

    private void Update()
    {
        // select inventory slots (scrolling upwards)
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            for (var i = 0; i < inventorySlots.Count; i++)
            {
                // select next slot in order
                if (i != 0 && inventorySlots[i].isSelected)
                {
                    inventorySlots[i - 1].isSelected = true;
                    inventorySlots[i].isSelected = false;

                    break;
                }

                // wrap around in order if last slot is selected
                if (i == 0 && inventorySlots[i].isSelected)
                {
                    inventorySlots[^1].isSelected = true;
                    inventorySlots[i].isSelected = false;

                    break;
                }
            }
        }

        // select inventory slots (scrolling downwards)
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            for (var i = 0; i < inventorySlots.Count; i++)
            {
                // select next slot in order
                if (i != inventorySlots.Count - 1 && inventorySlots[i].isSelected)
                {
                    inventorySlots[i + 1].isSelected = true;
                    inventorySlots[i].isSelected = false;

                    break;
                }

                // wrap around for first slot
                if (i == inventorySlots.Count - 1 && inventorySlots[i].isSelected)
                {
                    inventorySlots[0].isSelected = true;
                    inventorySlots[i].isSelected = false;

                    break;
                }
            }
        }

        // select inventory slots (numbas)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            for (var i = 0; i < inventorySlots.Count; i++)
            {
                inventorySlots[i].isSelected = false;
            }

            inventorySlots[0].isSelected = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            for (var i = 0; i < inventorySlots.Count; i++)
            {
                inventorySlots[i].isSelected = false;
            }

            inventorySlots[1].isSelected = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            for (var i = 0; i < inventorySlots.Count; i++)
            {
                inventorySlots[i].isSelected = false;
            }

            inventorySlots[2].isSelected = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            for (var i = 0; i < inventorySlots.Count; i++)
            {
                inventorySlots[i].isSelected = false;
            }

            inventorySlots[3].isSelected = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            for (var i = 0; i < inventorySlots.Count; i++)
            {
                inventorySlots[i].isSelected = false;
            }

            inventorySlots[4].isSelected = true;
        }
    }

    public void Add(ItemData itemData)
    {
        InventorySlot firstEmptySlot = null;
        bool newItemAdded = false;

        foreach (InventorySlot slot in inventorySlots)
        {
            if (!firstEmptySlot && slot.itemObject && !slot.itemObject.activeInHierarchy)
            {
                firstEmptySlot = slot;
            }

            if (slot.itemObject && slot.itemObject.activeInHierarchy && slot.itemObject.GetComponent<ItemUI>().itemData == itemData)
            {
                slot.itemObject.GetComponent<ItemUI>().itemStackSize++;
                slot.itemObject.GetComponent<ItemUI>().itemStackSizeText.text = slot.itemObject.GetComponent<ItemUI>().itemStackSize.ToString();

                newItemAdded = true;

                break;
            }
        }

        if (firstEmptySlot && !newItemAdded)
        {
            // enable slot's child and give it the new item's data
            firstEmptySlot.itemObject.SetActive(true);
            firstEmptySlot.itemObject.GetComponent<ItemUI>().itemData = itemData;

            // if an empty slot cloned from a dropped item is filled, re-enable its raycast target
            if (!firstEmptySlot.itemObject.GetComponent<ItemUI>().image.raycastTarget)
            {
                firstEmptySlot.itemObject.GetComponent<ItemUI>().image.raycastTarget = true;
            }

            print($"{itemData.itemName} was added to {firstEmptySlot.name}!");
        }
    }
}
