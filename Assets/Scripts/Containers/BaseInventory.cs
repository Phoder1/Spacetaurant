using DataSaving;
using Spacetaurant.Crafting;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Spacetaurant.Containers
{
    [Serializable]
    public abstract class BaseInventory : IDirtyData, IContainer<ResourceSlot>
    {
        [SerializeField]
        protected DirtyDataList<ResourceSlot> _container = new DirtyDataList<ResourceSlot>();
        public DirtyDataList<ResourceSlot> Container => _container;

        private bool _isDirty = true;
        public bool IsDirty
        {
            get => _isDirty || Container.IsDirty;
            private set
            {
                bool changed = IsDirty == value;
                _isDirty = value;

                if (changed)
                    OnDirty?.Invoke();
            }
        }
        public void ValueChanged() => _isDirty = true;

        public event Action OnDirty;

        private UnityEventForRefrence _onValueChange;
        public UnityEventForRefrence OnValueChange => _onValueChange;
        public void Add(ResourceSlot item)
        {
            int index = Container.FindIndex(x => x.Item == item.Item);

            if (index == -1)
                Container.Add(item);
            else
                Container[index].Amount += item.Amount;
        }

        public void Saved()
        {
            Container.Saved();
            _isDirty = false;
        }
        public void Sort(Comparison<ResourceSlot> comparison) => Container.Sort(comparison);
        public bool TryGet(ResourceSO resource, out ResourceSO outputResource)
        {
            outputResource = default;
            int index = Container.FindIndex(x => x.Item == resource);

            if (index != -1)
            {
                outputResource = Container[index].Item;
                return true;
            }

            return false;
        }
        public bool TryGet(ResourceSlot item, out ResourceSlot outputItem)
        {
            outputItem = default;
            int index = Container.FindIndex(x => x == item);

            if (index != -1)
            {
                outputItem = Container[index];
                return true;
            }

            return false;
        }
    }
}
