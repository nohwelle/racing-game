using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public GameObject itemObject;

    public void OnDrop(PointerEventData eventData)
    {
        // get info about the new thing put into this slot
        GameObject dropped = eventData.pointerDrag;
        ItemUI draggableItem = dropped.GetComponent<ItemUI>();

        // if old slot contains a real item, destroy empty from this slot
        if (draggableItem.parentAfterDrag.childCount > 0 && draggableItem.parentAfterDrag.GetChild(0).gameObject.activeInHierarchy)
        {
            Destroy(transform.GetChild(0).gameObject);

            // if real item is the same as item being moved, turn one instance into an empty and add to other's stack
            if (draggableItem.itemData == itemObject.GetComponent<ItemUI>().itemData)
            {

            }
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
