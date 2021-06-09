using Sirenix.OdinInspector;
using Spacetaurant.Crafting;
using System;
using UnityEngine;

namespace Spacetaurant.UI
{
    public class ResourceCollection : ItemCollection<ResourceUiSlot, ResourceSlot, ResourceSO>
    {
        [SerializeField, EnumToggleButtons, BoxGroup("Filters")]
        private TypeFilter _typeFilter = (TypeFilter)~0;

        protected override bool CheckFilters(ResourceSlot slot)
        {
            if (slot is ResourceSlot resourceSlot && !CheckTypeFilters(resourceSlot))
                return false;

            return base.CheckFilters(slot);
        }

        private bool CheckTypeFilters(ResourceSlot slot)
        {
            bool correctType = false;
            foreach (var filter in (TypeFilter[])Enum.GetValues(typeof(TypeFilter)))
                if (_typeFilter.HasFlag(filter) && CheckFilter(slot, filter))
                {
                    correctType = true;
                    break;
                }

            return correctType;

            bool CheckFilter(ResourceSlot slot, TypeFilter filter)
            {
                switch (filter)
                {
                    case TypeFilter.CookingResources:
                        return slot.Resource.Type == ResourceType.Cooking;
                    case TypeFilter.BuildingResources:
                        return slot.Resource.Type == ResourceType.Building;
                    default:
                        return false;
                }
            }
        }
    }
}
