using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Shop : MonoBehaviour
{
    [HideInInspector] public int teamIdentity;

    public TMP_Text openShopText;
    public Transform openShopTextPosition;

    // Start is called before the first frame update
    void Start()
    {
        openShopText.transform.position = openShopTextPosition.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<CTFRunner>() && collision.gameObject.GetComponent<CTFRunner>().teamIdentity == teamIdentity)
        {
            openShopText.text = "''<USE>'' TO OPEN SHOP";
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<CTFRunner>() && collision.gameObject.GetComponent<CTFRunner>().teamIdentity == teamIdentity)
        {
            openShopText.text = "";
        }
    }
}
