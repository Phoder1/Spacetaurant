using Spacetaurant.Crafting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spacetaurant
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Customer")]
    public class CustomerSO : EntitySO
    {
        [SerializeField]
        private RecipeSO _favoriteRecipe;
        public RecipeSO FavoriteRecipe => _favoriteRecipe;

        public float ChanceWeight
        {
            get
            {
                switch (_favoriteRecipe.Rarity)
                {
                    case ResourceRarity.Common:
                        return 6;
                    case ResourceRarity.Uncommon:
                        return 3;
                    case ResourceRarity.Rare:
                        return 1;
                    default:
                        return 0;
                }
            }
        }

        internal void Left()
        {
        }
    }
}
