using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;
using static UnityEditor.Progress;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public GameObject itemObject;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        ItemUI draggableItem = dropped.GetComponent<ItemUI>();

        itemObject.transform.SetParent(draggableItem.parentAfterDrag);
        draggableItem.parentAfterDrag.GetComponent<InventorySlot>().itemObject = itemObject;
        itemObject = draggableItem.gameObject;

        draggableItem.parentAfterDrag = transform;
    }
}
