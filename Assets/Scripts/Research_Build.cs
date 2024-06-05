using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Research_Build : MonoBehaviour
{
    public Text Name, currentStats, afterStats, level, techCount;
    public Button upgradeButton;
    public Image icon;

    public TechCenter home;
    public int BuildID;

    void Start()
    {
        UpdateThis();
    }

    public void UpdateThis()
    {
        Name.text = "Build: " + home.home.PlayerA.builds[BuildID].Name;
        string currentSt = "Current stats:\n";
        string afterSt = "After\nupgrade:\n";
        for (int i = 0; i < home.home.PlayerA.builds[BuildID].perSecpnt.Length; i++) 
        {
            afterSt+= " + " + (home.home.PlayerA.builds[BuildID].perSecpnt[i] * (home.home.PlayerA.builds[BuildID].level + 2)) + " " + home.home.PlayerA.GetItemName(home.home.PlayerA.builds[BuildID].productionIDs[i]) + "/s.\n";
            currentSt += " + " + (home.home.PlayerA.builds[BuildID].perSecpnt[i] * (home.home.PlayerA.builds[BuildID].level + 1)) + " " + home.home.PlayerA.GetItemName(home.home.PlayerA.builds[BuildID].productionIDs[i]) + "/s.\n";
        }

        currentStats.text = currentSt;
        afterStats.text = afterSt;

        icon.sprite = home.home.PlayerA.builds[BuildID].icon;

        level.text = "Current level:\n" + home.home.PlayerA.GetBuildLevel(BuildID).ToString();
        techCount.text = home.home.Techs + "/" + home.home.PlayerA.GetBuildCost(BuildID);

        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(() => { home.home.PlayerA.UpgradeBuild(BuildID); });
    }
}
