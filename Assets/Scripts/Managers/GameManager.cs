using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataSaving;

namespace Spacetaurant
{
    public class GameManager : MonoSingleton<GameManager>
    {
        private void OnApplicationQuit()
        {
            DataHandler.SaveAll();
        }
    }
}
