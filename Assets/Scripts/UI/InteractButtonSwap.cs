using CustomAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Spacetaurant.Interactable;

namespace Spacetaurant.UI
{
    public class InteractButtonSwap : MonoBehaviour
    {
        [SerializeField,  ShowIf("@!UnityEngine.Application.isPlaying")]
        private EventRefrence _eventRefrence = default;
        [SerializeField, LocalComponent]
        private Image _image = default;

        private Sprite _defaultSprite;
        private void Awake()
        {
            _defaultSprite = _image.sprite;
        }
        private void OnEnable() => Subscribe();
        private void Subscribe()
        {
            if (_eventRefrence != null)
                _eventRefrence.eventRefrence += SwapButtonImage;
        }
        private void OnDisable() => Unsubscribe();

        private void Unsubscribe()
        {
            if (_eventRefrence != null)
                _eventRefrence.eventRefrence -= SwapButtonImage;
        }

        private void SwapButtonImage(object interactable)
        {
            if (interactable == null)
                _image.sprite = _defaultSprite;
            else if (interactable is IInteractable _interactable)
                _image.sprite = _interactable.ButtonIcon;
        }
    }
}
