using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Spacetaurant
{
    public abstract class GatherableUiSlot<T> : MonoBehaviour
    {
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
        public abstract void LoadResource(T resource);
    }
}
