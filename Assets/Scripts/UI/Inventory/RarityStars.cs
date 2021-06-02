using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spacetaurant.Crafting;
using Sirenix.OdinInspector;

namespace Spacetaurant
{
    public class RarityStars : MonoBehaviour
    {
        [SerializeField, SceneObjectsOnly]
        private Image _starOne;
        [SerializeField, SceneObjectsOnly]
        private Image _starTwo;
        [SerializeField, SceneObjectsOnly]
        private Image _starThree;

        public void SetRarity(ResourceRarity rarity)
        {
            switch (rarity)
            {
                case ResourceRarity.Common:
                    SetStars(true, false, false);
                    break;
                case ResourceRarity.Uncommon:
                    SetStars(true, true, false);
                    break;
                case ResourceRarity.Rare:
                    SetStars(true, true, true);
                    break;
            }
        }
        public void SetStars(bool starOne, bool starTwo, bool starThree)
        {
            _starOne.enabled = starOne;
            _starTwo.enabled = starTwo;
            _starThree.enabled = starThree;
        }
    }
}
