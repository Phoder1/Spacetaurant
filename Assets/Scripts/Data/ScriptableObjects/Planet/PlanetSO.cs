using Sirenix.OdinInspector;
using Spacetaurant.Crafting;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;

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
        private int _planetNum = 1;
        public int PlanetNum => _planetNum;

        [FormerlySerializedAs("possibleResources")]
        [ListDrawerSettings(Expanded = true)]
        public List<ResourceSO> resources;
        [ListDrawerSettings(Expanded = true)]
        public List<RecipeSO> recipes;
        [ListDrawerSettings(Expanded = true)]
        public List<CustomerSO> customers;

        public void FillRandomizerWithCustomers(ref Randomizer<CustomerSO> randomizer)
        {
            foreach (var customer in customers)
                randomizer.Add(new Option<CustomerSO>(customer, customer.ChanceWeight));
        }
    }
}
