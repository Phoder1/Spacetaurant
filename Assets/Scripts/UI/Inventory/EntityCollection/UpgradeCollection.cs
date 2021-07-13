using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spacetaurant
{

    [Flags] public enum Filter { Available = 1, Unlocked = 2, Restaurant = 4, Stuff = 8, Garage = 16  }
    public class UpgradeCollection : ComponentCollection<UpgradeButton>
    {
        [SerializeField, EnumToggleButtons, LabelWidth(40)]
        private Filter _filters;

        [SerializeField, ListDrawerSettings(Expanded = true)]
        private UpgradesSorter[] _sorters;
        protected override Sorter<UpgradeButton>[] Sorters => _sorters;

        [SerializeField]
        private UpgradeInfo _infoPanel;
        public UpgradeInfo InfoPanel => _infoPanel;
        private void Awake()
        {
            _collection.ForEach((x) => { if (x != null) x.OnPress += InfoPanel.Load; } );
        }
        protected override void OnInstantiation(UpgradeButton item)
        {
            base.OnInstantiation(item);
            item.OnPress += InfoPanel.Load;
        }
        public override void ReloadCollection()
        {
            base.ReloadCollection();

            _collection.ForEach((x) => x.Load());
        }
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
                    case Filter.Restaurant:
                        filterResult = slot.UpgradeSO.Type == UpgradeType.Restaurant;
                        break;
                    case Filter.Stuff:
                        filterResult = slot.UpgradeSO.Type == UpgradeType.Stuff;
                        break;
                    case Filter.Garage:
                        filterResult = slot.UpgradeSO.Type == UpgradeType.Garage;
                        break;

                }

                if (!filterResult)
                    return false;
            }
            return true;
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
