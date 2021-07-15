using DataSaving;
using Spacetaurant.Containers;
using Spacetaurant.Crafting;
using Spacetaurant.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Spacetaurant
{
    public class RecipeInfo : ItemLoader<RecipeSO>
    {
        [SerializeField, RefrencesGroup]
        protected TextMeshProUGUI _valueText;
        [SerializeField, RefrencesGroup]
        protected string _valueTextIntFormatting = "";

        [SerializeField, RefrencesGroup]
        protected ResourceCollection _recipeCost;

        [SerializeField, RefrencesGroup]
        protected List<SlotLoader<ResourceSO, ResourceSlot>> _costLoader;

        [SerializeField, RefrencesGroup]
        protected BoolPassthrough _canMakeIcon;
        public override void Load(RecipeSO info)
        {
            base.Load(info);

            if (_recipeCost != null)
                _recipeCost.SetCollection(info.ResourceCost);

            if (_valueText != null)
                _valueText.text = info.Value.ToString(_valueTextIntFormatting);

            if (_canMakeIcon != null)
                _canMakeIcon.Trigger(DataHandler.Load<PlayerInventory>().CanMake(info));

            if (_costLoader != null && _costLoader.Count > 0)
            {
                for (int i = 0; i < _costLoader.Count; i++)
                {
                    if (info.ResourceCost.Count <= i)
                    {
                        _costLoader[i].gameObject.SetActive(false);
                        continue;
                    }

                    _costLoader[i].gameObject.SetActive(true);
                    _costLoader[i].Load(info.ResourceCost[i]);
                }
            }
        }
    }
}
