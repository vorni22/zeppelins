using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BUILD
{
    public string Name;
    public Sprite icon;
    public GameObject Object;

    public int level, techs;

    public int population;
    public int[] productionIDs, perSecpnt;
    public int[] resourceIDs, quantity;
}

public class Player : MonoBehaviour
{
    public BUILD[] builds;

    public List<GameObject> boats;
    public List<Zeppelin> zeps;

    public float speed;

    public Vector3 lastPos;

    public Generator Home;
    public float a, b;

    public bool Landing = false, InLanding, Landed, inDecolation;

    public Joystick Stick;

    public bool LandingMode;

    public GameObject HarvestingRadius, landingModeButton;

    public float Radius = 10.0f;

    public int working;

    public GameManager Manager;

    public RectTransform harvestPanel, buildPanel; public GameObject ResourcePanelExp, LandModeMenu, BuildPanelExp;
    public List<ResourcePanel> HarvestableIn;
    public List<BuildPanel> BuildPanels;

    public bool[,] BuildingPositions = new bool[7, 7];

    public TechCenter techCenter;

    void Start()
    {
        LandingMode = false;

        BuildingPositions[0, 0] = true; BuildingPositions[0, 1] = true; BuildingPositions[0, 5] = true; BuildingPositions[0, 6] = true;
        BuildingPositions[1, 0] = true; BuildingPositions[1, 6] = true;

        BuildingPositions[6, 0] = true; BuildingPositions[6, 1] = true; BuildingPositions[6, 5] = true; BuildingPositions[6, 6] = true;
        BuildingPositions[5, 0] = true; BuildingPositions[5, 6] = true;
    }

    void Update()
    {
        if (!Manager.pauze)
        {
            float x = Time.deltaTime * speed * Stick.Horizontal;
            float z = Time.deltaTime * speed * Stick.Vertical;

            Vector3 move = new Vector3(x, 0, z);

            int ct = 0;

            if (!InLanding && !Landed)
            {
                for (int i = 0; i < boats.Count; i++)
                {
                    Vector3 newPos = boats[i].transform.position + move;

                    float oceans = Mathf.PerlinNoise(((int)newPos.x * 2 + Home.GlobalSeed) * .003f, ((int)newPos.z * 2 + Home.GlobalSeed) * .003f);

                    float perlin = Mathf.PerlinNoise(((int)newPos.x * 2 + Home.GlobalSeed) * .03f, ((int)newPos.z * 2 + Home.GlobalSeed) * .03f) * oceans;

                    float Nx = Mathf.Floor(perlin * a);

                    if (Nx < 2 || Nx > 3) { ct++; }

                    boats[i].transform.position = newPos;

                    Vector3 direction = boats[0].transform.position - lastPos;

                    if (direction.x != 0 || direction.z != 0)
                    {
                        boats[i].transform.rotation = Quaternion.LookRotation(direction);
                    }
                }
            }

            if (ct == 0 && !Landed) { Landing = true; } else { Landing = false; }

            if (Landing)
            {
                //can land
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    InLanding = true;
                }
            }

            if (Landed)
            {
                if (Input.GetKeyDown(KeyCode.Space) && working == 0)
                {
                    inDecolation = true;
                    CloseHarvesting();
                }
            }

            if (inDecolation)
            {
                int done = 0;
                for (int i = 0; i < boats.Count; i++)
                {
                    float y = Time.deltaTime * speed;

                    if (boats[i].transform.position.y < 22f)
                    {
                        boats[i].transform.position += new Vector3(0, y, 0);
                    }
                    else
                    {
                        boats[i].transform.position = new Vector3(boats[i].transform.position.x, 22f, boats[i].transform.position.z);
                        done++;
                        zeps[i].anim.SetActive(true);
                    }
                }

                if (done == boats.Count)
                {
                    inDecolation = false;
                    Landed = false;
                }
            }

            if (InLanding)
            {
                int done = 0;
                for (int i = 0; i < boats.Count; i++)
                {
                    float xc = boats[i].transform.position.x, zc = boats[i].transform.position.z;

                    float oceans = Mathf.PerlinNoise(((int)xc * 2 + Home.GlobalSeed) * .003f, ((int)zc * 2 + Home.GlobalSeed) * .003f);

                    float perlin = Mathf.PerlinNoise(((int)xc * 2 + Home.GlobalSeed) * .03f, ((int)zc * 2 + Home.GlobalSeed) * .03f) * oceans;

                    float Nx = Mathf.Floor(perlin * a);

                    float y = -Time.deltaTime * speed;

                    if (boats[i].transform.position.y >= Nx + 1.2f)
                    {
                        boats[i].transform.position += new Vector3(0, y, 0);
                    }
                    else
                    {
                        done++;
                        zeps[i].anim.SetActive(false);
                    }
                }

                if (done == boats.Count)
                {
                    InLanding = false;
                    Landed = true;
                    SetUpHarvestingSystem();
                }
            }

            lastPos = boats[0].transform.position;
            transform.position = new Vector3(boats[0].transform.position.x, 4.5f, boats[0].transform.position.z);
        }
    }

    public void CloseHarvesting()
    {
        landingModeButton.SetActive(true);
        for (int i = 0; i < BuildPanels.Count; i++)
        {
            if (BuildPanels[i].statusReady)
            {
                Manager.AddPopulation(BuildPanels[i].population);
                BuildPanels[i].statusReady = false;
            }
            Destroy(BuildPanels[i].gameObject);
        }
        BuildPanels = new List<BuildPanel>();

        HarvestingRadius.SetActive(false);
        LandModeMenu.SetActive(false);

        Manager.SetUpResourcePanel();
    }
    public void SetUpHarvestingSystem()
    {
        landingModeButton.SetActive(false);
        BuildingPositions = new bool[7, 7];
        BuildingPositions[0, 0] = true; BuildingPositions[0, 1] = true; BuildingPositions[0, 5] = true; BuildingPositions[0, 6] = true;
        BuildingPositions[1, 0] = true; BuildingPositions[1, 6] = true;

        BuildingPositions[6, 0] = true; BuildingPositions[6, 1] = true; BuildingPositions[6, 5] = true; BuildingPositions[6, 6] = true;
        BuildingPositions[5, 0] = true; BuildingPositions[5, 6] = true;

        LandModeMenu.SetActive(true);

        //builds
        for (int i = 0; i < BuildPanels.Count; i++)
        {
            Destroy(BuildPanels[i].gameObject);
        }

        BuildPanels = new List<BuildPanel>();

        buildPanel.sizeDelta = new Vector2(builds.Length * 130, 192);

        for (int i = 0; i < builds.Length; i++)
        {
            GameObject newPanel = Instantiate(BuildPanelExp, new Vector3(0, 0, 0), Quaternion.identity);
            newPanel.transform.SetParent(buildPanel.transform);

            newPanel.transform.localScale = new Vector3(1, 1, 1);
            newPanel.transform.localPosition = new Vector3(65 + i * 125, 0, 0);

            BuildPanel resource = newPanel.GetComponent<BuildPanel>();
            resource.reference = new Vector3(transform.position.x, 0, transform.position.z);
            resource.home = this;
            resource.BuildID = i;
            resource.population = builds[i].population;
            resource.Spawn = true;

            BuildPanels.Add(resource);
        }

        //harverst panel
        for (int i = 0; i < HarvestableIn.Count; i++)
        {
            Destroy(HarvestableIn[i].gameObject);
        }

        Collider[] Harvestable = Physics.OverlapSphere(HarvestingRadius.transform.position, Radius);

        HarvestableIn = new List<ResourcePanel>();

        int buildsDetected = 0;
        int other = 0;

        for (int i = 0; i < Harvestable.Length; i++)
        {
            if (Harvestable[i].tag != "temple" && Harvestable[i].tag != "zep") 
            {
                if (Harvestable[i].tag != "build")
                {
                    GameObject newPanel = Instantiate(ResourcePanelExp, new Vector3(0, 0, 0), Quaternion.identity);
                    newPanel.transform.SetParent(harvestPanel.transform);

                    newPanel.transform.localScale = new Vector3(1, 1, 1);
                    newPanel.transform.localPosition = new Vector3(65 + (i - buildsDetected - other) * 125, 0, 0);

                    ResourcePanel resource = newPanel.GetComponent<ResourcePanel>();
                    resource.home = this;
                    resource.ResourceTarget = Harvestable[i].gameObject;
                    resource.reference = new Vector3(transform.position.x, 0, transform.position.z);

                    HarvestableIn.Add(resource);
                }
                else
                {
                    buildsDetected++;
                    int id = (Harvestable[i].name[0]) - 48;
                    if (!BuildPanels[id].statusReady)
                    {
                        BuildPanels[id].Spawn = false;
                        BuildPanels[id].homePanel.color = BuildPanels[id].PseudoC;
                    }

                    //detect and ban the position
                    int x = Mathf.RoundToInt(-(transform.position.x - Harvestable[i].transform.position.x) / 3f) + 3;
                    int y = Mathf.RoundToInt(-(transform.position.z - Harvestable[i].transform.position.z) / 3f) + 3;
                    if (x > 6) { x = 6; }
                    if (x < 0) { x = 0; }
                    if (y > 6) { y = 6; }
                    if (y < 0) { y = 0; }
                    BuildingPositions[y, x] = true;
                }
            }
            else
            {
                other++;
            }
        }

        harvestPanel.sizeDelta = new Vector2((Harvestable.Length - buildsDetected - other) * 130, 120);

        HarvestingRadius.SetActive(true);
    }

    public void HarvestThis(GameObject obj)
    {
        string tag = obj.tag;
        int ind = 0;

        if (tag != "build")
        {
            if (tag == "stone") { ind = 1; }
            if (tag == "woods") { ind = 0; }
            if (tag == "food") { ind = 2; }

            if (tag == "city")
            {
                Manager.statsTr.citiesCollected++;
                Manager.AddPopulation(1);
            }
            else
            {
                Manager.AddItem(ind, Manager.GetHarvestQuantity());
            }

            int ChunckX = Mathf.FloorToInt(obj.transform.position.x / 100) * 100, ChunckZ = Mathf.FloorToInt(obj.transform.position.z / 100) * 100;

            string chunckKey = (ChunckX / 100).ToString() + "_" + (ChunckZ / 100).ToString();
            string structKey = obj.name;

            if (Home.Modifications.ContainsKey(chunckKey))
            {
                Home.Modifications[chunckKey].Add(structKey);
            }
            else
            {
                Home.Modifications.Add(chunckKey, new List<string> { structKey });
            }

            Destroy(obj);

            Manager.SetUpResourcePanel();
        }
    }

    public void EvaluateLandinDecolation()
    {
        if(Landing)
        {
            InLanding = true;
        }

        if (Landed && working == 0) 
        {
            inDecolation = true;
            CloseHarvesting();
        }
    }

    public void EvaluateLandingMode()
    {
        if(LandingMode)
        {
            for (int i = 0; i < boats.Count; i++)
            {
                zeps[i].zooming.SetActive(false);
            }

            LandingMode = false;
        }
        else
        {
            for (int i = 0; i < boats.Count; i++)
            {
                zeps[i].zooming.SetActive(true);
            }

            LandingMode = true;
        }
    }

    public void UpgradeBuild(int BuildID)
    {
        bool posible = false;
        if (GetBuildCost(BuildID) <= Manager.Techs)
        {
            posible = true;
            Manager.Techs -= GetBuildCost(BuildID);
            techCenter.invested_Builds += GetBuildCost(BuildID);
            Manager.UpdateTechCount();
        }

        if (posible)
        {
            Manager.statsTr.buildsLevel++;
            builds[BuildID].level += 1;
            if (BuildPanels.Count > 0)
            {
                BuildPanels[BuildID].UpdatePanel();
            }

            techCenter.upgrades_Builds += 1;

            techCenter.builds_RS[BuildID].UpdateThis();
            techCenter.UpdateDetails_1();

            for (int i = 0; i < techCenter.builds_RS.Count; i++)
            {
                techCenter.builds_RS[i].techCount.text = Manager.Techs + "/" + GetBuildCost(i);
            }
            for (int i = 0; i < techCenter.zeps_RS.Count; i++)
            {
                techCenter.zeps_RS[i].count.text = Manager.Techs + "/" + Manager.zeppelins[i].Techs;
            }
            techCenter.SetUpHarvestingPanel();
        }
    }

    public void ResearchZep(int ZepId)
    {
        if (Manager.zeppelins[ZepId].Techs <= Manager.Techs)
        {
            Manager.Techs -= Manager.zeppelins[ZepId].Techs;
            techCenter.invested_Zeps += Manager.zeppelins[ZepId].Techs;
            techCenter.researchs_Zeps++;
            techCenter.TRating_Zeps += Manager.zeppelins[ZepId].rating;
            techCenter.UpdateDetails_2();
            Manager.UpdateTechCount();

            Manager.zeppelins[ZepId].Unlocked = true;
            if (techCenter.zeps_RS.Count > 0)
            {
                techCenter.zeps_RS[ZepId].UpdateThis();
            }

            for (int i = 0; i < techCenter.builds_RS.Count; i++)
            {
                techCenter.builds_RS[i].techCount.text = Manager.Techs + "/" + GetBuildCost(i);
            }
            for (int i = 0; i < techCenter.zeps_RS.Count; i++)
            {
                techCenter.zeps_RS[i].count.text = Manager.Techs + "/" + Manager.zeppelins[i].Techs;
            }
            techCenter.SetUpHarvestingPanel();

            Manager.SetUpShopPanel();
        }
    }

    public int GetBuildCost(int BuildID)
    {
        return (int)Mathf.Pow(builds[BuildID].techs, builds[BuildID].level);
    }

    public int GetBuildLevel(int BuildID)
    {
        return builds[BuildID].level + 1;
    }

    public string GetItemName(int id)
    {
        if (id == 0) { return "wood"; }
        else if (id == 1) { return "stone"; }
        else if (id == 2) { return "food"; }
        else if (id == 3) { return "water"; }
        else { return "undefined item"; }
    }
}
