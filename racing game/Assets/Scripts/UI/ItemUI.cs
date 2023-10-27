using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using static UnityEditor.Progress;

public class ItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform parentAfterDrag;
    Transform lastParentAfterDrag;
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
                itemStack.RemoveFromStack();
                
                if (itemStack.stackSize > 1)
                {
                    itemStackSizeText.text = itemStack.stackSize.ToString();
                }
                else
                {
                    itemStackSizeText.text = "";
                }

                if (itemStack.stackSize > 0)
                {
                    // make duplicate item UI object to stay in slot that item was removed from
                    GameObject itemClone = gameObject;
                    Instantiate(itemClone, transform.parent);
                }
            }
        }


        // set parent to root obj so item can be seen on top of inventory UI & place self at bottom of local hierarchy
        parentAfterDrag = transform.parent;
        lastParentAfterDrag = parentAfterDrag;
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
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
    }

    private void Update()
    {
        if (itemData && !image.sprite)
        {
            image.sprite = itemData.itemSprite;
        }
    }
}
