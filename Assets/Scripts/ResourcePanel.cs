using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ResourcePanel : MonoBehaviour
{
    public Image homePanel;
    public GameObject NormalPanel, WorkingPanel;

    public Color NormalC, WorkingC, ReadyC;

    public Text ResourceName_p1, ResourceName_p2, time_p1, timeLeft_p2, pop_p1, pop_p2, status_p2, distance_p1;
    public GameObject progressBar_p2;

    public Player home;

    public GameObject ResourceTarget;
    public Vector3 reference;

    public int Time, elapsedTime, population, resourceID;
    public bool statusReady;

    public Button harvestThisButton;

    public float Distance;
    void Start()
    {
        string name = ResourceTarget.tag;

        if (name == "stone")
        {
            resourceID = 1;
        }
        else if (name == "food")
        {
            resourceID = 2;
        }
        else if (name == "city")
        {
            resourceID = 4;
        }
        else if (name == "temple")
        {
            resourceID = 5;
        }

        Vector3 obj = new Vector3(ResourceTarget.transform.position.x, 0, ResourceTarget.transform.position.z);

        ResourceName_p1.text = "Resource: " + name; ResourceName_p2.text = "Resource: " + name;
        Distance = Vector3.Distance(obj, reference);

        distance_p1.text = "Distance: " + ((int)(Distance)).ToString() + " km";

        Time = (int)(Distance * 1.3f) + 1; time_p1.text = "Time: " + Time.ToString() + " seconds";

        population = Mathf.FloorToInt(Distance / 3.0f) - 1; if (population <= 0) { population = 1; }
        pop_p1.text = "Population: " + population.ToString(); pop_p2.text = pop_p1.text;
        timeLeft_p2.text = time_p1.text;

        harvestThisButton.onClick.AddListener(() => { HarvestButton(); });
    }

    public void HarvestButton()
    {
        StartCoroutine(HarvestThis());
    }

    public IEnumerator HarvestThis()
    {
        if (home.Manager.Population >= population)
        {
            home.working++;

            homePanel.color = WorkingC;
            WorkingPanel.SetActive(true);
            NormalPanel.SetActive(false);
            home.Manager.Population -= population;
            home.Manager.SetUpResourcePanel();
            status_p2.text = "Status: working...";

            for (int i = 0; i <= Time; i++)
            {
                progressBar_p2.transform.localScale = new Vector3((float)i/Time, 1, 1);
                yield return new WaitForSeconds(1.0f);
            }

            home.Manager.AddPopulation(population);
            homePanel.color = ReadyC;
            statusReady = true;
            home.HarvestThis(ResourceTarget);
            home.Manager.SetUpResourcePanel();
            status_p2.text = "Status: Done!";

            home.working--;
        }
    }
}
