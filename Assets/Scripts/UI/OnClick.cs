using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

namespace Spacetaurant
{
    public enum TriggerTime { Down, Up }
    public class OnClick : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
    {
        [SerializeField, EnumToggleButtons]
        private TriggerTime _triggerTime = TriggerTime.Up;

        [SerializeField]
        private bool _triggerOnlyInside = true;


        [EventsGroup]
        public UnityEvent OnTrigger;
        [EventsGroup]
        public UnityEvent OnHoverEnter;
        [EventsGroup]
        public UnityEvent OnHoverExit;

        private bool _isInside = false;
        private bool _clicked = false;

        public bool IsInside 
        { 
            get => _isInside; 
            set
            {
                if (_isInside == value)
                    return;

                if (!_isInside && value)
                    OnHoverEnter?.Invoke();
                else if (_isInside && !value)
                    OnHoverExit?.Invoke();

                _isInside = value;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            IsInside = true;
            _clicked = true;

            if (_triggerTime == TriggerTime.Down)
                OnTrigger?.Invoke();
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            IsInside = true;
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            IsInside = false;
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            if (_clicked && _triggerTime == TriggerTime.Up && (IsInside || !_triggerOnlyInside))
                OnTrigger?.Invoke();
            _clicked = false;
        }
    }
}
