using DataSaving;
using Sirenix.OdinInspector;
using Spacetaurant.Crafting;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spacetaurant.UI
{
    public enum SortingType
    {
        Planet,
        Rarity,
        Amount,
        Name
    }
    [Flags]
    public enum TypeFilter
    {
        CookingResources = 1,
        BuildingResources = 2,
    }
    [Flags]
    public enum RarityFilter
    {
        Rares = 1,
        Uncommon = 2,
        Common = 4,
    }
    public abstract class ItemCollection<TUiSlot, TSlot, T> : MonoBehaviour
        where TUiSlot : ItemUiSlot<TSlot, T>
        where TSlot : Slot<T>
        where T : ItemSO
    {
        [SerializeField, AssetsOnly]
        protected TUiSlot _itemSlotPrefab;

        [SerializeField, EnumToggleButtons]
        protected SortingType _sortMethod = SortingType.Planet;

        [SerializeField, EnumToggleButtons, BoxGroup("Filters")]
        private RarityFilter _rarityFilter = (RarityFilter)~0;

        [SerializeField]
        protected bool _selectFirstSlotOnStart = true;

        protected List<TUiSlot> _itemUiSlots;
        private void Awake()
        {
            _itemUiSlots = new List<TUiSlot>();
            GetComponentsInChildren(true, _itemUiSlots);
        }
        private void Start()
        {
            if (_itemUiSlots == null)
                _itemUiSlots = new List<TUiSlot>();

            SetCollection(_itemUiSlots.ConvertAll((x) => x.ItemSlot), _selectFirstSlotOnStart);
        }

        private void SelectFirstSlot()
        {
            if (_itemUiSlots.Count > 0)
                _itemUiSlots.Find((x) => x.gameObject.activeSelf)?.WasPressed();
        }

        public void SetCollection(object resourceSlots)
        {
            switch (resourceSlots)
            {
                case List<TSlot> _list:
                    SetCollection(_list, true);
                    break;
                case DirtyDataList<TSlot> _dirtyList:
                    SetCollection(_dirtyList, true);
                    break;
            }
        }
        public void SetCollection(DirtyDataList<TSlot> resourceSlots, bool selectFirstSlot)
            => SetCollection(resourceSlots.collection, selectFirstSlot);
        public void SetCollection(List<TSlot> resourceSlots, bool selectFirstSlot)
        {
            if (_itemUiSlots == null)
                _itemUiSlots = new List<TUiSlot>();

            if (resourceSlots == null)
                return;

            UpdateCollection(resourceSlots);

            Sort(_sortMethod);

            if (selectFirstSlot)
                SelectFirstSlot();
        }

        private void UpdateCollection(List<TSlot> resourceSlots)
        {
            if (_itemUiSlots.Count > resourceSlots.Count)
            {
                for (int i = resourceSlots.Count; i < _itemUiSlots.Count; i++)
                {
                    _itemUiSlots[i].gameObject.SetActive(false);
                }
            }

            if (resourceSlots.Count == 0)
                return;

            for (int i = 0; i < resourceSlots.Count; i++)
            {
                if (resourceSlots[i]?.Item == null || !CheckFilters(resourceSlots[i]))
                {
                    _itemUiSlots[i].gameObject.SetActive(false);
                    continue;
                }

                var currentItemSlot = resourceSlots[i];

                if (_itemUiSlots.Count <= i)
                {
                    var newSlot = Instantiate(_itemSlotPrefab, transform);
                    _itemUiSlots.Add(newSlot);
                }

                var currentUiSlot = _itemUiSlots[i];
                currentUiSlot.gameObject.SetActive(true);

                currentUiSlot.LoadItem(currentItemSlot);
            }
        }

        #region Filtering
        protected virtual bool CheckFilters(TSlot slot)
        {
            if (!CheckRarityFilters(slot))
                return false;

            return true;
        }

        private bool CheckRarityFilters(TSlot slot)
        {
            bool correctRarity = false;
            foreach (var filter in (RarityFilter[])Enum.GetValues(typeof(RarityFilter)))
                if (_rarityFilter.HasFlag(filter) && CheckFilter(slot, filter))
                {
                    correctRarity = true;
                    break;
                }

            return correctRarity;

            bool CheckFilter(TSlot slot, RarityFilter filter)
            {
                switch (filter)
                {
                    case RarityFilter.Rares:
                        return slot.Item.Rarity == ResourceRarity.Rare;
                    case RarityFilter.Uncommon:
                        return slot.Item.Rarity == ResourceRarity.Uncommon;
                    case RarityFilter.Common:
                        return slot.Item.Rarity == ResourceRarity.Common;
                    default:
                        return false;
                }
            }
        }
        #endregion

        #region Sorting
        public void Sort(SortingType sortingType)
        {
            _itemUiSlots.Sort((a, b) => SortByOrder(a, b, GetSortingOrder(sortingType)));

            for (int i = 0; i < _itemUiSlots.Count; i++)
                _itemUiSlots[i].transform.SetSiblingIndex(i);
        }
        private int SortByOrder(TUiSlot a, TUiSlot b, SortingType[] sortingOrder)
        {
            int comparison = 0;

            if (a?.ItemSlot?.Item == null && b?.ItemSlot?.Item != null)
                return -1;
            if (a?.ItemSlot?.Item != null && b?.ItemSlot?.Item == null)
                return 1;
            if (a?.ItemSlot?.Item == null && b?.ItemSlot?.Item == null)
                return 0;

            foreach (var soringType in sortingOrder)
            {
                comparison = GetComparer(soringType)(a, b);

                if (comparison != 0)
                    return comparison;
            }

            return 0;

        }
        protected Comparison<TUiSlot> GetComparer(SortingType sortingType)
        {
            switch (sortingType)
            {
                case SortingType.Planet:
                    return ComparePlanets;
                case SortingType.Rarity:
                    return CompareRarities;
                case SortingType.Amount:
                    return CompareAmounts;
                case SortingType.Name:
                    return CompareNames;
                default:
                    return (a, b) => 0;
            }
        }
        protected SortingType[] GetSortingOrder(SortingType sortingType)
        {
            switch (sortingType)
            {
                case SortingType.Planet:
                    return new SortingType[]
{
                        SortingType.Planet,
                        SortingType.Rarity,
                        SortingType.Name,
                        SortingType.Amount,
};
                case SortingType.Rarity:
                    return new SortingType[]
{
                        SortingType.Rarity,
                        SortingType.Planet,
                        SortingType.Name,
                        SortingType.Amount,
};
                case SortingType.Amount:
                    return new SortingType[]
{
                        SortingType.Amount,
                        SortingType.Planet,
                        SortingType.Rarity,
                        SortingType.Name,
};
                case SortingType.Name:
                    return new SortingType[]
                    {
                        SortingType.Name,
                        SortingType.Planet,
                        SortingType.Rarity,
                        SortingType.Amount,
                    };
                default:
                    return new SortingType[0];
            }
        }

        protected virtual int CompareAmounts(TUiSlot a, TUiSlot b)
            => a.ItemSlot.Amount.CompareTo(b.ItemSlot.Amount);
        protected virtual int CompareNames(TUiSlot a, TUiSlot b)
            => a.ItemSlot.Item.Name.CompareTo(b.ItemSlot.Item.Name);

        protected virtual int ComparePlanets(TUiSlot a, TUiSlot b)
            => a.ItemSlot.Item.Planet.PlanetNum.CompareTo(b.ItemSlot.Item.Planet.PlanetNum);

        protected virtual int CompareRarities(TUiSlot a, TUiSlot b)
            => a.ItemSlot.Item.Rarity.CompareTo(b.ItemSlot.Item.Rarity);
        #endregion
    }
}