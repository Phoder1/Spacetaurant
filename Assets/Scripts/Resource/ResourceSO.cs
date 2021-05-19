using Sirenix.OdinInspector;
using UnityEngine;
using System;
using DataSaving;

namespace Spacetaurant.Resources
{
    public enum ResourceRarity { Common, Uncommon, Rare }
    public enum ResourceType { Cooking, Building }
    [CreateAssetMenu(menuName = "ScriptableObjects/Resource")]
    public class ResourceSO : ScriptableObject
    {
        [HorizontalGroup("Title")]
        [BoxGroup("Title/Name", showLabel: false)]
        [LabelText("Name"), LabelWidth(60)]
        public string resourceName;

        [HorizontalGroup("Title", width: 60)]
        [BoxGroup("Title/Icon")]
        [PreviewField, HideLabel]
        public Sprite icon;

        [BoxGroup("Title/Name", showLabel: false)]
        [TextArea]
        public string description;

        [EnumToggleButtons, HideLabel, Title("Rarity", bold: false, horizontalLine: false)]
        public ResourceRarity rarity;

        [EnumToggleButtons, HideLabel, Title("Type", bold: false, horizontalLine: false)]
        public ResourceType type;
    }
    [Serializable]
    [InlineProperty]
    public class ResourceSlot : IDirtyData
    {
        [SerializeField, HorizontalGroup, HideLabel]
        private int _amount;
        public int Amount
        {
            get => _amount;
            set
            {
                if (_amount == value)
                    return;

                _amount = value;
                ValueChanged();
            }
        }
        [SerializeField, HorizontalGroup, HideLabel]
        private ResourceSO _resource;
        public ResourceSO Resource
        {
            get => _resource;
            set
            {
                if (_resource == value)
                    return;

                _resource = value;
                ValueChanged();
            }
        }

        private bool _isDirty;
        public bool IsDirty => _isDirty;

        public event Action OnDirty;
        public void Saved() => _isDirty = false;
        public void ValueChanged() => _isDirty = true;
    }
}
