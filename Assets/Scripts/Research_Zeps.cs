using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Research_Zeps : MonoBehaviour
{
    public int ZepId;
    public TechCenter home;

    public Color normalC, researchedC;

    public Text Name, stats, count, doneText, rating;
    public Button researchButton;
    public Image icon;

    public Image container;

    void Start()
    {
        UpdateThis();
    }

    public void UpdateThis()
    {
        Name.text = home.home.zeppelins[ZepId].Name;
        icon.sprite = home.home.zeppelins[ZepId].icon;

        string st = "Stats:\n\n Consumption - " + home.home.zeppelins[ZepId].woodConsumption + " wood/s\n\n Capacity:\n" +
            home.home.zeppelins[ZepId].HVlimit + " wood & stone\n" +
            home.home.zeppelins[ZepId].foodLimit + " food\n" +
            home.home.zeppelins[ZepId].waterLimit + " water\n" +
            home.home.zeppelins[ZepId].popLimit + " population";

        stats.text = st;

        count.text = home.home.Techs + "/" + home.home.zeppelins[ZepId].Techs;
        rating.text = "Rating: " + home.home.zeppelins[ZepId].rating;

        researchButton.onClick.RemoveAllListeners();

        if (home.home.zeppelins[ZepId].Unlocked)
        {
            container.color = researchedC;
            researchButton.gameObject.SetActive(false);
            doneText.gameObject.SetActive(true);
        }
        else
        {
            container.color = normalC;
            researchButton.gameObject.SetActive(true);
            doneText.gameObject.SetActive(false);
            researchButton.onClick.AddListener(() => { home.home.PlayerA.ResearchZep(ZepId); });
        }
    }
}
