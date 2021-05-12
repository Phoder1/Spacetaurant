using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

namespace Spacetaurant
{
    public class ButtonHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        #region Events
        [SerializeField, Tooltip("Button events can pass data inside the events, and you can use SetData to change the data at runtime.")]
        private Object _eventData;
        [SerializeField, FoldoutGroup("Events"), SuffixLabel("Data")]
        private UnityEventForRefrence OnButtonDown = default;
        [SerializeField, FoldoutGroup("Events"), SuffixLabel("Data")]
        private UnityEventForRefrence WhileButtonPressed = default;
        [SerializeField, FoldoutGroup("Events"), SuffixLabel("Data")]
        private UnityEventForRefrence OnButtonUp = default;
        [SerializeField, FoldoutGroup("Events"), SuffixLabel("Data")]
        private UnityEventForRefrence OnButtonCanceled = default;
        #endregion
        #region State
        private bool _pressed = false;
        #endregion
        public void OnPointerDown(PointerEventData eventData)
        {
            _pressed = true;
            OnButtonDown?.Invoke(_eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _pressed = false;
            OnButtonCanceled?.Invoke(_eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _pressed = false;
            OnButtonUp?.Invoke(_eventData);
        }
        private void Update()
        {
            if (_pressed)
                WhileButtonPressed?.Invoke(_eventData);
        }
        public void SetEventData(Object eventData) => _eventData = eventData;
    }
}
