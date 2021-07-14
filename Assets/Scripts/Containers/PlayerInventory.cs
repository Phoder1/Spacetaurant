using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataSaving;
using System;

namespace Spacetaurant.Containers
{
    [System.Serializable]
    public class PlayerInventory : BaseInventory, ISaveable
    {
        static PlayerInventory() => LoadingHandler.CacheAllData += Cache;

        public event Action OnSaveStarted;
        public event Action OnSaveFinished;
        private static void Cache() => DataHandler.Load<PlayerInventory>();
        public void BeforeSave() => OnSaveStarted?.Invoke();
        public void AfterSave() => OnSaveFinished?.Invoke();
    }
}
