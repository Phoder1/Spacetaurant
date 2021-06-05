using DataSaving;
using Spacetaurant.Containers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace Spacetaurant
{
    public class InventoryCallbacker : MonoBehaviour
    {
        private PlayerInventory _playerInventory;

        [SerializeField, FoldoutGroup("Events")]
        private UnityEventForRefrence UpdateInventoryEvent;


        private void Awake()
        {
            _playerInventory = DataHandler.GetData<PlayerInventory>();

            UpdateInventory();
        }
        private void OnEnable()
        {
            UpdateInventory();
            _playerInventory.Container.OnValueChange += UpdateInventory;
        }
        private void OnDisable()
        {
            _playerInventory.Container.OnValueChange -= UpdateInventory;
        }

        private void UpdateInventory()
        {
            UpdateInventoryEvent?.Invoke(_playerInventory.Container);
        }
    }
}
