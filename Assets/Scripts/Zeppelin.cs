using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zeppelin : MonoBehaviour
{
    public GameObject zooming;

    public GameObject anim;

    public int ID, slodID, inListID;

    public Player home;

    public int Level;

    private void OnTriggerEnter(Collider obj)
    {
        if (obj.gameObject.tag == "temple")
        {
            home.Manager.statsTr.templeCollected++;
            home.Manager.Techs++;
            home.Manager.UpdateTechCount();

            int ChunckX = Mathf.FloorToInt(obj.transform.position.x / 100) * 100, ChunckZ = Mathf.FloorToInt(obj.transform.position.z / 100) * 100;

            string chunckKey = (ChunckX / 100).ToString() + "_" + (ChunckZ / 100).ToString();
            string structKey = obj.gameObject.name;

            if (home.Home.Modifications.ContainsKey(chunckKey))
            {
                home.Home.Modifications[chunckKey].Add(structKey);
            }
            else
            {
                home.Home.Modifications.Add(chunckKey, new List<string> { structKey });
            }

            Destroy(obj.gameObject);
        }
    }
}
