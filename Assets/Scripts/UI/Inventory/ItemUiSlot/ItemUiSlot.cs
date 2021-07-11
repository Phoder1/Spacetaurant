using DataSaving;
using Sirenix.OdinInspector;
using Spacetaurant.Containers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Spacetaurant
{
    public abstract class ItemUiSlot<TSlot,T> : MonoBehaviour, IUiItem<TSlot>
        where TSlot : ItemSlot<T>
        where T : ItemSO
    {
        [SerializeField]
        protected TSlot _itemSlot;
        public TSlot ItemSlot => _itemSlot;

        [SerializeField, SceneObjectsOnly, FoldoutGroup("Refrences", 99)]
        private TextMeshProUGUI _nameText;

        [SerializeField, SceneObjectsOnly, FoldoutGroup("Refrences", 99)]
        private RarityStars _rarityStars;

        [SerializeField, SceneObjectsOnly, FoldoutGroup("Refrences")]
        private Image _icon;

        [SerializeField, SceneObjectsOnly, FoldoutGroup("Refrences")]
        private Image _planetIcon;

        [SerializeField, SceneObjectsOnly, FoldoutGroup("Refrences")]
        private TextMeshProUGUI _descriptionText;

        [SerializeField, SceneObjectsOnly, FoldoutGroup("Refrences")]
        private TextMeshProUGUI _amountText;



        [FoldoutGroup("Events", 98)]
        public UnityEventForRefrence OnWasPressed;
        public void LoadItem(object item)
        {
            if (item is TSlot _item)
                LoadItem(_item);
            else
                Debug.LogError("Wrong object type passed!");
        }

        public virtual void WasPressed() => OnWasPressed?.Invoke(_itemSlot);
        protected virtual void Start()
        {
            if (_itemSlot != null && _itemSlot.Item != null)
            {
                int index;
                var playerInventory = DataHandler.Load<PlayerInventory>();

                if ((index = playerInventory.Container.FindIndex((x) => x.Resource == _itemSlot.Item)) >= 0)
                    LoadItem(playerInventory.Container[index]);
                else
                {
                    LoadItem(_itemSlot);
                    //LoadItem(playerInventory.Container[playerInventory.Container.Count - 1]);
                }
            }
        }
        public virtual void LoadItem(TSlot itemSlot)
        {
            _itemSlot = itemSlot;

            T recipeSO = itemSlot.Item;

            if (_nameText != null)
                _nameText.text = recipeSO.Name;

            if (_rarityStars != null)
                _rarityStars.SetRarity(recipeSO.Rarity);

            if (_icon != null)
            {
                _icon.gameObject.SetActive(true);
                _icon.sprite = recipeSO.Icon;
            }

            if (_descriptionText != null)
                _descriptionText.text = recipeSO.Description;

            if (_planetIcon != null && recipeSO.Planet != null)
                _planetIcon.sprite = recipeSO.Planet.Icon;

            if (_amountText != null)
                _amountText.text = itemSlot.Amount.ToString();
        }
    }
}
