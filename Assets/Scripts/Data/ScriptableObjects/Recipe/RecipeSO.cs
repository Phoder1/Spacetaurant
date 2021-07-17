using DataSaving;
using Sirenix.OdinInspector;
using Spacetaurant.Containers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spacetaurant.Crafting
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Recipe")]
    public class RecipeSO : ItemSO
    {
        [SerializeField, ListDrawerSettings(Expanded = true)]
        private List<ResourceSlot> resourceCost;
        public List<ResourceSlot> ResourceCost => resourceCost;

        public bool CanMake => DataHandler.Load<PlayerInventory>().CanMake(this);
    }
    [Serializable]
    [InlineProperty]
    public class RecipeSlot : Slot<RecipeSO>
    {
        public RecipeSlot(int amount, RecipeSO item) : base(amount, item)
        {
        }
        public RecipeSO Recipe => Item;
    }
}
