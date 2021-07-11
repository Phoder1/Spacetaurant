using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Spacetaurant
{
    public class UpgradeButton : MonoBehaviour
    {
        [SerializeField]
        private UpgradeSO _upgradeSO;
        [SerializeField]
        private bool _loadOnStart = false;

        #region Refrences
        [SerializeField, FoldoutGroup("Refrences")]
        private Image _icon;
        #endregion

        public UpgradeSO UpgradeSO => _upgradeSO;

        private void Start()
        {
            if (_loadOnStart)
                Load();
        }

        [Button]
        public void Load()
        {
            if (UpgradeSO == null)
                return;

            gameObject.name = UpgradeSO.name;

            if (_icon != null)
                _icon.sprite = UpgradeSO.Icon;



        }
    }
}
