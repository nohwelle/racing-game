using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlotButton : MonoBehaviour
{
    Button button;

    public float itemStackSize = 1; // number of items in any given stack, different count for each instance of ItemUI objects

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        button.onClick.AddListener(AddItemToInventory);
    }

    void AddItemToInventory()
    {
        foreach (InventorySlot slot in InventorySystem.Instance.inventorySlots)
        {
            // how many coins does the player have in their inventory?
            if (slot.transform.childCount > 0 && slot.transform.GetChild(0).GetComponent<ItemUI>().itemData && slot.transform.GetChild(0).GetComponent<ItemUI>().itemData.itemID == 1)
            {
                // if it's more than or equal to the cost of this slot's item, let's buy it!
                if (slot.transform.GetChild(0).GetComponent<ItemUI>().itemStackSize >= transform.GetChild(0).GetComponent<ShopItemUI>().itemCost)
                {
                    // add item to inventory on click
                    InventorySystem.Instance.Add(transform.GetChild(0).GetComponent<ShopItemUI>().itemData);

                    // deduct money from inventory
                    slot.transform.GetChild(0).GetComponent<ItemUI>().itemStackSize -= transform.GetChild(0).GetComponent<ShopItemUI>().itemCost;
                    slot.transform.GetChild(0).GetComponent<ItemUI>().itemStackSizeText.text = slot.transform.GetChild(0).GetComponent<ItemUI>().itemStackSize.ToString();
                }
            }
        }
    }
}
