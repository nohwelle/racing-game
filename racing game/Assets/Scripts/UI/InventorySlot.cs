using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public GameObject itemObject;
    public bool hasItemDroppedInto;

    public void OnDrop(PointerEventData eventData)
    {
        hasItemDroppedInto = true;

        // get info about the new thing put into this slot
        GameObject dropped = eventData.pointerDrag;
        ItemUI draggableItem = dropped.GetComponent<ItemUI>();

        // if two items involved in interaction are the same, combine them
        if (transform.childCount != 0 && transform.GetChild(0).GetComponent<ItemUI>().itemData == draggableItem.itemData)
        {
            print("Items moved in inventory are the same! Should be combined!");
            // set current item in slot to empty
            transform.GetChild(0).GetComponent<ItemUI>().itemData = null;
            transform.GetChild(0).gameObject.SetActive(false);

            // add to new item's stack; pretend like things are working fine
            if (InventorySystem.Instance.itemDictionary.TryGetValue(draggableItem.GetComponent<ItemUI>().itemData, out Item itemStack))
            {
                itemStack.AddToStack();
                draggableItem.itemStackSizeText.text = itemStack.stackSize.ToString();
            }
        }

        // if old slot contains a real item, destroy empty from this slot
        if (draggableItem.parentAfterDrag.childCount != 0 && draggableItem.parentAfterDrag.GetChild(0).gameObject.activeInHierarchy)
        {
            Destroy(transform.GetChild(0).gameObject);
        }

        // swap current item (empty)'s position with position of new item
        if (itemObject)
        {
            itemObject.transform.SetParent(draggableItem.parentAfterDrag);
        }

        // set other slot's item object as its current child
        draggableItem.parentAfterDrag.GetComponent<InventorySlot>().itemObject = draggableItem.parentAfterDrag.GetChild(0).gameObject;

        itemObject = draggableItem.gameObject;
        draggableItem.parentAfterDrag = transform;
    }
}
