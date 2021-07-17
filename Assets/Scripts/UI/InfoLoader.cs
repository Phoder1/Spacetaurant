using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Spacetaurant
{
    public abstract class InfoLoader<T> : MonoBehaviour
    {
        [SerializeField, EventsGroup]
        private UnityEvent<T> OnLoad;

        [Button]
        public virtual void Load(object info)
        {
            if (info is T _T)
                Load(_T);
        }
        public virtual void Load(T info)
        {
            if (info == null)
            {

                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);

            OnLoad?.Invoke(info);
        }
    }

    public abstract class EntityLoader<T> : InfoLoader<T> where T : EntitySO
    {
        [SerializeField, SceneObjectsOnly, RefrencesGroup]
        private TextMeshProUGUI _nameText;


        public override void Load(T info)
        {
            base.Load(info);

            if (_nameText != null)
                _nameText.text = info.Name;
        }
    }
    public abstract class ItemLoader<T> : EntityLoader<T> where T : ItemSO
    {
        [SerializeField, SceneObjectsOnly, RefrencesGroup]
        private RarityStars _rarityStars;

        [SerializeField, SceneObjectsOnly, RefrencesGroup]
        private Image _icon;

        [SerializeField, SceneObjectsOnly, RefrencesGroup]
        private Image _planetIcon;

        [SerializeField, SceneObjectsOnly, RefrencesGroup]
        private TextMeshProUGUI _planetName;

        [SerializeField, SceneObjectsOnly, RefrencesGroup]
        private TextMeshProUGUI _descriptionText;
        public override void Load(T info)
        {
            base.Load(info);

            if (_rarityStars != null)
                _rarityStars.SetRarity(info.Rarity);

            if (_icon != null)
            {
                _icon.gameObject.SetActive(true);
                _icon.sprite = info.Icon;
            }

            if (_descriptionText != null)
                _descriptionText.text = info.Description;

            if (_planetIcon != null && info.Planet != null)
                _planetIcon.sprite = info.Planet.Icon;

            if (_planetName != null && info.Planet != null)
                _planetName.text = info.Planet.PlanetName;
        }
    }
    public abstract class SlotLoader<T, TSlot> : InfoLoader<T> where TSlot : Slot<T>
    {
        [SerializeField, SceneObjectsOnly, RefrencesGroup]
        private TextMeshProUGUI _amount;
        public virtual void Load(UiButton<TSlot> uiButton) => Load(uiButton.Content);
        public virtual void Load(TSlot info)
        {
            Load(info.Item);

            if (_amount != null)
                _amount.text = info.Amount.ToString();
        }
    }
}
