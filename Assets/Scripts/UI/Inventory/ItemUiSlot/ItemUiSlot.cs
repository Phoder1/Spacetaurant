using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Spacetaurant
{
    public abstract class ItemUiSlot<T> : MonoBehaviour
    {
        [SerializeField]
        protected T _itemSlot;
        public T ItemSlot => _itemSlot;

        [SerializeField, SceneObjectsOnly, FoldoutGroup("Refrences", 99)]
        protected TextMeshProUGUI _nameText;
        public TextMeshProUGUI Name => _nameText;

        [SerializeField, SceneObjectsOnly, FoldoutGroup("Refrences", 99)]
        protected RarityStars _rarityStars;
        public RarityStars RarityStars => _rarityStars;

        [SerializeField, SceneObjectsOnly, FoldoutGroup("Refrences")]
        protected Image _icon;
        public Image Icon => _icon;

        [SerializeField, SceneObjectsOnly, FoldoutGroup("Refrences")]
        protected Image _planetIcon;
        public Image PlanetIcon => _planetIcon;

        [SerializeField, SceneObjectsOnly, FoldoutGroup("Refrences")]
        protected TextMeshProUGUI _descriptionText;
        public TextMeshProUGUI DescriptionText => _descriptionText;

        [FoldoutGroup("Events", 98)]
        public UnityEventForRefrence OnWasPressed;
        public void LoadItem(object item)
        {
            if (item is T _item)
                LoadItem(_item);
            else
                Debug.LogError("Wrong object type passed!");
        }
        public abstract void LoadItem(T item);

        public virtual void WasPressed() => OnWasPressed?.Invoke(_itemSlot);
    }
}
