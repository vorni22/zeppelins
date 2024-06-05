using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public GameObject chunk;

    public int LandscapeLength;

    public GameObject MainCamera;

    public float speed;

    public GameObject left, right;

    public GameObject loadingPanel;

    public Text stats;

    void Start()
    {
        Time.timeScale = 1;

        float RandomSeedX = Random.Range(0f, 99999f);
        float RandomSeedZ = Random.Range(0f, 99999f);

        for (int i = 0; i < LandscapeLength; i++)
        {
            GameObject newCH = Instantiate(chunk, new Vector3(100 * i, 0, 0), Quaternion.identity);

            MainMenuLandscape land = newCH.GetComponent<MainMenuLandscape>();

            land.seedX = RandomSeedX + i * 200; land.seedZ = RandomSeedZ;

            if (i == 0) { land.right = true; right = newCH; }
            if (i == LandscapeLength - 1) { land.left = true; left = newCH; }

            land.GenerateChunk();
        }

        GameObject MirrorLeft = Instantiate(right, new Vector3(100 * (LandscapeLength), 0, 0), Quaternion.identity);
        //MainMenuLandscape ml = MirrorLeft.GetComponent<MainMenuLandscape>();
        //ml.seedZ = RandomSeedZ; ml.seedX = RandomSeedX;
        //ml.right = true;

        GameObject MirrorRight = Instantiate(left, new Vector3(-100, 0, 0), Quaternion.identity);
        //MainMenuLandscape mr = MirrorRight.GetComponent<MainMenuLandscape>();
        //mr.seedZ = RandomSeedZ; mr.seedX = RandomSeedX + (LandscapeLength - 1) * 200;
        //mr.left = true;
    }

    public void SetUpStats()
    {
        string st = "Score: ";  st += PlayerPrefs.GetInt("score");

        st += "\nCollected cities: " + PlayerPrefs.GetInt("cities");
        st += "\nCollected temples: " + PlayerPrefs.GetInt("temples");
        st += "\nBuildings: " + PlayerPrefs.GetInt("buildings");

        int t = PlayerPrefs.GetInt("time");
        int hours = Mathf.FloorToInt(t / 3600f); string h; if (hours >= 10) { h = hours.ToString(); } else { h = "0" + hours.ToString(); }
        int minutes = Mathf.FloorToInt((t - hours * 3600) / 60f); string m; if (minutes >= 10) { m = minutes.ToString(); } else { m = "0" + minutes.ToString(); }
        int seconds = t - hours * 3600 - minutes * 60; string s; if (seconds >= 10) { s = seconds.ToString(); } else { s = "0" + seconds.ToString(); }

        st += "\nTime: " + h + ":" + m + ":" + s;
        st += "\nZeppelins: " + PlayerPrefs.GetInt("zepps");
        st += "\nZeppelin total rating: " + PlayerPrefs.GetInt("rating");
        st += "\nTotal builds level: " + PlayerPrefs.GetInt("buildlvl");
        st += "\nTotal zeppelin level: " + PlayerPrefs.GetInt("zeppslvl");
        st += "\nResearched zeppelins: " + PlayerPrefs.GetInt("rszepps");
        st += "\nBurned fuel: " + PlayerPrefs.GetInt("fuel");

        stats.text = st;
    }

    private void Update()
    {
        MainCamera.transform.position += new Vector3(speed * Time.deltaTime, 0, 0);

        if (MainCamera.transform.position.x >= LandscapeLength * 100)
        {
            MainCamera.transform.position = new Vector3(0, 26.7f, 1.7f);
        }
    }

    public void NewGame()
    {
        loadingPanel.SetActive(true);
        SceneManager.LoadScene(1);
    }
}
