using DataSaving;
using Sirenix.OdinInspector;
using Spacetaurant.Containers;
using Spacetaurant.Crafting;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Spacetaurant.UI
{
    public class RecipeUiSlot : ItemUiSlot<RecipeSlot>
    {
        [SerializeField, SceneObjectsOnly, FoldoutGroup("Refrences")]
        protected TextMeshProUGUI _amountText;

        [SerializeField, SceneObjectsOnly, FoldoutGroup("Refrences")]
        protected TextMeshProUGUI _valueText;
        [SerializeField, SceneObjectsOnly, FoldoutGroup("Refrences")]
        protected string _valueTextIntFormatting = "";

        [SerializeField, SceneObjectsOnly, FoldoutGroup("Refrences")]
        protected ResourceCollection _recipeCost;


        private void Start()
        {
            if (_itemSlot != null && _itemSlot.Item != null)
            {
                int index;
                var playerInventory = DataHandler.GetData<PlayerInventory>();

                if ((index = playerInventory.Container.FindIndex((x) => x.Resource == _itemSlot.Recipe)) >= 0)
                    LoadItem(playerInventory.Container[index]);
                else
                {
                    LoadItem(_itemSlot);
                    //LoadItem(playerInventory.Container[playerInventory.Container.Count - 1]);
                }
            }
        }
        public override void LoadItem(RecipeSlot recipe)
        {
            _itemSlot = recipe;

            RecipeSO recipeSO = recipe.Recipe;

            if (_nameText != null)
                _nameText.text = recipeSO.Name;

            if (_rarityStars != null)
                _rarityStars.SetRarity(recipeSO.Rarity);

            if (_amountText != null)
                _amountText.text = recipe.Amount.ToString();

            if (_icon != null)
            {
                _icon.gameObject.SetActive(true);
                _icon.sprite = recipeSO.Icon;
            }

            if (_descriptionText != null)
                _descriptionText.text = recipeSO.Description;

            if (_planetIcon != null && recipeSO.Planet != null)
                _planetIcon.sprite = recipeSO.Planet.Icon;

            if (_recipeCost != null)
                _recipeCost.SetCollection(recipeSO.ResourceCost);

            if (_valueText != null)
                _valueText.text = recipeSO.Value.ToString(_valueTextIntFormatting);
        }
    }
}
