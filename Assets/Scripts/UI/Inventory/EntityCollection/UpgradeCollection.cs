using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spacetaurant
{

    [Flags] public enum Filter { Available = 1, Unlocked = 2 }
    public class UpgradeCollection : ComponentCollection<UpgradeButton>
    {
        [SerializeField, EnumToggleButtons]
        private Filter _filters;

        [SerializeField, ListDrawerSettings(Expanded = true)]
        private UpgradesSorter[] _sorters;

        protected override Sorter<UpgradeButton>[] Sorters => _sorters;

        protected override bool CheckFilters(UpgradeButton slot)
        {
            bool filterResult = base.CheckFilters(slot);

            if (!filterResult)
                return false;

            foreach (var filter in Enum.GetValues(typeof(Filter)))
            {
                if (!_filters.HasFlag((Filter)filter))
                    continue;

                switch (filter)
                {
                    case Filter.Available:
                        filterResult = slot.UpgradeSO.Available;
                        break;
                    case Filter.Unlocked:
                        filterResult = slot.UpgradeSO.Unlocked;
                        break;
                }

                if (!filterResult)
                    return false;
            }
            return true;
        }
        [Button]
        private void LoadButton()
        {
            _collection.ForEach((x) => x.Load());
        }
    }
    [Serializable, InlineProperty, HideLabel]
    public class UpgradesSorter : Sorter<UpgradeButton>
    {
        public enum Sort { Price, Name, Unlocked, Available }
        [SerializeField, SorterEnum]
        private Sort _sort;
        protected override Comparison<UpgradeButton> GetComparer()
        {
            switch (_sort)
            {
                case Sort.Price:
                    return (a, b) => a.UpgradeSO.Price.CompareTo(b.UpgradeSO.Price);
                case Sort.Name:
                    return (a, b) => a.UpgradeSO.Name.CompareTo(b.UpgradeSO.Name);
                case Sort.Unlocked:
                    return (a, b) => a.UpgradeSO.Unlocked.CompareTo(b.UpgradeSO.Unlocked);
                case Sort.Available:
                    return (a, b) => a.UpgradeSO.Available.CompareTo(b.UpgradeSO.Available);
                default:
                    return (a, b) => 0;
            }
        }
    }
}
