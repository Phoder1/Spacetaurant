using DataSaving;
using Sirenix.OdinInspector;
using Spacetaurant.Containers;
using Spacetaurant.Crafting;
using TMPro;
using UnityEngine;

namespace Spacetaurant
{
    public class IngredientUiSlot : GatherableUiSlot<ResourceSlot>
    {
        [SerializeField]
        private ResourceSlot _resource;

        [SerializeField, SceneObjectsOnly, FoldoutGroup("Refrences")]
        protected TextMeshProUGUI _amountText;


        private void Start()
        {
            if (_resource != null)
            {
                int index;
                var playerInventory = DataHandler.GetData<PlayerInventory>();
                if ((index = playerInventory.Container.FindIndex((x) => x == _resource)) >= 0)
                    LoadResource(playerInventory.Container[index]);
                else
                {
                    playerInventory.Add(_resource);
                    LoadResource(playerInventory.Container[playerInventory.Container.Count - 1]);
                }
            }
        }
        public override void LoadResource(ResourceSlot resource)
        {
            if (_rarityStars != null)
                _rarityStars.SetRarity(resource.Resource.Rarity);

            if (_amountText != null)
                _amountText.text = resource.Amount.ToString();

            if (_icon != null)
                _icon.sprite = resource.Resource.Icon;

            if (_planetIcon != null)
                _planetIcon.sprite = resource.Resource.Planet.Icon;

            if (_descriptionText != null)
                _descriptionText.text = resource.Resource.Description;
        }
    }
}
