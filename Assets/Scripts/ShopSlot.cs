using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    public Text Name;
    public Text Cost;
    public Image img;
    public Button changeTarget;

    public GameManager manager;
    public int ID, inShopSlot;

    public Image panel;

    public Color ActiveColor, UnactivedColor;

    public Text Stats;

    public void ChangeTr()
    {
        manager.currentModelToBuy = ID;
        manager.CurrentModel = inShopSlot;
        for (int i = 0; i < manager.Shop.Count; i++)
        {
            manager.Shop[i].panel.color = UnactivedColor;
        }

        for (int i = 0; i < manager.myZepsDetails.Count; i++)
        {
            manager.myZepsDetails[i].UpdateThis();
        }
        panel.color = ActiveColor;

        manager.SelectedZep.text = "Selected zepelin: " + manager.zeppelins[ID].Name + " (Rating: " + manager.zeppelins[ID].rating + ")";
    }
}
