using Sirenix.OdinInspector;
using Spacetaurant.Crafting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Spacetaurant
{
    public class RarityStars : MonoBehaviour
    {
        private enum RarityFormat { StarCount, Number }
        [SerializeField, EnumToggleButtons]
        private RarityFormat _rarityFormat = RarityFormat.StarCount;
        [SerializeField, SceneObjectsOnly, ShowIf("@_rarityFormat == RarityFormat.Number")]
        private TextMeshProUGUI _rarityText;
        [SerializeField, SceneObjectsOnly, ShowIf("@_rarityFormat == RarityFormat.StarCount")]
        private Image _starOne;
        [SerializeField, SceneObjectsOnly, ShowIf("@_rarityFormat == RarityFormat.StarCount")]
        private Image _starTwo;
        [SerializeField, SceneObjectsOnly, ShowIf("@_rarityFormat == RarityFormat.StarCount")]
        private Image _starThree;
        public void SetRarity(ResourceRarity rarity)
        {
            switch (_rarityFormat)
            {
                case RarityFormat.StarCount:
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
                    break;
                case RarityFormat.Number:
                    _rarityText.text = ((int)rarity + 1).ToString();
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
