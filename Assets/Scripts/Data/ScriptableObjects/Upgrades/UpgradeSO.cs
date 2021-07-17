using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spacetaurant
{
    public enum UpgradeType { Restaurant, Staff, Garage, Inventory}
    [CreateAssetMenu(menuName = SoUtillities.SoMenuName + "Upgrade")]
    [InlineEditor]
    public class UpgradeSO : EntitySO
    {
        [BoxGroup("Title/Info", showLabel: false)]
        [SerializeField]
        private int _price;
        public int Price => _price;

        [HorizontalGroup("Title", width: 60)]
        [BoxGroup("Title/Icon")]
        [PreviewField, HideLabel]
        [SerializeField]
        private Sprite _icon;
        public Sprite Icon => _icon;

        [BoxGroup("Title/Info")]
        [SerializeField]
        private UpgradeType _type;

        [SerializeField, ListDrawerSettings(Expanded = true)]
        private UpgradeSO[] _prerequisites;
        public UpgradeSO[] Prerequisites => _prerequisites;

        public bool Available => Prerequisites == null || Prerequisites.Length == 0 || Array.TrueForAll(Prerequisites, (x) => x.Unlocked);
        public bool Unlocked => true;
        public UpgradeType Type => _type;
    }
}
