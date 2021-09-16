using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItems : MonoBehaviour
{
    public bool spawnUpgradeItem;

    public GameObject itemUpgrade;
    public GameObject goldMoney;

    [Range(0.0f, 4.0f)]
    public int minChanceLoot;
    [Range(0.0f, 4.0f)]
    public int maxChanceLoot;

    private bool OneTime;

    public void OnDeath()
    {
        if (!OneTime)
        {
            if (spawnUpgradeItem)
            {
                GameObject itemGo = Instantiate(itemUpgrade);
                itemGo.transform.position = transform.position;

                Item item = itemGo.GetComponent<Item>();
                item.minChanceItemLevel = minChanceLoot;
                item.maxChanceItemLevel = maxChanceLoot;
                item.itemName = gameObject.name + " Crystal";
            }

            OneTime = true;

            for (int i = 0; i < 10; i++)
            {
                GameObject goldMoneyGo = Instantiate(goldMoney);
                goldMoney.transform.position = transform.position;

                Rigidbody goldMoneyRb = goldMoneyGo.GetComponent<Rigidbody>();
                goldMoneyRb.AddForceAtPosition((Random.insideUnitSphere * 5) * 1000, transform.position);
            }
        }
    }
}
