using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StatsTracker : MonoBehaviour
{
    public int score, citiesCollected, templeCollected, 
        buildings, zepps, zeppTotatlRating, 
        buildsLevel, zeppLevel, researchedZepps, burnedFuel;

    public GameManager home;

    public Text Stats, GameOverStats;

    public void SaveStats()
    {
        zepps = home.PlayerA.boats.Count;
        PlayerPrefs.SetInt("score", PlayerPrefs.GetInt("cities") + citiesCollected + templeCollected + zepps + home.PlayerA.techCenter.TRating_Zeps + buildings + zeppLevel + buildsLevel);
        PlayerPrefs.SetInt("cities", PlayerPrefs.GetInt("temples") + citiesCollected);
        PlayerPrefs.SetInt("temples", PlayerPrefs.GetInt("buildings") + templeCollected);
        PlayerPrefs.SetInt("buildings", PlayerPrefs.GetInt("buildings") + buildings);
        PlayerPrefs.SetInt("time", PlayerPrefs.GetInt("time") + (int)Time.timeSinceLevelLoad);
        PlayerPrefs.SetInt("zepps", PlayerPrefs.GetInt("zepps") + zepps);
        PlayerPrefs.SetInt("rating", PlayerPrefs.GetInt("rating") + home.PlayerA.techCenter.TRating_Zeps);
        PlayerPrefs.SetInt("buildlvl", PlayerPrefs.GetInt("buildlvl") + buildsLevel);
        PlayerPrefs.SetInt("zeppslvl", PlayerPrefs.GetInt("zeppslvl") + zeppLevel);
        PlayerPrefs.SetInt("rszepps", PlayerPrefs.GetInt("rszepps") + home.PlayerA.techCenter.researchs_Zeps);
        PlayerPrefs.SetInt("fuel", PlayerPrefs.GetInt("fuel") + burnedFuel);
    }

    public void SetUpStatsPanel()
    {
        zepps = home.PlayerA.boats.Count;

        string st = "Score: "; score = citiesCollected + templeCollected + zepps + home.PlayerA.techCenter.TRating_Zeps + buildings + zeppLevel + buildsLevel; st += score;

        st += "\nCollected cities: " + citiesCollected;
        st += "\nCollected temples: " + templeCollected;
        st += "\nBuildings: " + buildings;

        int t = (int)Time.timeSinceLevelLoad;
        int hours = Mathf.FloorToInt(t / 3600f); string h; if (hours >= 10) { h = hours.ToString(); } else { h = "0" + hours.ToString(); }
        int minutes = Mathf.FloorToInt((t - hours * 3600) / 60f); string m; if (minutes >= 10) { m = minutes.ToString(); } else { m = "0" + minutes.ToString(); }
        int seconds = t - hours * 3600 - minutes * 60; string s; if (seconds >= 10) { s = seconds.ToString(); } else { s = "0" + seconds.ToString(); }

        st += "\nTime: " + h + ":" + m + ":" + s;
        st += "\nZeppelins: " + zepps;
        st += "\nZeppelin total rating: " + home.PlayerA.techCenter.TRating_Zeps;
        st += "\nTotal builds level: " + buildsLevel;
        st += "\nTotal zeppelin level: " + zeppLevel;
        st += "\nResearched zeppelins: " + home.PlayerA.techCenter.researchs_Zeps;
        st += "\nBurned fuel: " + burnedFuel; 

        Stats.text = st; GameOverStats.text = st;
    }

    public void LoadMaineMenu()
    {
        SaveStats();
        SceneManager.LoadScene(0);
    }
}
