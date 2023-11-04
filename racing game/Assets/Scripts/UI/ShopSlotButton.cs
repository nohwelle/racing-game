using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlotButton : MonoBehaviour
{
    Button button;

    bool itemWasPurchased;

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

        foreach (ShopSlotButton shopItem in FindObjectsOfType<ShopSlotButton>())
        {
            // if it's less than the cost of this slot's item, disable the button
            if (totalCoinsInSlots >= shopItem.transform.GetChild(0).GetComponent<ShopItemUI>().itemCost)
            {
                shopItem.button.interactable = true;
            }
            else
            {
                shopItem.button.interactable = false;
            }
        }
    }

    void AddItemToInventory()
    {
        List<InventorySlot> slotsWithCoins = new();
        float totalCoinsInSlots = 0;
        float itemCostRemainder = transform.GetChild(0).GetComponent<ShopItemUI>().itemCost;

        foreach (InventorySlot slot in InventorySystem.Instance.inventorySlots)
        {
            // how many coins does the player have in their inventory?
            if (slot.transform.childCount > 0 && slot.transform.GetChild(0).GetComponent<ItemUI>().itemData && slot.transform.GetChild(0).GetComponent<ItemUI>().itemData.itemID == 1)
            {
                // save this slot to a list of potential candidates to remove coins from first
                slotsWithCoins.Add(slot);

                // add amount of coins between the slots together
                totalCoinsInSlots += slot.transform.GetChild(0).GetComponent<ItemUI>().itemStackSize;

                print($"{slot.name} detects " + totalCoinsInSlots + " coins in inventory!");

                // if this inventory slot has more coins than / equal coins to the cost of this shop slot's item, let's buy it!
                if (slot.transform.GetChild(0).GetComponent<ItemUI>().itemStackSize >= transform.GetChild(0).GetComponent<ShopItemUI>().itemCost)
                {
                    // deduct money from inventory slot
                    slot.transform.GetChild(0).GetComponent<ItemUI>().itemStackSize -= transform.GetChild(0).GetComponent<ShopItemUI>().itemCost;
                    slot.transform.GetChild(0).GetComponent<ItemUI>().itemStackSizeText.text = slot.transform.GetChild(0).GetComponent<ItemUI>().itemStackSize.ToString();

                    // add item to inventory on click
                    InventorySystem.Instance.Add(transform.GetChild(0).GetComponent<ShopItemUI>().itemData);

                    itemWasPurchased = true;
                    print($"Purchased {transform.GetChild(0).GetComponent<ShopItemUI>().itemData.itemName} for {transform.GetChild(0).GetComponent<ShopItemUI>().itemCost}!");

                    break;
                }
            }
        }

        // if a single slot does not have enough money in it to buy an item, but the total coins in all slots can, remove coins from slots in order until cost is covered
        if (totalCoinsInSlots >= transform.GetChild(0).GetComponent<ShopItemUI>().itemCost && !itemWasPurchased)
        {
            foreach (InventorySlot slot in slotsWithCoins)
            {
                // if a saved slot has (more than) enough coins to cover the cost, deduct money like normal
                if (slot.transform.GetChild(0).GetComponent<ItemUI>().itemStackSize - itemCostRemainder > 0)
                {
                    // do the opposite of above so itemCostRemainder = 0 and slot's coin coin is >= 1 by the end
                    slot.transform.GetChild(0).GetComponent<ItemUI>().itemStackSize -= itemCostRemainder;
                    itemCostRemainder -= slot.transform.GetChild(0).GetComponent<ItemUI>().itemStackSize - (slot.transform.GetChild(0).GetComponent<ItemUI>().itemStackSize - itemCostRemainder);

                    print(itemCostRemainder);
                    print($"Cost of item remaining after coins were deducted from {slot.name}: " + itemCostRemainder);

                    break;
                }

                // if a saved slot has too few coins in it to cover the cost, just pay off whatever amount it has
                if (slot.transform.GetChild(0).GetComponent<ItemUI>().itemStackSize - itemCostRemainder <= 0)
                {
                    // -- EX. 3 - (5 - (5 - 3)) = 0
                    itemCostRemainder -= slot.transform.GetChild(0).GetComponent<ItemUI>().itemStackSize;
                    slot.transform.GetChild(0).GetComponent<ItemUI>().itemStackSize -= itemCostRemainder - (itemCostRemainder - slot.transform.GetChild(0).GetComponent<ItemUI>().itemStackSize);

                    print(itemCostRemainder);
                    print($"Cost of item remaining after coins were deducted from {slot.name}: " + itemCostRemainder);
                }
            }

            if (itemCostRemainder == 0)
            {
                // -- NOTE: bought item will occupy "next" available slot because coins don't get destroyed before the bought item is added, so the slot is still technically occupied. should be changed
                // add item to inventory on click
                InventorySystem.Instance.Add(transform.GetChild(0).GetComponent<ShopItemUI>().itemData);

                print($"Purchased {transform.GetChild(0).GetComponent<ShopItemUI>().itemData.itemName} for {transform.GetChild(0).GetComponent<ShopItemUI>().itemCost}!");
            }
        }

        // update how many coins are in inventory after all is said and done
        CheckInventory();
        itemWasPurchased = false;
    }
}
