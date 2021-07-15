using DataSaving;
using Sirenix.OdinInspector;
using Spacetaurant.Containers;
using Spacetaurant.Crafting;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Spacetaurant
{
    public class ResourceSlotInfo : SlotLoader<ResourceSO,ResourceSlot>
    {
        [SerializeField, SceneObjectsOnly, RefrencesGroup]
        private TextMeshProUGUI _amountAvailable;
        [SerializeField, SceneObjectsOnly, RefrencesGroup]
        private TextMeshProUGUI _amountOutOfAvailable;
        [SerializeField, SceneObjectsOnly, RefrencesGroup]
        private BoolPassthrough _hasEnough;
        public override void Load(ResourceSlot info)
        {
            base.Load(info);

            ResourceSlot inventoryResourceSlot = DataHandler.Load<PlayerInventory>().Container.collection.Find((x) => x.Resource == info.Resource);
            int amount = inventoryResourceSlot == null ? 0 : inventoryResourceSlot.Amount;

            if (_amountAvailable != null)
                _amountAvailable.text = amount.ToString();
            if (_amountOutOfAvailable != null)
                _amountOutOfAvailable.text = info.Amount.ToString() + "/" + amount.ToString();

            if (_hasEnough != null)
                _hasEnough.Trigger(info.Amount <= amount);
        }
    }
}
