using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyDropper : MonoBehaviour
{
    public GameObject coinPrefab;
    public GameObject moneyDropperChute;

    public float moneyDispenseRate;
    bool dispenseNextMoney;


    // Start is called before the first frame update
    void Start()
    {
        dispenseNextMoney = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (dispenseNextMoney)
        {
            StartCoroutine(DispenseMoney());
        }
    }

    IEnumerator DispenseMoney()
    {
        dispenseNextMoney = false;

        yield return new WaitForSeconds(moneyDispenseRate);

        Instantiate(coinPrefab, moneyDropperChute.transform.position, Quaternion.identity);

        dispenseNextMoney = true;

        yield break;
    }
}
