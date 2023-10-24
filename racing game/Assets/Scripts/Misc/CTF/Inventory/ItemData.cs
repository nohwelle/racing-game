using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : MonoBehaviour
{
    public string itemID;
    public string itemDisplayName;
    public Sprite icon;
    public GameObject itemPrefab;

    private void Awake()
    {
        itemDisplayName = name;
        icon = GetComponent<SpriteRenderer>().sprite;
        itemPrefab = gameObject;
    }
}
