using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildPanel : MonoBehaviour
{
    public Image homePanel, icon1, icon2;
    public GameObject NormalPanel, WorkingPanel;

    public Color NormalC, ReadyC, PseudoC;

    public Text name_p1, name_p2, description_p1, stats_p2, pop_p2, pop_p1;

    public Player home;

    public int BuildID;
    public Vector3 reference;

    public int population;
    public bool statusReady;

    public Button buildThisButton, abandonButton;

    public bool Spawn;

    void Start()
    {
        UpdatePanel();
    }

    public void UpdatePanel()
    {
        icon1.sprite = home.builds[BuildID].icon;
        icon2.sprite = icon1.sprite;

        name_p1.text = "(Lvl: " + (home.builds[BuildID].level + 1) + ")Build: " + home.builds[BuildID].Name;
        name_p2.text = name_p1.text;
        pop_p1.text = "Workers: " + population; pop_p2.text = pop_p1.text;

        string Cost = "Cost:";
        for (int i = 0; i < home.builds[BuildID].quantity.Length; i++)
        {
            Cost += " " + home.builds[BuildID].quantity[i] + " " + home.GetItemName(home.builds[BuildID].resourceIDs[i]) + "\n";
        }

        if (!Spawn)
        {
            Cost = "A " + home.builds[BuildID].Name + " already exists. Would you like to work it?";
        }

        string Provides = "Provides:";
        for (int i = 0; i < home.builds[BuildID].perSecpnt.Length; i++)
        {
            Provides += " + " + (home.builds[BuildID].perSecpnt[i] * (home.builds[BuildID].level + 1)) + " " + home.GetItemName(home.builds[BuildID].productionIDs[i]) + "/s.\n";
        }

        description_p1.text = Cost + Provides;
        stats_p2.text = Provides;

        buildThisButton.onClick.RemoveAllListeners();
        buildThisButton.onClick.AddListener(() => { BuildThis(BuildID, Spawn); });

        abandonButton.onClick.RemoveAllListeners();
        abandonButton.onClick.AddListener(() => { Abandon(); });
    }

    public void Abandon()
    {
        if(statusReady)
        {
            Spawn = false;
            statusReady = false;
            NormalPanel.SetActive(true);
            WorkingPanel.SetActive(false);
            homePanel.color = PseudoC;
            home.Manager.AddPopulation(population);
            UpdatePanel();
            home.Manager.SetUpResourcePanel();
        }
    }

    public IEnumerator work()
    {
        while (statusReady)
        {
            yield return new WaitForSeconds(1f);
            for (int i = 0; i < home.builds[BuildID].productionIDs.Length; i++) 
            {
                home.Manager.AddItem(home.builds[BuildID].productionIDs[i], home.builds[BuildID].perSecpnt[i] * (home.builds[BuildID].level + 1));
                home.Manager.SetUpResourcePanel();
            }
        }
    }

    public void BuildThis(int ID, bool Sp)
    {
        if (home.Manager.Population >= home.builds[ID].population)
        {
            bool eligible = true;

            if (Sp)
            {
                for (int i = 0; i < home.builds[ID].resourceIDs.Length; i++)
                {
                    if (home.Manager.inv.ResourceQuantity[home.builds[ID].resourceIDs[i]] < home.builds[ID].quantity[i])
                    {
                        eligible = false;
                    }
                }
            }
            else { eligible = true; }

            if (eligible)
            {
                if (Sp)
                {
                    //appply costs
                    home.Manager.statsTr.buildings++;
                    for (int i = 0; i < home.builds[ID].resourceIDs.Length; i++)
                    {
                        if (home.Manager.inv.ResourceQuantity[home.builds[ID].resourceIDs[i]] >= home.builds[ID].quantity[i])
                        {
                            home.Manager.inv.ResourceQuantity[home.builds[ID].resourceIDs[i]] -= home.builds[ID].quantity[i];
                        }
                    }

                    for (int y = 0; y < 7; y++)
                    {
                        for (int x = 0; x < 7; x++)
                        {
                            if (!home.BuildingPositions[y, x])
                            {
                                float xPos = reference.x + x * 3f - 9f;
                                float yPos = reference.z + y * 3f - 9f;

                                float oceans = Mathf.PerlinNoise((xPos * 2f + home.Home.GlobalSeed) * .003f, (yPos * 2f + home.Home.GlobalSeed) * .003f);

                                float perlin = Mathf.PerlinNoise((xPos * 2f + home.Home.GlobalSeed) * .03f, (yPos * 2f + home.Home.GlobalSeed) * .03f) * oceans;

                                int rezult = Mathf.FloorToInt(perlin * home.Manager.a);

                                if (rezult == 2 || rezult == 3)
                                {
                                    home.BuildingPositions[y, x] = true;

                                    Vector4 newBuild = new Vector4(xPos, rezult, yPos, BuildID);

                                    int ChunckX = Mathf.FloorToInt(newBuild.x / 100) * 100, ChunckZ = Mathf.FloorToInt(newBuild.z / 100) * 100;

                                    string chunckKey = (ChunckX / 100).ToString() + "_" + (ChunckZ / 100).ToString();

                                    if (home.Home.Buildings.ContainsKey(chunckKey))
                                    {
                                        home.Home.Buildings[chunckKey].Add(newBuild);
                                    }
                                    else
                                    {
                                        home.Home.Buildings.Add(chunckKey, new List<Vector4>() { newBuild });
                                    }

                                    GameObject obj = Instantiate(home.builds[BuildID].Object, new Vector3(xPos, rezult, yPos), Quaternion.identity);
                                    obj.transform.SetParent(home.Home.chuncks[chunckKey].transform);
                                    obj.name = BuildID.ToString() + "build";

                                    goto endLoop;

                                }
                                else
                                {
                                    home.BuildingPositions[y, x] = true;
                                }
                            }
                        }
                    }

                }

                endLoop:

                //continue
                statusReady = true;
                NormalPanel.SetActive(false);
                WorkingPanel.SetActive(true);
                homePanel.color = ReadyC;
                StartCoroutine(work());
                home.Manager.Population -= population;
                home.Manager.UpdateFoodConsumption();
                home.Manager.SetUpResourcePanel();
            }
        }
    }
}
