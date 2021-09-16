using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotItemUpgrade : MonoBehaviour
{
    public int levelIndex;
    public string itemName;

    public Image panelLevelItem;
    public TextMeshProUGUI itemNameText;

    private void Update()
    {
        if (levelIndex == 0)
        {
            panelLevelItem.color = new Color(200,200,200,100);
        }
        else if (levelIndex == 1)
        {
            panelLevelItem.color = new Color(0,115,0,100);
        }
        else if (levelIndex == 2)
        {
            panelLevelItem.color = new Color(0, 0, 200, 100);
        }
        else if (levelIndex == 3)
        {
            panelLevelItem.color = new Color(250,150,0,100);
        }
        else if (levelIndex == 4)
        {
            panelLevelItem.color = new Color(140,0,250,100);
        }

        itemNameText.text = itemName;
    }
}
