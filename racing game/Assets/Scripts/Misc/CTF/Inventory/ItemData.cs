using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemData : ScriptableObject
{
    public string itemID;
    public string itemName;
    public string itemDescription;
    public Sprite itemSprite;
    public GameObject itemPrefab;
}
