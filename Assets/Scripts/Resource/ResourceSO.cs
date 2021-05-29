using Sirenix.OdinInspector;
using UnityEngine;
using System;
using DataSaving;
using Spacetaurant.Planets;

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
        [SerializeField]
        private string _resourceName;
        public string ResourceName => _resourceName;

        [HorizontalGroup("Title", width: 60)]
        [BoxGroup("Title/Icon")]
        [PreviewField, HideLabel]
        [SerializeField]
        private Sprite _icon;
        public Sprite Icon => _icon;

        [BoxGroup("Title/Name", showLabel: false)]
        [TextArea]
        [SerializeField]
        private string _description;
        public string Description => _description;

        [SerializeField, EnumToggleButtons, HideLabel, Title("Rarity", bold: false, horizontalLine: false)]
        private ResourceRarity _rarity;
        public ResourceRarity Rarity => _rarity;

        [SerializeField, EnumToggleButtons, HideLabel, Title("Type", bold: false, horizontalLine: false)]
        private ResourceType _type;
        public ResourceType Type => _type;

        [SerializeField]
        private PlanetSO _planet;
        public PlanetSO Planet => _planet;
    }
    [Serializable]
    [InlineProperty]
    public class ResourceSlot : DirtyData
    {
        [SerializeField, HorizontalGroup, HideLabel]
        private int _amount;
        public int Amount
        {
            get => _amount;
            set => Setter(ref _amount, value);
        }
        [SerializeField, HorizontalGroup, HideLabel]
        private ResourceSO _resource;
        public ResourceSO Resource
        {
            get => _resource;
            set => Setter(ref _resource, value);
        }

        public ResourceSlot(int amount, ResourceSO resource)
        {
            _amount = amount;
            _resource = resource;
            IsDirty = true;
        }
    }
}
