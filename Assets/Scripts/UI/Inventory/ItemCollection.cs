using Sirenix.OdinInspector;
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
    public abstract class ItemCollection<TUiSlot, TSlot, T> : MonoBehaviour
        where TUiSlot : ItemUiSlot<TSlot>
        where TSlot : ItemSlot<T>
        where T : ItemSO
    {
        [SerializeField, AssetsOnly]
        protected TUiSlot _itemSlotPrefab;

        [SerializeField, SceneObjectsOnly]
        protected List<TUiSlot> _itemUiSlots;
        [SerializeField, EnumToggleButtons]
        protected SortingType _defaultSortMethod = SortingType.Planet;

        [SerializeField]
        protected bool _selectFirstSlotOnStart = true;

        private void Start()
        {
            if (_selectFirstSlotOnStart && _itemUiSlots.Count > 0)
                _itemUiSlots[0].WasPressed();

            Sort(_defaultSortMethod);
        }
        public void SetCollection(List<TSlot> resourceSlots)
        {
            if (_itemUiSlots == null)
                _itemUiSlots = new List<TUiSlot>();

            UpdateCollection(resourceSlots);
        }

        private void UpdateCollection(List<TSlot> resourceSlots)
        {
            if (_itemUiSlots.Count > resourceSlots.Count)
            {
                for (int i = resourceSlots.Count - 1; i < _itemUiSlots.Count; i++)
                {
                    _itemUiSlots[i].gameObject.SetActive(false);
                }
            }

            for (int i = 0; i < resourceSlots.Count; i++)
            {

                var currentItemSlot = resourceSlots[i];

                if (_itemUiSlots.Count <= i)
                {
                    var newSlot = Instantiate(_itemSlotPrefab, transform);
                    _itemUiSlots.Add(newSlot.GetComponent<TUiSlot>());
                }

                var currentUiSlot = _itemUiSlots[i];
                currentUiSlot.gameObject.SetActive(true);

                currentUiSlot.LoadItem(currentItemSlot);
            }
        }

        #region sorting
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
            => a.ItemSlot.Item.Planet.PlanetNum.CompareTo(b.ItemSlot.Item.Planet.PlanetNum);
        #endregion
    }
}