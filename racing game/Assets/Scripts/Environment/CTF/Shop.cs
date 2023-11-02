using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class Shop : MonoBehaviour
{
    CTFPlayerMovement player;

    [HideInInspector] public int teamIdentity;

    public TMP_Text openShopText;
    public Transform openShopTextPosition;
    public GameObject shopUI;

    bool isShopUIOpen;

    // Start is called before the first frame update
    void Start()
    {
        openShopText.transform.position = openShopTextPosition.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (player && Input.GetKeyDown(player.useKey))
        {
            shopUI.SetActive(true);
            isShopUIOpen = true;
        }

        if (isShopUIOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            shopUI.SetActive(false);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<CTFRunner>() && collision.gameObject.GetComponent<CTFRunner>().teamIdentity == teamIdentity)
        {
            openShopText.text = "''<USE>'' TO OPEN SHOP";

            if (collision.gameObject.GetComponent<CTFPlayerMovement>())
            {
                player = collision.gameObject.GetComponent<CTFPlayerMovement>();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<CTFRunner>() && collision.gameObject.GetComponent<CTFRunner>().teamIdentity == teamIdentity)
        {
            openShopText.text = "";

            if (isShopUIOpen)
            {
                shopUI.SetActive(false);
            }

            player = null;
        }
    }
}
