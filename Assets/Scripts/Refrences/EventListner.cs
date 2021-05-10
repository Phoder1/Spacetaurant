using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEngine.Events
{
    public class EventListner : MonoBehaviour
    {
        [SerializeField]
        private EventRefrence _eventRefrence;
        public EventRefrence EventRefrence
        {
            get => _eventRefrence;
            set
            {
                if (_eventRefrence == value)
                    return;

                if (_eventRefrence != null)
                    _eventRefrence.eventRefrence -= _passThroughEvent;

                _eventRefrence = value;

                if (_eventRefrence != null)
                    _eventRefrence.eventRefrence += _passThroughEvent;
                _eventRefrence.eventRefrence += _passThroughEvent;

            }
        }
        public Object DefaultData = default;
        [SerializeField, TabGroup("Default", true, AnimateVisibility = false)]
        private UnityEventWrap<object> _passThroughEvent;
        [SerializeField, TabGroup("Early", true, AnimateVisibility = false)]
        private UnityEventWrap<object> _earlyPassthroughEvent;
        [SerializeField, TabGroup("Late", true, AnimateVisibility = false)]
        private UnityEventWrap<object> _latePassthroughEvent;

        private void OnEnable()
        {
            if (EventRefrence != null && EventRefrence.eventRefrence != null)
                EventRefrence.eventRefrence.AddListener(Trigger);
        }

        private void OnDisable()
        {
            if (EventRefrence != null && EventRefrence.eventRefrence != null)
                EventRefrence.eventRefrence.RemoveListener(Trigger);
        }
        [Button]
        public void TriggerDefault() => Trigger(DefaultData);
        public void Trigger(object data)
        {
            if (isActiveAndEnabled)
            {
                _earlyPassthroughEvent?.Invoke(data);
                _passThroughEvent?.Invoke(data);
                _latePassthroughEvent?.Invoke(data);
            }
        }
        public void SetDefaultData(object data) => DefaultData = (Object)data;

    }
}
