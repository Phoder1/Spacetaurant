using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public static MenuController MC;

    public GameObject shopPannel;

    public GameObject[] buttonLock;
    public Button[] unlockedButtons;

    private void OnEnable()
    {
        MC = this;
    }

    private void Start()
    {
        SetUpStore();
    }

    public void SetUpStore()
    {
        for (int i = 0; i < PersistentData.PD.allskins.Length; i++)
        {
            buttonLock[i].SetActive(!PersistentData.PD.allskins[i]);
            unlockedButtons[i].interactable = PersistentData.PD.allskins[i];
        }
    }


    //public void UnlockSkin(int index)
    //{
    //    PersistentData.PD.allskins[index] = true;
    //    PlayfabController.PFC.SetUserData(PersistentData.PD.skinsDataToString());
    //    SetUpStore();

    //}
    public void Update_Data()
    {
        
        PlayfabController.PFC.SetUserData();
    }


    public void OpenShop()
    {
        shopPannel.SetActive(true);
    }
    public void SetMySkin(int whichSkin)
    {
        PersistentData.PD.mySkin = whichSkin;
    }


}
