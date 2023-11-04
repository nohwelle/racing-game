using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Shop : MonoBehaviour
{
    CTFPlayerMovement player;

    [HideInInspector] public int teamIdentity;

    public TMP_Text openShopText;
    public Transform openShopTextPosition;
    public GameObject shopUI;

    public bool isShopUIOpen;

    // Start is called before the first frame update
    void Start()
    {
        // just being used as a way to make updating the shop slots with CheckInventory more efficient
        shopUI.transform.hasChanged = false;

        openShopText.transform.position = openShopTextPosition.position;
    }

    // Update is called once per frame
    void Update()
    {
        // open the closed shop
        if (player && Input.GetKeyDown(player.useKey) && !isShopUIOpen)
        {
            shopUI.SetActive(true);
            shopUI.transform.hasChanged = true;
            isShopUIOpen = true;

            return;
        }

        if (shopUI.transform.hasChanged)
        {
            foreach (ShopSlotButton shopItem in FindObjectsOfType<ShopSlotButton>())
            {
                shopItem.CheckInventory();
            }

            shopUI.transform.hasChanged = false;
        }

        // close the opened shop
        if ((Input.GetKeyDown(KeyCode.Escape) || (player && Input.GetKeyDown(player.useKey))) && isShopUIOpen)
        {
            shopUI.SetActive(false);
            isShopUIOpen = false;

            return;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<CTFRunner>() && collision.gameObject.GetComponent<CTFRunner>().teamIdentity == teamIdentity)
        {
            if (collision.gameObject.GetComponent<CTFPlayerMovement>())
            {
                player = collision.gameObject.GetComponent<CTFPlayerMovement>();
            }

            openShopText.text = "''<USE>'' TO OPEN SHOP";
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<CTFRunner>() && collision.gameObject.GetComponent<CTFRunner>().teamIdentity == teamIdentity)
        {
            openShopText.text = "";

            if (shopUI && isShopUIOpen)
            {
                shopUI.SetActive(false);
                isShopUIOpen = false;
            }

            player = null;
        }
    }
}
