using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace Spacetaurant
{
    [RequireComponent(typeof(OnClick))]
    public class UpgradeButton : MonoBehaviour
    {
        [SerializeField]
        private UpgradeSO _upgradeSO;
        [SerializeField]
        private bool _loadOnStart = false;

        #region Refrences

        #endregion

        #region Events
        [SerializeField, EventsGroup]
        private UnityEvent<UpgradeSO> OnLoad;
        [EventsGroup]
        public UnityEvent<UpgradeSO> OnPress;
        #endregion

        public UpgradeSO UpgradeSO => _upgradeSO;
        private void Awake()
        {
            GetComponent<OnClick>().OnTrigger.AddListener(() => OnPress?.Invoke(_upgradeSO));
        }

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

            OnLoad?.Invoke(UpgradeSO);
        }
    }
}
