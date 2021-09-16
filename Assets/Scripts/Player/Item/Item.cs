using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int minChanceItemLevel;
    public int maxChanceItemLevel;

    public int powerOfItem;
    public string itemName;

    private SlotsUpgradeManager slotsUpgradeManager;

    public GameObject cylinderObject;

    private void Start()
    {
        powerOfItem = Random.Range(minChanceItemLevel, maxChanceItemLevel);
    }

    private void Update()
    {
        cylinderObject.transform.Rotate(Vector3.forward, Time.deltaTime * 20);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            slotsUpgradeManager = FindObjectOfType<SlotsUpgradeManager>();
            slotsUpgradeManager.AddCard(powerOfItem, itemName);
            Destroy(gameObject);
        }
    }
}
