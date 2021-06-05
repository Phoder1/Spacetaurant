using Sirenix.OdinInspector;
using Spacetaurant.Planets;
using System;
using UnityEngine;

namespace Spacetaurant.Crafting
{
    public enum ResourceRarity { Common, Uncommon, Rare }
    public enum ResourceType { Cooking, Building }
    [CreateAssetMenu(menuName = "ScriptableObjects/Resource")]
    public class ResourceSO : ItemSO
    {
        [SerializeField, EnumToggleButtons, HideLabel, Title("Type", bold: false, horizontalLine: false)]
        private ResourceType _type;
        public ResourceType Type => _type;
    }
    [Serializable]
    [InlineProperty]
    public class ResourceSlot : ItemSlot<ResourceSO>
    {
        public ResourceSlot(int amount, ResourceSO item) : base(amount, item)
        {
        }
        public ResourceSO Resource => Item;
    }
}
