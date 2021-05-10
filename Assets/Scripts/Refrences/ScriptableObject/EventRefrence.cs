using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace UnityEngine.Events
{
    [CreateAssetMenu(menuName = "Refrences/Event")]
    public class EventRefrence : ScriptableObject
    {
        [SerializeField, Tooltip("A child event will call this event on trigger."), OnCollectionChanged(Before = "UnsubscribeFromChilds", After = "SubscribeToChilds")]
        private List<EventRefrence> _childEvents;
        [ReadOnly, Tooltip("Actions on trigger.")]
        public UnityEventWrap<object> eventRefrence = new UnityEventWrap<object>();
        public void TriggerEvent() => TriggerEvent(null);
        public void TriggerEvent(object obj) => eventRefrence?.Invoke(obj);
        private void OnDisable() => eventRefrence.RemoveAllListeners();
#if UNITY_EDITOR
        private void UnsubscribeFromChilds() => _childEvents.ForEach((x) => UnityEditor.Events.UnityEventTools.RemovePersistentListener(x.eventRefrence, TriggerEvent));
        private void SubscribeToChilds() => _childEvents.ForEach((x) => UnityEditor.Events.UnityEventTools.AddPersistentListener(x.eventRefrence, TriggerEvent));
#endif
    }
}
