using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Transform parentAfterDrag;
    public Image image;
    public TMP_Text itemStackSizeText;
    public ItemData itemData;

    public void OnBeginDrag(PointerEventData eventData)
    {
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
        if (parentAfterDrag)
        {
            // get placed in new slot
            transform.SetParent(parentAfterDrag);
            image.raycastTarget = true;
        }
        else
        {
            // remove item (self) from inventory system & display
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (itemData && !image.sprite)
        {
            image.sprite = itemData.itemSprite;
        }

        if (!itemData)
        {
            image.sprite = null;
        }
    }
}
