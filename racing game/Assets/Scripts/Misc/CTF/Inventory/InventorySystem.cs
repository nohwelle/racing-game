using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    public List<Item> inventory = new();
    public List<InventorySlot> inventorySlots = new();
    private Dictionary<ItemData, Item> itemDictionary = new();

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
            if (itemDictionary.TryGetValue(itemData, out Item item))
            {
                if (slot.itemObject.activeInHierarchy && slot.itemObject.GetComponent<ItemUI>().itemData == itemData)
                {
                    item.AddToStack();

                    print($"{item.itemData.itemName}'s total stack is now: {item.stackSize}!");

                    if (item.stackSize > 1)
                    {
                        slot.itemObject.GetComponent<ItemUI>().itemStackSizeText.text = item.stackSize.ToString();
                    }

                    break;
                }
            }
            else
            {
                if (!slot.itemObject.activeInHierarchy)
                {
                    Item newItem = new(itemData);
                    inventory.Add(newItem);
                    itemDictionary.Add(itemData, newItem);

                    print($"{itemData.itemName} was added to {slot.name}!");

                    // enable slot's child and give it the new item's data
                    slot.itemObject.SetActive(true);
                    slot.itemObject.GetComponent<ItemUI>().itemData = itemData;

                    break;
                }
            }
        }
    }

    public void Remove(ItemData itemData)
    {
        if (itemDictionary.TryGetValue(itemData, out Item item))
        {
            item.RemoveFromStack();

            if (item.stackSize == 0)
            {
                inventory.Remove(item);
                itemDictionary.Remove(itemData);
            }
        }
    }
}
