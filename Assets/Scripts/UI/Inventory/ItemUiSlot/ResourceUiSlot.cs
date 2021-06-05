using DataSaving;
using Sirenix.OdinInspector;
using Spacetaurant.Containers;
using Spacetaurant.Crafting;
using TMPro;
using UnityEngine;

namespace Spacetaurant
{
    public class ResourceUiSlot : ItemUiSlot<ResourceSlot>
    {
        [SerializeField, SceneObjectsOnly, FoldoutGroup("Refrences")]
        protected TextMeshProUGUI _amountText;


        private void Start()
        {
            if (_itemSlot != null && _itemSlot.Resource != null)
            {
                int index;
                var playerInventory = DataHandler.GetData<PlayerInventory>();
                if ((index = playerInventory.Container.FindIndex((x) => x.Resource == _itemSlot.Resource)) >= 0)
                    LoadItem(playerInventory.Container[index]);
                else
                {
                    playerInventory.Add(_itemSlot);
                    LoadItem(playerInventory.Container[playerInventory.Container.Count - 1]);
                }
            }
        }
        public override void LoadItem(ResourceSlot resource)
        {
            _itemSlot = resource;

            if (_nameText != null)
                _nameText.text = resource.Resource.Name;

            if (_rarityStars != null)
                _rarityStars.SetRarity(resource.Resource.Rarity);

            if (_amountText != null)
                _amountText.text = resource.Amount.ToString();

            if (_icon != null)
            {
                _icon.gameObject.SetActive(true);
                _icon.sprite = resource.Resource.Icon;
            }

            if (_planetIcon != null && resource.Resource.Planet != null)
                _planetIcon.sprite = resource.Resource.Planet.Icon;

            if (_descriptionText != null)
                _descriptionText.text = resource.Resource.Description;
        }
    }
}
