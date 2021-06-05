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
    public class RecipeUiSlot : ItemUiSlot<RecipeSlot, RecipeSO>
    {


        [SerializeField, SceneObjectsOnly, FoldoutGroup("Refrences")]
        protected TextMeshProUGUI _valueText;
        [SerializeField, SceneObjectsOnly, FoldoutGroup("Refrences")]
        protected string _valueTextIntFormatting = "";

        [SerializeField, SceneObjectsOnly, FoldoutGroup("Refrences")]
        protected ResourceCollection _recipeCost;
        public override void LoadItem(RecipeSlot itemSlot)
        {
            base.LoadItem(itemSlot);

            if (_recipeCost != null)
                _recipeCost.SetCollection(itemSlot.Item.ResourceCost);

            if (_valueText != null)
                _valueText.text = itemSlot.Item.Value.ToString(_valueTextIntFormatting);
        }
    }
}
