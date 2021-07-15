using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataSaving;
using System;
using Spacetaurant.Crafting;

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

        public bool CanMake(RecipeSO recipe) => recipe.ResourceCost.TrueForAll(HasEnough);
        public bool HasEnough(ResourceSlot resourceSlot) 
        {
            var inventorySlot = _container.collection.Find((x) => x.Resource == resourceSlot.Resource);
            return inventorySlot != null && inventorySlot.Amount >= resourceSlot.Amount;
        }
    }
}
