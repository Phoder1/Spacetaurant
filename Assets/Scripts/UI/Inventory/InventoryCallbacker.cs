using DataSaving;
using Sirenix.OdinInspector;
using Spacetaurant.Containers;
using UnityEngine;
using UnityEngine.Events;

namespace Spacetaurant
{
    public class InventoryCallbacker : MonoBehaviour
    {
        [SerializeField]
        private bool _updateOnEnable = true;
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
            if (_updateOnEnable)
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
