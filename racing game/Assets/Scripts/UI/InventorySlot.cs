using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text stackLabelText;

    public bool containsItem;

    public static InventorySlot Instance;

    public void OnDrop(PointerEventData eventData)
    {
        // place item in new inventory slot (this one) if empty
        containsItem = true;
        GameObject dropped = eventData.pointerDrag;
        ItemUI draggableItem = dropped.GetComponent<ItemUI>();

        if (transform.childCount == 1)
        {
            transform.GetChild(0).SetParent(draggableItem.parentAfterDrag);
        }

        draggableItem.parentAfterDrag = transform;

    }

    public void Set(Item item)
    {
        if (item == GetComponentInChildren<Item>())
        {
            itemNameText.text = item.data.itemDisplayName;

            if (item.stackSize > 1)
            {
                stackLabelText.text = item.stackSize.ToString();
            }
        }
    }

    public void Get(Item item)
    {
        if (item == GetComponentInChildren<Item>() && item.stackSize > 1)
        {
            stackLabelText.text = item.stackSize.ToString();
        }

        if (item == GetComponentInChildren<Item>() && item.stackSize <= 1)
        {
            stackLabelText.text = "";
        }
    }

    void Awake()
    {
        Instance = this;
    }
}
