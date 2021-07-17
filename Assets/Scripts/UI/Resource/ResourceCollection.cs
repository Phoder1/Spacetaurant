using Sirenix.OdinInspector;
using Spacetaurant.Crafting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spacetaurant
{
    public class ResourceCollection : ButtonCollection<ResourceSlotButton, ResourceSlot>
    {
        protected override Sorter<ResourceSlotButton>[] Sorters => null;

        [Serializable, InlineProperty, HideLabel]
        public class RecipeSorter : Sorter<ResourceSlotButton>
        {
            public enum Sort { Name, Amount, Rarity, Planet, Type }
            [SerializeField, SorterEnum]
            private Sort _sort;
            protected override Comparison<ResourceSlotButton> GetComparer()
            {
                switch (_sort)
                {
                    case Sort.Name:
                        return (a, b) => a.Content.Item.Name.CompareTo(b.Content.Item.Name);
                    case Sort.Amount:
                        return (a, b) => a.Content.Amount.CompareTo(b.Content.Amount);
                    case Sort.Rarity:
                        return (a, b) => a.Content.Item.Rarity.CompareTo(b.Content.Item.Rarity);
                    case Sort.Planet:
                        return (a, b) => a.Content.Item.Planet.PlanetNum.CompareTo(b.Content.Item.Planet.PlanetNum);
                    case Sort.Type:
                        return (a, b) => a.Content.Item.Type.CompareTo(b.Content.Item.Type);
                    default:
                        return (a, b) => 0;
                }
            }
        }
    }
}
