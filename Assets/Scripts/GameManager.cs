using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct Resource
{
    public string Name;
    public int ID;
}

[System.Serializable]
public struct Inventory
{
    public Resource[] Resources;
    public int[] ResourceQuantity;
}

[System.Serializable]
public struct ZepModels
{
    public Sprite icon;
    public GameObject prefab;
    public string Name;
    public bool Unlocked;

    public int Techs, rating;

    public float upgradeRatio;

    public float woodConsumption;
    public int HVlimit, foodLimit, waterLimit, popLimit;

    public int[] resourcesIDs;
    public int[] resourcesQuantity;
}

public class GameManager : MonoBehaviour
{
    public Inventory inv;
    public int Techs;
    public int Population, PopulationLimit;
    public int FoodLim, HVLim, WaterLim;
    public float foodConsumption, woodConsumption, waterConsumption;

    public float FoodPerPopulation, waterPerPop;

    public ZepModels[] zeppelins;
    public Player PlayerA;

    public bool[] zeps;
    public int[] zepIDs;

    public Vector3[] slotPos;
    int CurrentSlot;
    public int CurrentModel, currentModelToBuy;

    //shop panel
    public RectTransform ShopContainer, myZepsContainer;
    public GameObject shopSlotExmple, myZepExample;
    public List<ShopSlot> Shop;
    public List<MyZepDetails> myZepsDetails;
    public Text ZepCount, SelectedZep;
    public GameObject buyButton, warningText;

    //details panel
    public Text woodCount, stoneCount, foodCount, waterCount;
    public Text hvLimCount, foodLimCount, waterLimCount;
    public RectTransform hvBar, foodBar, waterBar;

    public Text populationTxt, foodConsTxt, woodConsTxt, waterConsTxt;

    //tech panel
    public Text techCount;

    public float a, b;

    public bool pauze;

    public StatsTracker statsTr;

    float NextAction = 0f;
    float spendWood, spendWater, spendFood;

    public Text GameOverCauze;
    public GameObject GameOverMenu;

    public int HarvestingLevel, harvestingPower, harvestUpgradeCost;

    void Start()
    {
        PlayerA.zeps = new List<Zeppelin>();

        zeps = new bool[7];  zepIDs = new int[7];

        CurrentSlot = 4;//0
        CurrentModel = 0;

        Shop = new List<ShopSlot>();
        AddZep(false);
        AddPopulation(1);
        SetUpResourcePanel();
        UpdateTechCount();
    }

    void Update()
    {
        if (Time.timeSinceLevelLoad > NextAction && !pauze) 
        {
            NextAction += 1f;
            spendFood += foodConsumption;
            spendWater += waterConsumption;
            if (!PlayerA.Landed)
            {
                spendWood += woodConsumption;
            }

            inv.ResourceQuantity[0] -= Mathf.FloorToInt(spendWood);
            statsTr.burnedFuel += Mathf.FloorToInt(spendWood);
            spendWood -= Mathf.Floor(spendWood);

            inv.ResourceQuantity[2] -= Mathf.FloorToInt(spendFood);
            spendFood -= Mathf.Floor(spendFood);

            inv.ResourceQuantity[3] -= Mathf.FloorToInt(spendWater);
            spendWater -= Mathf.Floor(spendWater);

            SetUpResourcePanel();

            if(inv.ResourceQuantity[0]<0)
            {
                GameOver("Out of fuel!");
            }
            else if (inv.ResourceQuantity[2] < 0)
            {
                GameOver("Out of food!");
            }
            else if(inv.ResourceQuantity[3] < 0)
            {
                GameOver("Out of water");
            }
        }
    }

    public void UpgradeHarvesting()
    {
        if (Techs >= GetHarvesrUpgradeCost())
        {
            Techs -= GetHarvesrUpgradeCost();
            PlayerA.techCenter.invested_Builds += GetHarvesrUpgradeCost();
            HarvestingLevel++;
            PlayerA.techCenter.UpdateDetails_1();
            UpdateTechCount();

            for (int i = 0; i < PlayerA.techCenter.builds_RS.Count; i++)
            {
                PlayerA.techCenter.builds_RS[i].techCount.text = Techs + "/" + PlayerA.GetBuildCost(i);
            }
            for (int i = 0; i < PlayerA.techCenter.zeps_RS.Count; i++)
            {
                PlayerA.techCenter.zeps_RS[i].count.text = Techs + "/" + zeppelins[i].Techs;
            }
            PlayerA.techCenter.SetUpHarvestingPanel();
        }
    }

    public int GetHarvesrUpgradeCost()
    {
        return harvestUpgradeCost * (HarvestingLevel + 1);
    }
    public int GetHarvestQuantity()
    {
        return harvestingPower * (HarvestingLevel + 1);
    }

    public void GameOver(string Cauze)
    {
        statsTr.SetUpStatsPanel();
        PauseGame();
        GameOverCauze.text = Cauze;
        GameOverMenu.SetActive(true);
        PlayerA.Stick.gameObject.SetActive(false);
    }

    public void PauseGame()
    {
        pauze = true;
        Time.timeScale = 0;
    }

    public void UnPauseGame()
    {
        pauze = false;
        Time.timeScale = 1;
    }

    public void UpgradeZep(int inListID, int zepID)
    {
        bool buyed = true;

        for (int i = 0; i < zeppelins[zepID].resourcesIDs.Length; i++)
        {
            if (inv.ResourceQuantity[zeppelins[zepID].resourcesIDs[i]] < GetZepUpgradeCost(i, zepID, inListID)) 
            {
                buyed = false;
            }
        }

        if(buyed)
        {
            statsTr.zeppLevel++;

            for (int i = 0; i < zeppelins[zepID].resourcesIDs.Length; i++)
            {
                if (inv.ResourceQuantity[zeppelins[zepID].resourcesIDs[i]] >= GetZepUpgradeCost(i, zepID, inListID))
                {
                    inv.ResourceQuantity[zeppelins[zepID].resourcesIDs[i]] -= GetZepUpgradeCost(i, zepID, inListID);
                }
            }

            PlayerA.zeps[inListID].Level++;

            UpdateTotalLimits();

            SetUpResourcePanel();
            SetUpShopPanel();
        }
    }

    public int GetZepUpgradeCost(int ResourceID, int zepID, int InList)
    {
        return (int)(zeppelins[zepID].resourcesQuantity[ResourceID] * zeppelins[zepID].upgradeRatio * (PlayerA.zeps[InList].Level + 1));
    }

    public void UpdateTechCount()
    {
        techCount.text = Techs.ToString();
    }

    public void AddItem(int index, int Quantity)
    {
        inv.ResourceQuantity[index] += Quantity;

        if (index == 0) { if (inv.ResourceQuantity[index]+inv.ResourceQuantity[1] > HVLim) { inv.ResourceQuantity[index] += HVLim - inv.ResourceQuantity[index] - inv.ResourceQuantity[1]; } }
        if (index == 1) { if (inv.ResourceQuantity[index] + inv.ResourceQuantity[0] > HVLim) { inv.ResourceQuantity[index] += HVLim - inv.ResourceQuantity[index] - inv.ResourceQuantity[0]; } }
        if (index == 2) { if (inv.ResourceQuantity[index] > FoodLim) { inv.ResourceQuantity[index] = FoodLim; } }
        if (index == 3) { if (inv.ResourceQuantity[index] > WaterLim) { inv.ResourceQuantity[index] = WaterLim; } }
        if (index == 4) { AddPopulation(Quantity); }
    }

    public void UpdateFoodConsumption()
    {
        foodConsumption = Population * FoodPerPopulation;
        waterConsumption = Population * waterPerPop;
    }

    public void AddPopulation(int PopulationNumber)
    {
        if (Population + PopulationNumber <= PopulationLimit)
        {
            Population += PopulationNumber;
        }
        else
        {
            Population = PopulationLimit;
        }

        UpdateFoodConsumption();
    }

    public void SetUpResourcePanel()
    {
        woodCount.text = inv.ResourceQuantity[0].ToString();
        stoneCount.text = inv.ResourceQuantity[1].ToString();
        foodCount.text = inv.ResourceQuantity[2].ToString();
        waterCount.text = inv.ResourceQuantity[3].ToString();

        int hv = (inv.ResourceQuantity[0] + inv.ResourceQuantity[1]);

        hvLimCount.text = hv.ToString() + "/" + HVLim.ToString();
        foodLimCount.text = inv.ResourceQuantity[2].ToString() + "/" + FoodLim.ToString();
        waterLimCount.text = inv.ResourceQuantity[3].ToString() + "/" + WaterLim.ToString();

        if (HVLim != 0) 
        {
            hvBar.transform.localScale = new Vector3((float)hv / HVLim, 1, 1);
        }
        else { hvBar.transform.localScale = new Vector3(1, 1, 1); }

        if (FoodLim != 0)
        {
            foodBar.transform.localScale = new Vector3((float)inv.ResourceQuantity[2] / FoodLim, 1, 1);
        }
        else { foodBar.transform.localScale = new Vector3(1, 1, 1); }

        if (WaterLim != 0)
        {
            waterBar.transform.localScale = new Vector3((float)inv.ResourceQuantity[3] / WaterLim, 1, 1);
        }
        else { waterBar.transform.localScale = new Vector3(1, 1, 1); }

        populationTxt.text = "Population: " + Population.ToString() + "/" + PopulationLimit.ToString();
        foodConsTxt.text = "Food consumption:\n" + foodConsumption.ToString() + " food/second";
        woodConsTxt.text = "Wood consumption:\n" + woodConsumption.ToString() + " wood/second";
        waterConsTxt.text = " Water consumption:\n" + waterConsumption.ToString() + " water/second";
    }

    public void UpdateMyZepsDetails()
    {
        for (int i = 0; i < myZepsDetails.Count; i++)
        {
            Destroy(myZepsDetails[i].gameObject);
        }
        myZepsDetails = new List<MyZepDetails>();

        myZepsContainer.sizeDelta = new Vector2(PlayerA.boats.Count * 175, 223);
        for (int i = 0; i < PlayerA.zeps.Count; i++)
        {
            GameObject newPanel = Instantiate(myZepExample, new Vector3(0, 0, 0), Quaternion.identity);
            newPanel.transform.SetParent(myZepsContainer.transform);

            newPanel.transform.localScale = new Vector3(1, 1, 1);
            newPanel.transform.localPosition = new Vector3(85 + i * 175, 0, 0);

            MyZepDetails zp = newPanel.GetComponent<MyZepDetails>();
            zp.home = this;
            zp.ZepId = PlayerA.zeps[i].ID;
            zp.slotID = PlayerA.zeps[i].slodID;
            zp.inListID = PlayerA.zeps[i].inListID;

            myZepsDetails.Add(zp);
        }
    }

    public void SetUpShopPanel()
    {
        ZepCount.text = PlayerA.boats.Count.ToString() + "/7 zepelines";

        for(int i = 0; i < Shop.Count; i++) { Destroy(Shop[i].gameObject); }
        Shop = new List<ShopSlot>();
        ShopContainer.sizeDelta = new Vector2(0, 0);

        buyButton.SetActive(true); warningText.SetActive(false);

        for (int i = 0, k=0; i < zeppelins.Length; i++)
        {
            if (zeppelins[i].Unlocked)
            {
                ShopContainer.sizeDelta += new Vector2(125, 0);
                GameObject newUI = Instantiate(shopSlotExmple, new Vector3(0, 0, 0), Quaternion.identity);
                newUI.transform.SetParent(ShopContainer.transform);
                newUI.transform.localScale = new Vector3(1, 1, 1);
                newUI.transform.localPosition = new Vector3(65 + 125 * k, 0, 0);
                ShopSlot sh = newUI.GetComponent<ShopSlot>();
                sh.img.sprite = zeppelins[i].icon;

                sh.Name.text = zeppelins[i].Name + "\nRating: " + zeppelins[i].rating;

                string st = "Stats:\n\nConsumption:\n" + "    " + zeppelins[i].woodConsumption + " wood/s\nCapacity:\n" +
                "    " + zeppelins[i].HVlimit + " wood & stone\n" +
                "    " + zeppelins[i].foodLimit + " food\n" +
                "    " + zeppelins[i].waterLimit + " water\n" +
                "    " + zeppelins[i].popLimit + " population";

                sh.Stats.text = st;

                string cost = "";
                for (int j = 0; j < zeppelins[i].resourcesIDs.Length; j++)
                {
                    int id = zeppelins[i].resourcesIDs[j];
                    cost = cost + inv.ResourceQuantity[id].ToString() + "/" + zeppelins[i].resourcesQuantity[j].ToString() + " " + inv.Resources[id].Name.ToString() + "\n";
                }
                sh.Cost.text = cost;

                sh.manager = this;
                sh.ID = i;
                sh.inShopSlot = k;
                sh.changeTarget.onClick.AddListener(() => { sh.ChangeTr(); });

                if (i == currentModelToBuy)
                {
                    sh.panel.color = sh.ActiveColor;
                    CurrentModel = k;
                }
                else
                {
                    sh.panel.color = sh.UnactivedColor;
                }

                Shop.Add(sh);
                k++;
            }
        }

        if (PlayerA.boats.Count >= 7)
        {
            buyButton.SetActive(false);
        }

        if (CurrentModel >= Shop.Count)
        {
            CurrentModel = Shop.Count - 1;

            if (Shop.Count != 0)
            {
                Shop[CurrentModel].panel.color = Shop[CurrentModel].ActiveColor;
            }
        }

        SelectedZep.text = "Selected zepelin: " + zeppelins[Shop[CurrentModel].ID].Name + " (Rating: " + zeppelins[Shop[CurrentModel].ID].rating + ")";
        currentModelToBuy = Shop[CurrentModel].ID;

        UpdateMyZepsDetails();
    }

    public void ReplaceZep(int slotID)
    {
        int Model = currentModelToBuy;

        bool Buyed = true;
        for (int i = 0; i < zeppelins[Model].resourcesIDs.Length; i++)
        {
            int ind = zeppelins[Model].resourcesIDs[i];
            if (inv.ResourceQuantity[ind] < zeppelins[Model].resourcesQuantity[i])
            {
                Buyed = false;
            }
        }

        if(CurrentModel == PlayerA.zeps[zepIDs[slotID]].ID) { Buyed = false; }

        if(Buyed)
        {
            for (int i = 0; i < zeppelins[Model].resourcesIDs.Length; i++)
            {
                int ind = zeppelins[Model].resourcesIDs[i];
                if (inv.ResourceQuantity[ind] >= zeppelins[Model].resourcesQuantity[i])
                {
                    inv.ResourceQuantity[ind] -= zeppelins[Model].resourcesQuantity[i];
                }
            }

            Destroy(PlayerA.zeps[zepIDs[slotID]].gameObject);

            Vector3 newPos;
            Quaternion rot;

            if (PlayerA.boats.Count == 0)
            {
                newPos = new Vector3(PlayerA.transform.position.x, 22, PlayerA.transform.position.z);
                rot = new Quaternion(0, 0, 0, 0);
            }
            else
            {
                newPos = PlayerA.boats[0].transform.position + slotPos[slotID];
                rot = PlayerA.boats[0].transform.rotation;
            }

            GameObject newZep = Instantiate(zeppelins[Model].prefab, newPos, rot);
            PlayerA.boats[zepIDs[slotID]] = newZep;

            Zeppelin zp = newZep.GetComponent<Zeppelin>();
            if (PlayerA.LandingMode) { zp.zooming.SetActive(true); }
            zp.home = PlayerA;
            zp.ID = Model;
            zp.inListID = zepIDs[slotID];
            zp.slodID = slotID;
            PlayerA.zeps[zepIDs[slotID]] = zp;

            UpdateTotalLimits();
            SetUpShopPanel();
            SetUpResourcePanel();
        }
    }

    public int GetHVLimit(int i)
    {
        return zeppelins[PlayerA.zeps[i].ID].HVlimit + (int)(PlayerA.zeps[i].Level * zeppelins[PlayerA.zeps[i].ID].HVlimit * zeppelins[PlayerA.zeps[i].ID].upgradeRatio);
    }
    public int GetFoodLimit(int i)
    {
        return zeppelins[PlayerA.zeps[i].ID].foodLimit + (int)(PlayerA.zeps[i].Level * zeppelins[PlayerA.zeps[i].ID].foodLimit * zeppelins[PlayerA.zeps[i].ID].upgradeRatio); 
    }
    public int GetWaterLimit(int i)
    {
        return zeppelins[PlayerA.zeps[i].ID].waterLimit + (int)(PlayerA.zeps[i].Level * zeppelins[PlayerA.zeps[i].ID].waterLimit * zeppelins[PlayerA.zeps[i].ID].upgradeRatio);
    }
    public int GetPopLimit(int i)
    {
        return zeppelins[PlayerA.zeps[i].ID].popLimit + (int)(PlayerA.zeps[i].Level * zeppelins[PlayerA.zeps[i].ID].popLimit * zeppelins[PlayerA.zeps[i].ID].upgradeRatio);
    }

    public void UpdateTotalLimits()
    {
        HVLim = 0;
        FoodLim = 0;
        WaterLim = 0;
        woodConsumption = 0;
        PopulationLimit = 0;

        for (int i = 0; i < PlayerA.zeps.Count; i++)
        {
            HVLim += GetHVLimit(i);
            FoodLim += GetFoodLimit(i);
            WaterLim += GetWaterLimit(i);
            woodConsumption += zeppelins[PlayerA.zeps[i].ID].woodConsumption;
            PopulationLimit += GetPopLimit(i);
        }

        if (inv.ResourceQuantity[0] + inv.ResourceQuantity[1] > HVLim) 
        {
            int delta = Mathf.CeilToInt(Mathf.Abs(HVLim - inv.ResourceQuantity[0] - inv.ResourceQuantity[1]) / 2.0f);
            if (inv.ResourceQuantity[0] >= delta) 
            {
                if (inv.ResourceQuantity[1] >= delta)
                {
                    inv.ResourceQuantity[0] -= delta;
                    inv.ResourceQuantity[1] -= delta;
                }
                else
                {
                    inv.ResourceQuantity[1] = 0;
                    inv.ResourceQuantity[0] -= (delta + Mathf.Abs(inv.ResourceQuantity[1] - delta));
                }
            }
            else
            {
                inv.ResourceQuantity[0] = 0;
                inv.ResourceQuantity[1] -= (delta + Mathf.Abs(inv.ResourceQuantity[0] - delta));
            }
        }

        if (inv.ResourceQuantity[2] > FoodLim)
        {
            inv.ResourceQuantity[2] = FoodLim;
        }

        if (inv.ResourceQuantity[3] > WaterLim)
        {
            inv.ResourceQuantity[3] = WaterLim;
        }

        if (Population > PopulationLimit)
        {
            Population = PopulationLimit;
        }
    }

    public void AddZep(bool buy)
    {
        int slotID = CurrentSlot;
        int Model = currentModelToBuy;

        if(!zeps[slotID])
        {
            bool Buyed = true;

            if (buy)
            {
                for (int i = 0; i < zeppelins[Model].resourcesIDs.Length; i++)
                {
                    int ind = zeppelins[Model].resourcesIDs[i];
                    if (inv.ResourceQuantity[ind] < zeppelins[Model].resourcesQuantity[i])
                    {
                        Buyed = false;
                    }
                }
            }
            else { Buyed = true; }

            if (Buyed)
            {
                if (buy)
                {
                    for (int i = 0; i < zeppelins[Model].resourcesIDs.Length; i++)
                    {
                        int ind = zeppelins[Model].resourcesIDs[i];
                        if (inv.ResourceQuantity[ind] >= zeppelins[Model].resourcesQuantity[i])
                        {
                            inv.ResourceQuantity[ind] -= zeppelins[Model].resourcesQuantity[i];
                        }
                    }
                }

                zepIDs[slotID] = PlayerA.boats.Count;
                zeps[slotID] = true;

                Vector3 newPos;
                Quaternion rot;

                if (PlayerA.boats.Count == 0) 
                {
                    newPos = new Vector3(PlayerA.transform.position.x, 22, PlayerA.transform.position.z);
                    rot = new Quaternion(0, 0, 0, 0);
                }
                else
                {
                    newPos = PlayerA.boats[0].transform.position + slotPos[slotID];
                    rot = PlayerA.boats[0].transform.rotation;
                }

                GameObject newZep = Instantiate(zeppelins[Model].prefab, newPos, rot);
                PlayerA.boats.Add(newZep);

                Zeppelin zp = newZep.GetComponent<Zeppelin>(); 
                if (PlayerA.LandingMode) { zp.zooming.SetActive(true); }
                zp.home = PlayerA;
                zp.ID = Model;
                zp.inListID = PlayerA.zeps.Count;
                zp.slodID = slotID;
                PlayerA.zeps.Add(zp);

                CurrentSlot++;
                if (CurrentSlot == 4) { CurrentSlot ++; }
                if (CurrentSlot > 6) { CurrentSlot = 0; }

                UpdateLimits(Model);
                SetUpShopPanel();
                SetUpResourcePanel();

                PlayerA.HarvestingRadius.transform.localScale += new Vector3(0.5f, 0.5f, 0);
                PlayerA.Radius += 0.5f;
            }
        }
    }

    public void UpdateLimits(int ZepModel)
    {
        HVLim += zeppelins[ZepModel].HVlimit;
        FoodLim += zeppelins[ZepModel].foodLimit;
        WaterLim += zeppelins[ZepModel].waterLimit;
        woodConsumption += zeppelins[ZepModel].woodConsumption;
        PopulationLimit += zeppelins[ZepModel].popLimit;
    }
}
