using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlotButton : MonoBehaviour
{
    Button button;

    public float itemStackSize = 1; // number of items in any given stack, different count for each instance of ItemUI objects

    public static ShopSlotButton Instance;

    private void Awake()
    {
        Instance = this;
        button = GetComponent<Button>();
    }

    private void Start()
    {
        button.onClick.AddListener(AddItemToInventory);
        button.interactable = false;
    }

    // -- VERY LIKELY THAT ALL vvv IS BROKEN LOL

    public void CheckInventory()
    {
        float totalCoinsInSlots = 0;

        foreach (InventorySlot slot in InventorySystem.Instance.inventorySlots)
        {
            // how many coins does the player have in their inventory?
            if (slot.transform.childCount > 0 && slot.transform.GetChild(0).GetComponent<ItemUI>().itemData && slot.transform.GetChild(0).GetComponent<ItemUI>().itemData.itemID == 1)
            {
                // add amount of coins between the slots together
                totalCoinsInSlots += slot.transform.GetChild(0).GetComponent<ItemUI>().itemStackSize;
            }
        }

        // if it's less than the cost of this slot's item, disable the button
        if (totalCoinsInSlots >= transform.GetChild(0).GetComponent<ShopItemUI>().itemCost)
        {
            button.interactable = true;
        }
        else
        {
            button.interactable = false;
        }
    }

    void AddItemToInventory()
    {
        List<InventorySlot> slotsWithCoins = new();
        float totalCoinsInSlots = 0;

        foreach (InventorySlot slot in InventorySystem.Instance.inventorySlots)
        {
            // how many coins does the player have in their inventory?
            if (slot.transform.childCount > 0 && slot.transform.GetChild(0).GetComponent<ItemUI>().itemData && slot.transform.GetChild(0).GetComponent<ItemUI>().itemData.itemID == 1)
            {
                // save this slot to a list of potential candidates to remove coins from first
                slotsWithCoins.Add(slot);

                // add amount of coins between the slots together
                totalCoinsInSlots += slot.transform.GetChild(0).GetComponent<ItemUI>().itemStackSize;


                // if this inventory slot has more coins than / equal coins to the cost of this shop slot's item, let's buy it!
                if (slot.transform.GetChild(0).GetComponent<ItemUI>().itemStackSize >= transform.GetChild(0).GetComponent<ShopItemUI>().itemCost)
                {
                    // add item to inventory on click
                    InventorySystem.Instance.Add(transform.GetChild(0).GetComponent<ShopItemUI>().itemData);

                    // deduct money from inventory slot
                    slot.transform.GetChild(0).GetComponent<ItemUI>().itemStackSize -= transform.GetChild(0).GetComponent<ShopItemUI>().itemCost;
                    slot.transform.GetChild(0).GetComponent<ItemUI>().itemStackSizeText.text = slot.transform.GetChild(0).GetComponent<ItemUI>().itemStackSize.ToString();

                    break;
                }
            }
        }

        // if a single slot does not have enough money in it to buy an item, but the total coins in all slots can, remove coins from slots in order until cost is covered
        if (totalCoinsInSlots >= transform.GetChild(0).GetComponent<ShopItemUI>().itemCost)
        {
            float itemCostRemainder = transform.GetChild(0).GetComponent<ShopItemUI>().itemCost;

            foreach (InventorySlot slot in slotsWithCoins)
            {
                // if a saved slot has too few coins in it to cover the cost, just pay off whatever amount it has
                if (slot.transform.GetChild(0).GetComponent<ItemUI>().itemStackSize - itemCostRemainder <= 0)
                {
                    // -- EX. 3 - (5 - (5 - 3)) = 0
                    slot.transform.GetChild(0).GetComponent<ItemUI>().itemStackSize -= itemCostRemainder - (itemCostRemainder - slot.transform.GetChild(0).GetComponent<ItemUI>().itemStackSize);
                    itemCostRemainder -= slot.transform.GetChild(0).GetComponent<ItemUI>().itemStackSize;
                }

                // if a saved slot has (more than) enough coins to cover the cost, deduct money like normal
                if (slot.transform.GetChild(0).GetComponent<ItemUI>().itemStackSize - itemCostRemainder > 0)
                {
                    slot.transform.GetChild(0).GetComponent<ItemUI>().itemStackSize -= itemCostRemainder;
                }
            }
        }
    }
}
