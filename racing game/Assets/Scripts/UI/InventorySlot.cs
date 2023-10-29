using System.Collections;
using System.Collections.Generic;
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
        if (transform.childCount != 0 && transform.GetChild(0).GetComponent<ItemUI>().itemData == draggableItem.itemData)
        {
            // add to new item's stack; pretend like things are working fine
            draggableItem.itemStackSize += itemObject.GetComponent<ItemUI>().itemStackSize;
            draggableItem.itemStackSizeText.text = draggableItem.itemStackSize.ToString();

            // set current item in slot to empty; it will be overwritten by incoming item
            transform.GetChild(0).GetComponent<ItemUI>().itemData = null;
            transform.GetChild(0).gameObject.SetActive(false);

            print("Items moved in inventory are the same! Should be combined!");
        }

        // if old slot contains a real item, destroy empty from this slot if there is one
        if (transform.childCount > 0 && !transform.GetChild(0).gameObject.activeInHierarchy && draggableItem.parentAfterDrag.childCount > 0 && draggableItem.parentAfterDrag.GetChild(0).gameObject.activeInHierarchy)
        {
            Destroy(transform.GetChild(0).gameObject);
        }

        // swap current item's position with position of new item
        if (itemObject)
        {
            // if slot being swapped into still contains an item, and said item is the same as incoming item for this slot, combine the objects
            if (itemObject.activeInHierarchy && draggableItem.parentAfterDrag.childCount > 0 && draggableItem.parentAfterDrag.GetChild(0).GetComponent<ItemUI>().itemData == draggableItem.itemData)
            {
                draggableItem.itemStackSize += draggableItem.parentAfterDrag.GetChild(0).GetComponent<ItemUI>().itemStackSize;
                draggableItem.itemStackSizeText.text = draggableItem.itemStackSize.ToString();
                Destroy(draggableItem.parentAfterDrag.GetChild(0).gameObject);

                print(draggableItem.parentAfterDrag.GetComponent<InventorySlot>().itemObject + " + " + itemObject);
            }

            // -- BUG: cool unsolvable bug? item stack gets destroyed as item from other slot moves around; item from other slot replaces item stack in that slot; slot never restores itemObject
            // despite explicitly being told to do so in code; item from that slot cannot be dragged out and placed back in, else crash
            itemObject.transform.SetParent(draggableItem.parentAfterDrag);
        }

        // set other slot's item object as its current child
        draggableItem.parentAfterDrag.GetComponent<InventorySlot>().itemObject = draggableItem.parentAfterDrag.GetChild(0).gameObject;


        itemObject = draggableItem.gameObject;
        draggableItem.parentAfterDrag = transform;
    }
}
