using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Image = UnityEngine.UI.Image;
using static UnityEditor.Progress;

public class ItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform parentAfterDrag;
    public Image image;
    public TMP_Text itemStackSizeText;
    public ItemData itemData;

    public void OnBeginDrag(PointerEventData eventData)
    {
        // move one copy of item in stack on left-click
        if (Input.GetMouseButton(0)) // left
        {
            // mess with stack counts
            if (InventorySystem.Instance.itemDictionary.TryGetValue(itemData, out Item itemStack))
            {
                if (itemStack.stackSize > 1)
                {
                    itemStack.RemoveFromStack();
                    itemStackSizeText.text = itemStack.stackSize.ToString();
                    print($"{itemStack.itemData.itemName}'s total stack is now: {itemStack.stackSize}!");

                    // make duplicate item UI object to stay in slot that item was removed from
                    GameObject itemClone = Instantiate(gameObject, transform.parent);
                    itemClone.name = itemClone.name.Replace("(Clone)", "").Trim();

                    // set removed item's stack size to one, since it's been removed separately
                    // -- ISSUE: itemStack affects all itemDict values of itemData, so multiple copies of the same item get affected by this
                    itemStack.stackSize = 1;
                }

                if (itemStack.stackSize == 1)
                {
                    itemStackSizeText.text = "";
                    print($"{itemStack.itemData.itemName}'s total stack is 1!");
                }
            }
        }


        // set parent to root obj so item can be seen on top of inventory UI & place self at bottom of local hierarchy
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // follow mouse
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (parentAfterDrag.GetComponent<InventorySlot>().hasItemDroppedInto)
        {
            // get placed in new slot
            transform.SetParent(parentAfterDrag);
            image.raycastTarget = true;

            parentAfterDrag.GetComponent<InventorySlot>().hasItemDroppedInto = false;
        }
        else
        {
            if (InventorySystem.Instance.itemDictionary.TryGetValue(itemData, out Item itemStack))
            {
                // drop however many items were in the stack
                // figure out whose inventory we're actually in
                if (InventorySystem.Instance.inventorySlots.Contains(parentAfterDrag.GetComponent<InventorySlot>()))
                {
                    print(InventorySystem.Instance.transform.root);
                    for (var i = 0; i < itemStack.stackSize; i++)
                    {
                        GameObject droppedItem = itemData.itemPrefab;
                        Instantiate(droppedItem, InventorySystem.Instance.transform.root);
                    }
                }
            }
        }
    }

    private void Update()
    {
        if (itemData && !image.sprite)
        {
            image.sprite = itemData.itemSprite;
        }

        if (!itemData && image.sprite)
        {
            image.sprite = null;
        }
    }
}
