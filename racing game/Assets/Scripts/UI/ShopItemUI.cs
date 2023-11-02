using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Image = UnityEngine.UI.Image;

public class ShopItemUI : MonoBehaviour
{
    public Image image;
    public TMP_Text itemStackSizeText;
    public ItemData itemData;

    public float itemStackSize = 1; // number of items in any given stack, different count for each instance of ItemUI objects
    public float itemCost;

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
