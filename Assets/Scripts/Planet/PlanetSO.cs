using Sirenix.OdinInspector;
using Spacetaurant.Crafting;
using System.Collections.Generic;
using UnityEngine;

namespace Spacetaurant.Planets
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Planet")]
    public class PlanetSO : ScriptableObject
    {
        [HorizontalGroup("Title")]
        [BoxGroup("Title/Name", showLabel: false)]
        [LabelText("Name"), LabelWidth(60)]
        [SerializeField]
        private string _planetName;
        public string PlanetName => _planetName;

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

        [SerializeField]
        public List<ResourceSO> possibleResources;
    }
}
