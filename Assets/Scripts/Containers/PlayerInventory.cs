using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataSaving;

namespace Spacetaurant.Containers
{
    [System.Serializable]
    public class PlayerInventory : BaseInventory, ISaveable 
    {
        static PlayerInventory() => LoadingManager.CacheAllData += Cache;
        private static void Cache() => DataHandler.Load<PlayerInventory>();
    }
}
