using DataSaving;
using Sirenix.OdinInspector;
using Spacetaurant.Crafting;
using Spacetaurant.Planets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spacetaurant
{
    public abstract class ItemSO : EntitySO
    {


        [BoxGroup("Title/Info", showLabel: false)]
        [SerializeField]
        private int _value;
        public int Value => _value;

        [HorizontalGroup("Title", width: 60)]
        [BoxGroup("Title/Icon")]
        [PreviewField, HideLabel]
        [SerializeField]
        private Sprite _icon;
        public Sprite Icon => _icon;

        [SerializeField, EnumToggleButtons, HideLabel, Title("Rarity", bold: false, horizontalLine: false)]
        private ResourceRarity _rarity;
        public ResourceRarity Rarity => _rarity;

        [SerializeField]
        private PlanetSO _planet;
        public PlanetSO Planet => _planet;
    }

    [Serializable]
    [InlineProperty]
    public abstract class ItemSlot<T> : DirtyData where T : ItemSO
    {
        [SerializeField, HorizontalGroup, HideLabel]
        private int _amount;
        public int Amount
        {
            get => _amount;
            set => Setter(ref _amount, value);
        }
        [SerializeField, HorizontalGroup, HideLabel]
        private T _item;
        public T Item
        {
            get => _item;
            set => Setter(ref _item, value);
        }

        public ItemSlot(int amount, T item)
        {
            _amount = amount;
            _item = item;
            IsDirty = true;
        }

        public bool Equals(ItemSlot<T> slot)
        {
            return slot == null || Amount != slot.Amount || Item != slot.Item || base.Equals(slot);
        }
    }
}
