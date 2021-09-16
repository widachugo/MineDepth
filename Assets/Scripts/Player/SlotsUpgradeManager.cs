using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotsUpgradeManager : MonoBehaviour
{
    private RectTransform rectTransformContent;

    public GameObject contentWeaponUpgrade;

    public GameObject contentCanvas;

    public List<GameObject> itemsList;

    private void Start()
    {
        rectTransformContent = contentCanvas.GetComponent<RectTransform>();
    }

    public void AddCard(int levelItem, string itemName)
    {
        GameObject goItem = Instantiate(contentWeaponUpgrade);
        goItem.transform.parent = contentCanvas.transform;

        SlotItemUpgrade slotItemUpgrade = goItem.GetComponent<SlotItemUpgrade>();
        slotItemUpgrade.levelIndex = levelItem;
        slotItemUpgrade.itemName = itemName;

        itemsList.Add(goItem);
    }

    public void RemoveCard()
    {
        int indexRemove = Random.Range(0, itemsList.Count);
        Destroy(itemsList[indexRemove]);
        itemsList.RemoveAt(indexRemove);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RemoveCard();
        }

        for (int i = 0; i < itemsList.Count; i++)
        {
            RectTransform rectTransformItem = itemsList[i].GetComponent<RectTransform>();

            if (i == 0)
            {
                rectTransformItem.localPosition = new Vector3(300, -60, 0);
            }
            else
            {
                rectTransformItem.localPosition = new Vector3(300, itemsList[i - 1].transform.localPosition.y - 105, 0);
            }

            rectTransformItem.localScale = new Vector3(1, 1, 1);
        }

        rectTransformContent.sizeDelta = new Vector2(0, 105 * itemsList.Count);
    }
}
