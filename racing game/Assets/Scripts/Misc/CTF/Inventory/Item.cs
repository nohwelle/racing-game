using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item
{
    public ItemData itemData;

    public Item(ItemData item)
    {
        itemData = item;
    }
}
