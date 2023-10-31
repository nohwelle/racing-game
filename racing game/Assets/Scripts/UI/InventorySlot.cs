using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public GameObject itemObject;
    public bool hasItemDroppedInto; // a check for if the itemUI object was actually placed in a slot or dropped

    public void OnDrop(PointerEventData eventData)
    {
        hasItemDroppedInto = true;

        // get info about the incoming item put into this slot
        GameObject dropped = eventData.pointerDrag;
        ItemUI draggableItem = dropped.GetComponent<ItemUI>();

        // if the possessed item and incoming item(s) for this slot are the same, combine them
        if (transform.childCount > 0)
        {
            if (transform.GetChild(0).GetComponent<ItemUI>().itemData == draggableItem.itemData)
            {
                // add to new item's stack; pretend like things are working fine
                draggableItem.itemStackSize += itemObject.GetComponent<ItemUI>().itemStackSize;
                draggableItem.itemStackSizeText.text = draggableItem.itemStackSize.ToString();
                print($"{draggableItem.name}'s stack size is now: {draggableItem.itemStackSize}!");

                // set current item in slot to empty; it will be overwritten by incoming item
                Destroy(transform.GetChild(0).gameObject);
                /*transform.GetChild(0).GetComponent<ItemUI>().itemData = null;
                transform.GetChild(0).gameObject.SetActive(false);*/

                print("Items moved in inventory are the same! Should be combined!");

                if (draggableItem.parentAfterDrag.childCount > 0 && draggableItem.parentAfterDrag.GetChild(0).GetComponent<ItemUI>().itemData != itemObject.GetComponent<ItemUI>().itemData)
                {
                    if (!draggableItem.parentAfterDrag.GetChild(0).gameObject.activeInHierarchy)
                    {
                        print($"{draggableItem.parentAfterDrag.name} still contains an empty!");
                    }
                    else
                    {
                        print($"{draggableItem.parentAfterDrag.name} still contains a different item!");
                    }
                }
            }
            else
            {
                if (!transform.GetChild(0).gameObject.activeInHierarchy)
                {
                    Destroy(transform.GetChild(0).gameObject);

                    print($"Destroying {gameObject.name}'s empty for a new incoming item!");
                }

                transform.GetChild(0).SetParent(draggableItem.parentAfterDrag);
                draggableItem.transform.SetParent(transform);

                print($"Swapping {gameObject.name}'s item for a new incoming item!");

            }
        }

        // set other slot's item object as its current child (item from this slot that was just swapped)
        draggableItem.parentAfterDrag.GetComponent<InventorySlot>().itemObject = draggableItem.parentAfterDrag.GetChild(0).gameObject;


        itemObject = draggableItem.gameObject;
        draggableItem.parentAfterDrag = transform;
    }

    private void Update()
    {
        if (transform.childCount == 0)
        {
            GameObject itemClone = Instantiate(itemObject);
            itemClone.name = itemClone.name.Replace("(Clone)", "").Trim();
            itemClone.GetComponent<ItemUI>().itemData = null;
            itemClone.GetComponent<ItemUI>().itemStackSize = 1;
            itemClone.SetActive(false);
            itemClone.transform.SetParent(transform);

            print($"Filling in {gameObject.name} with a new empty!");
        }

        if (transform.childCount > 0)
        {
            itemObject = transform.GetChild(0).gameObject;
        }
    }
}
