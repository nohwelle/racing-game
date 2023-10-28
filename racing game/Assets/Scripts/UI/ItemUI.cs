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

    public bool isItemStackUI;

    public void OnBeginDrag(PointerEventData eventData)
    {
        // move one copy of item in stack on left-click
        if (Input.GetMouseButton(0)) // left
        {
            print("Item is being dragged!");

            // mess with stack counts
            if (InventorySystem.Instance.itemDictionary.TryGetValue(itemData, out Item itemStack))
            {
                // remove 1 from stack count and create new instance of item UI to represent the remaining stack
                if (itemStack.stackSize > 1)
                {
                    // -- BUG: leftover item stack does not keep count of items correctly, gets stack size set to 1 despite check put in place

                    itemStack.RemoveFromStack();
                    itemStackSizeText.text = itemStack.stackSize.ToString();

                    // make duplicate item UI object to stay in slot that item was removed from
                    GameObject itemClone = Instantiate(gameObject, transform.parent);
                    itemClone.name = itemClone.name.Replace("(Clone)", "").Trim();
                    itemClone.GetComponent<ItemUI>().isItemStackUI = true;

                    // set removed item's stack size to one
                    if (!isItemStackUI)
                    {
                        print($"Item removed from stack! Remaining {itemStack.itemData.itemName} stack is now: {itemStack.stackSize}!");
                        itemStack.stackSize = 1;
                    }

                    // if there is only one item left in the stack after an item was removed, clear stack text
                    if (itemStack.stackSize == 1)
                    {
                        itemStackSizeText.text = "";
                    }
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
        // get placed in new slot
        if (parentAfterDrag.GetComponent<InventorySlot>().hasItemDroppedInto)
        {
            transform.SetParent(parentAfterDrag);
            image.raycastTarget = true;

            parentAfterDrag.GetComponent<InventorySlot>().hasItemDroppedInto = false;
        }
        else
        {
            // if left-click dragging, drop one item
            if (Input.GetMouseButtonUp(0))
            {
                // figure out whose inventory we're actually in
                if (InventorySystem.Instance.inventorySlots.Contains(parentAfterDrag.GetComponent<InventorySlot>()))
                {
                    GameObject droppedItem = itemData.itemPrefab;
                    Instantiate(droppedItem, InventorySystem.Instance.transform.root);

                    if (InventorySystem.Instance.transform.root.gameObject.GetComponent<CTFPlayerMovement>() && InventorySystem.Instance.transform.root.gameObject.GetComponent<CTFPlayerMovement>().isFacingLeft)
                    {
                        print("Fling item away to the left!");
                        droppedItem.GetComponent<Rigidbody2D>().AddForce(new(-50, 0), ForceMode2D.Impulse);
                    }
                    if (InventorySystem.Instance.transform.root.gameObject.GetComponent<CTFPlayerMovement>() && !InventorySystem.Instance.transform.root.gameObject.GetComponent<CTFPlayerMovement>().isFacingLeft)
                    {
                        print("Fling item away to the right!");
                        droppedItem.GetComponent<Rigidbody2D>().AddForce(new(50, 0), ForceMode2D.Impulse);
                    }

                    // create empty item to replace dropped itemUI if dropping the only item left in a slot
                    if (parentAfterDrag.childCount == 0)
                    {
                        GameObject itemClone = Instantiate(gameObject, transform.parent);
                        itemClone.name = itemClone.name.Replace("(Clone)", "").Trim();
                        itemClone.transform.SetParent(parentAfterDrag);
                        parentAfterDrag.GetComponent<InventorySlot>().itemObject = itemClone;
                        itemClone.GetComponent<ItemUI>().itemData = null;
                        itemClone.SetActive(false);

                        // check all other slots for references to the dropped item before removing it from the item log
                        bool removeItemInfoFromInventory = true;

                        foreach (InventorySlot slot in InventorySystem.Instance.inventorySlots)
                        {
                            if (slot.itemObject.GetComponent<ItemUI>().itemData == itemData)
                            {
                                removeItemInfoFromInventory = false;
                            }
                        }

                        if (removeItemInfoFromInventory)
                        {
                            InventorySystem.Instance.Remove(itemData);
                        }
                    }
                    else // if there are still items in the slot & stack, set slot's itemObject to remaining stack object
                    {
                        parentAfterDrag.GetComponent<InventorySlot>().itemObject = parentAfterDrag.GetChild(0).gameObject;
                    }

                    Destroy(gameObject);
                }
            }

            // if right-click dragging, drop however many items were in the stack
            if (Input.GetMouseButtonUp(1))
            {
                if (InventorySystem.Instance.itemDictionary.TryGetValue(itemData, out Item itemStack))
                {
                    // figure out whose inventory we're actually in
                    if (InventorySystem.Instance.inventorySlots.Contains(parentAfterDrag.GetComponent<InventorySlot>()))
                    {
                        for (var i = 0; i < itemStack.stackSize; i++)
                        {
                            GameObject droppedItem = itemData.itemPrefab;
                            Instantiate(droppedItem, InventorySystem.Instance.transform.root);

                            if (InventorySystem.Instance.transform.root.gameObject.GetComponent<CTFPlayerMovement>() && InventorySystem.Instance.transform.root.gameObject.GetComponent<CTFPlayerMovement>().isFacingLeft)
                            {
                                print("Fling item away to the left!");
                                droppedItem.GetComponent<Rigidbody2D>().AddForce(new(-50, 0), ForceMode2D.Impulse);
                            }
                            if (InventorySystem.Instance.transform.root.gameObject.GetComponent<CTFPlayerMovement>() && !InventorySystem.Instance.transform.root.gameObject.GetComponent<CTFPlayerMovement>().isFacingLeft)
                            {
                                print("Fling item away to the right!");
                                droppedItem.GetComponent<Rigidbody2D>().AddForce(new(50, 0), ForceMode2D.Impulse);
                            }

                            if (i == itemStack.stackSize - 1)
                            {
                                GameObject itemClone = Instantiate(gameObject, transform.parent);
                                itemClone.name = itemClone.name.Replace("(Clone)", "").Trim();
                                itemClone.transform.SetParent(parentAfterDrag);
                                parentAfterDrag.GetComponent<InventorySlot>().itemObject = itemClone;
                                itemClone.GetComponent<ItemUI>().itemData = null;
                                itemClone.SetActive(false);

                                bool removeItemInfoFromInventory = true;

                                foreach (InventorySlot slot in InventorySystem.Instance.inventorySlots)
                                {
                                    if (slot.itemObject.GetComponent<ItemUI>().itemData == itemData)
                                    {
                                        removeItemInfoFromInventory = false;
                                    }
                                }

                                if (removeItemInfoFromInventory)
                                {
                                    InventorySystem.Instance.Remove(itemData);
                                }

                                Destroy(gameObject);
                            }
                        }
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
