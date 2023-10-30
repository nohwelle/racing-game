using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Image = UnityEngine.UI.Image;

public class ItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform parentAfterDrag;
    public Image image;
    public TMP_Text itemStackSizeText;
    public ItemData itemData;

    public float itemStackSize = 1; // number of items in any given stack, different count for each instance of ItemUI objects

    public void OnBeginDrag(PointerEventData eventData)
    {
        // move one copy of item in stack on left-click
        if (Input.GetMouseButton(0)) // left
        {
            print("Item is being dragged!");

            // mess with stack counts
            if (itemStackSize > 1)
            {
                // set stack size for the cloned item
                itemStackSize--;
                itemStackSizeText.text = itemStackSize.ToString();

                // make duplicate item UI object to stay in slot that item was removed from
                GameObject itemClone = Instantiate(gameObject, transform.parent);
                itemClone.name = itemClone.name.Replace("(Clone)", "").Trim();

                if (itemClone.GetComponent<ItemUI>().itemStackSize == 1)
                {
                    itemClone.GetComponent<ItemUI>().itemStackSizeText.text = "";
                }

                // set stack size for self after separating from the full stack
                itemStackSize = 1;
                itemStackSizeText.text = "";

                print($"Item removed from stack! Remaining {itemData.itemName} stack is now: {itemStackSize}!");
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
            parentAfterDrag.GetComponent<InventorySlot>().itemObject = gameObject;
            image.raycastTarget = true;

            parentAfterDrag.GetComponent<InventorySlot>().hasItemDroppedInto = false;

            if (Input.GetMouseButtonUp(0))
            {
                itemStackSize = 1;
            }
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
                        itemClone.GetComponent<ItemUI>().itemStackSize = 1;
                        itemClone.SetActive(false);
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
                // figure out whose inventory we're actually in
                if (InventorySystem.Instance.inventorySlots.Contains(parentAfterDrag.GetComponent<InventorySlot>()))
                {
                    for (var i = 0; i < itemStackSize; i++)
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

                        // create empty item to replace dropped itemUI after all items have been dropped
                        if (i == itemStackSize - 1)
                        {
                            GameObject itemClone = Instantiate(gameObject, transform.parent);
                            itemClone.name = itemClone.name.Replace("(Clone)", "").Trim();
                            itemClone.transform.SetParent(parentAfterDrag);
                            parentAfterDrag.GetComponent<InventorySlot>().itemObject = itemClone;
                            itemClone.GetComponent<ItemUI>().itemData = null;
                            itemClone.GetComponent<ItemUI>().itemStackSize = 1;
                            itemClone.SetActive(false);

                            Destroy(gameObject);
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
    }

    private void OnDisable()
    {
        if (!itemData && image.sprite)
        {
            image.sprite = null;
        }
    }
}
