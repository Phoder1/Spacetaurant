using Sirenix.OdinInspector;
using Spacetaurant.Crafting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spacetaurant
{
    public class RecipeCollection : ButtonCollection<RecipeButton, RecipeSO>
    {
        [SerializeField, ListDrawerSettings(Expanded = true)]
        private RecipeSorter[] _sorters;
        protected override Sorter<RecipeButton>[] Sorters => _sorters;

        [Serializable, InlineProperty, HideLabel]
        public class RecipeSorter : Sorter<RecipeButton>
        {
            public enum Sort { Name, CanMake, Rarity, Planet}
            [SerializeField, SorterEnum]
            private Sort _sort;
            protected override Comparison<RecipeButton> GetComparer()
            {
                switch (_sort)
                {
                    case Sort.Name:
                        return (a, b) => a.Content.Name.CompareTo(b.Content.Name);
                    case Sort.CanMake:
                        return (a, b) => a.Content.CanMake.CompareTo(b.Content.CanMake);
                    case Sort.Rarity:
                        return (a, b) => a.Content.Rarity.CompareTo(b.Content.Rarity);
                    case Sort.Planet:
                        return (a, b) => a.Content.Planet.PlanetNum.CompareTo(b.Content.Planet.PlanetNum);
                    default:
                        return (a, b) => 0;
                }
            }
        }
    }
}
