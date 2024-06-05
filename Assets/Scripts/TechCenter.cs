using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TechCenter : MonoBehaviour
{
    public GameManager home;

    public GameObject BuildResearhExemple, ZepResearchExample;

    public RectTransform buildingsContainer, zepContainer;

    public Text Invest_d1, Invest_d2, upgrades_d1, researchs_d2, totalRating_d2, techCount;

    public List<Research_Build> builds_RS;
    public List<Research_Zeps> zeps_RS;

    public Text HR_CurrentStats, HR_AfterStats, HR_Cost, HR_Level;

    public int invested_Builds,
               invested_Zeps, invested_ZepsMax,
               upgrades_Builds,
               researchs_Zeps, researchs_ZepMax,
               TRating_Zeps, TRating_ZepMax;

    void Start()
    {
        for (int i = 0; i < home.zeppelins.Length; i++)
        {
            invested_ZepsMax += home.zeppelins[i].Techs;
            researchs_ZepMax++;
            TRating_ZepMax += home.zeppelins[i].rating;

            if(home.zeppelins[i].Unlocked)
            {
                invested_Zeps += home.zeppelins[i].Techs;
                researchs_Zeps++;
                TRating_Zeps += home.zeppelins[i].rating;
            }
        }
    }

    public void SetUpTechPanel()
    {
        UpdateBuildResearch();
        UpdateZepResearch();
        UpdateDetails_1();
        UpdateDetails_2();
        SetUpHarvestingPanel();
    }
    
    public void SetUpHarvestingPanel()
    {
        HR_Cost.text = home.Techs + "/" + home.GetHarvesrUpgradeCost();
        HR_CurrentStats.text = "Current stats: " + home.GetHarvestQuantity() + " items/harvest";
        HR_AfterStats.text = "After upgrade stats: " + (home.GetHarvestQuantity() + home.harvestingPower) + " items/harvest";
        HR_Level.text = "Level: " + home.HarvestingLevel;
    }

    public void UpdateBuildResearch()
    {
        for (int i = 0; i < builds_RS.Count; i++)
        {
            Destroy(builds_RS[i].gameObject);
        }
        builds_RS = new List<Research_Build>();

        float c = buildingsContainer.sizeDelta.x;
        float newC = home.PlayerA.builds.Length * 180 + 165;
        if (newC < c) { newC = c; }
        buildingsContainer.sizeDelta = new Vector2(newC, 223);

        for (int i = 0; i < home.PlayerA.builds.Length; i++)
        {
            GameObject newPanel = Instantiate(BuildResearhExemple, new Vector3(0, 0, 0), Quaternion.identity);
            newPanel.transform.SetParent(buildingsContainer.transform);

            newPanel.transform.localScale = new Vector3(1, 1, 1);
            newPanel.transform.localPosition = new Vector3(165 + 90 + i * 175, 0, 0);

            Research_Build RB = newPanel.GetComponent<Research_Build>();
            RB.home = this;
            RB.BuildID = i;

            builds_RS.Add(RB);
        }
    }

    public void UpdateZepResearch()
    {
        for (int i = 0; i < zeps_RS.Count; i++)
        {
            Destroy(zeps_RS[i].gameObject);
        }
        zeps_RS = new List<Research_Zeps>();

        float c = zepContainer.sizeDelta.x;
        float newC = home.zeppelins.Length * 180;
        if (newC < c) { newC = c; }
        zepContainer.sizeDelta = new Vector2(newC, 282);

        for (int i = 0; i < home.zeppelins.Length; i++)
        {
            GameObject newPanel = Instantiate(ZepResearchExample, new Vector3(0, 0, 0), Quaternion.identity);
            newPanel.transform.SetParent(zepContainer.transform);

            newPanel.transform.localScale = new Vector3(1, 1, 1);
            newPanel.transform.localPosition = new Vector3(90 + i * 175, 0, 0);

            Research_Zeps RZ = newPanel.GetComponent<Research_Zeps>();
            RZ.home = this;
            RZ.ZepId = i;

            zeps_RS.Add(RZ);
        }
    }

    public void UpdateDetails_1()
    {
        Invest_d1.text = "Techs invested:\n" + invested_Builds;
        upgrades_d1.text = "Upgrades:\n" + upgrades_Builds;
        techCount.text = home.Techs.ToString();
    }

    public void UpdateDetails_2()
    {
        Invest_d2.text = "Techs invested:\n" + invested_Zeps + "/" + invested_ZepsMax;
        researchs_d2.text = "Upgrades:\n" + researchs_Zeps + "/" + researchs_ZepMax;
        totalRating_d2.text = "Total rating:\n" + TRating_Zeps + "/" + TRating_ZepMax;
        techCount.text = home.Techs.ToString();
    }
}
