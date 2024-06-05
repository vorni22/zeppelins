using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyZepDetails : MonoBehaviour
{
    public int ZepId, slotID, inListID;
    public GameManager home;

    public Text Name, stats, rating, upgradeCost, level;
    public Button replaceButton, upgradeButton;
    public Image icon;

    void Start()
    {
        UpdateThis();
    }

    public void UpdateThis()
    {
        Name.text = home.zeppelins[ZepId].Name;
        icon.sprite = home.zeppelins[ZepId].icon;

        string cost = "";
        for (int i = 0; i < home.zeppelins[ZepId].resourcesIDs.Length; i++)
        {
            cost += home.GetZepUpgradeCost(i, ZepId, inListID).ToString() + " " + home.inv.Resources[home.zeppelins[ZepId].resourcesIDs[i]].Name + "\n";
        }
        upgradeCost.text = cost;

        level.text = "Level: " + home.PlayerA.zeps[inListID].Level;

        string st = "Stats: (this vs selected)\n\nConsumption:\n" +
                    "    " + home.zeppelins[ZepId].woodConsumption + " vs " + home.zeppelins[home.currentModelToBuy].woodConsumption + " wood/s\n\nCapacity:\n" +
                    "    " + (home.GetHVLimit(inListID)) + " vs " + home.zeppelins[home.currentModelToBuy].HVlimit + " wood & stone\n" +
                    "    " + (home.GetFoodLimit(inListID)) + " vs " + home.zeppelins[home.currentModelToBuy].foodLimit + " food\n" +
                    "    " + (home.GetWaterLimit(inListID)) + " vs " + home.zeppelins[home.currentModelToBuy].waterLimit + " water\n" +
                    "    " + (home.GetPopLimit(inListID)) + " vs " + home.zeppelins[home.currentModelToBuy].popLimit + " population";

        stats.text = st;

        rating.text = "Rating: " + home.zeppelins[ZepId].rating + "/10";

        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(() => { home.UpgradeZep(inListID, ZepId); });

        replaceButton.onClick.RemoveAllListeners();
        replaceButton.onClick.AddListener(() => { home.ReplaceZep(slotID); });
    }
}
