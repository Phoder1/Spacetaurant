using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentData : MonoBehaviour
{
    public static PersistentData PD;

    public bool[] allskins;
    
    public int mySkin;

    private void OnEnable()
    {
        PersistentData.PD = this;
    }

    public void skinsStringToData(string skinsIn)
    {
        for (int i = 0; i < skinsIn.Length; i++)
        {
            if (int.Parse(skinsIn[i].ToString()) > 0)
            {
                allskins[i] = true;
            }
            else
            {
                allskins[i] = false;
            }

        }

        MenuController.MC.SetUpStore();
    }

    public string skinsDataToString()
    {
        string toString = "";

        for (int i = 0; i < allskins.Length; i++)
        {
            if(allskins[i] == true) 
            {
                toString += "1";
            }
            else
            {
                toString += "0";
            }

        }
        return toString;
    }

}
