using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spacetaurant.Crafting
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Recipe")]
    public class RecipeSO : ScriptableObject
    {
        [HorizontalGroup("Title")]
        [BoxGroup("Title/Info", showLabel: false)]
        [LabelText("Name"), LabelWidth(60)]
        [SerializeField]
        private string _resourceName;
        public string ResourceName => _resourceName;


        [BoxGroup("Title/Info", showLabel: false)]
        [TextArea]
        [SerializeField]
        private string _description;
        public string Description => _description;

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

        [SerializeField]
        private List<ResourceSlot> resourceCost;

        [SerializeField, EnumToggleButtons, HideLabel, Title("Rarity", bold: false, horizontalLine: false)]
        private ResourceRarity _rarity;
        public ResourceRarity Rarity => _rarity;
    }
}
