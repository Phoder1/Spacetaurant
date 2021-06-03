using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Spacetaurant
{
    public abstract class GatherableUiSlot<T> : MonoBehaviour
    {
        [SerializeField]
        protected T _gatherable;

        [SerializeField, SceneObjectsOnly, FoldoutGroup("Refrences", 99)]
        protected TextMeshProUGUI _nameText;

        [SerializeField, SceneObjectsOnly, FoldoutGroup("Refrences", 99)]
        protected RarityStars _rarityStars;

        [SerializeField, SceneObjectsOnly, FoldoutGroup("Refrences")]
        protected Image _icon;

        [SerializeField, SceneObjectsOnly, FoldoutGroup("Refrences")]
        protected Image _planetIcon;

        [SerializeField, SceneObjectsOnly, FoldoutGroup("Refrences")]
        protected TextMeshProUGUI _descriptionText;

        [SerializeField, FoldoutGroup("Events", 98)]
        protected UnityEventForRefrence OnWasPressed;
        public void LoadResource(object resource)
        {
            if (resource is T _gatherable)
                LoadResource(_gatherable);
            else
                Debug.LogError("Wrong object type passed!");
        }
        public abstract void LoadResource(T resource);

        public virtual void WasPressed() => OnWasPressed?.Invoke(_gatherable);
    }
}
