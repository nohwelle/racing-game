using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Image image;
    [HideInInspector] public Transform parentAfterDrag;

    private void Awake()
    {
        image.sprite = GetComponent<SpriteRenderer>().sprite;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (GetComponent<Item>().stackSize > 1)
        {
            parentAfterDrag = transform;
        }

        // set parent to root obj so item can be seen on top of inventory UI & place self at bottom of local hierarchy
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
            InventorySystem.Instance.Remove(GetComponent<Item>().data);
            Destroy(gameObject);
        }
    }
}
