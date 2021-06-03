using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

namespace UnityEngine.Events
{
    public class EventsListner : MonoBehaviour
    {
        [SerializeField, ListDrawerSettings(Expanded = true, ShowPaging = true, ListElementLabelName = "@ElementName", ShowItemCount = true)]
        private List<Listner> _listners;
        private void OnEnable() => _listners.ForEach((x) => x.Enabled = true);
        private void OnDisable () => _listners.ForEach((x) => x.Enabled = false);

        [Serializable]
        public class Listner
        {
            [SerializeField]
            private EventRefrence _refrence;
            public EventRefrence Refrence
            {
                get => _refrence;
                set
                {
                    if (_refrence == value)
                        return;

                    if (_refrence != null)
                        _refrence.eventRefrence -= OnTrigger;

                    _refrence = value;

                    if (_refrence != null)
                        _refrence.eventRefrence += OnTrigger;
                    _refrence.eventRefrence += OnTrigger;

                }
            }
            public Object DefaultData = default;
            [SerializeField, TabGroup("Default", true, AnimateVisibility = false)]
            private UnityEventForRefrence OnTrigger;
            [SerializeField, TabGroup("Early", true, AnimateVisibility = false)]
            private UnityEventForRefrence OnTrigger_Early;
            [SerializeField, TabGroup("Late", true, AnimateVisibility = false)]
            private UnityEventForRefrence OnTrigger_Late;

#if UNITY_EDITOR
            public string ElementName
            {
                get
                {
                    if (Refrence != null)
                        return Refrence.name;
                    else
                        return "Listner";
               }

            }
#endif
            private bool _enabled;
            public bool Enabled
            {
                get => _enabled;
                set
                {
                    if (_enabled == value)
                        return;

                    _enabled = value;

                    if (_enabled)
                        Enable();
                    else
                        Disable();
                }
            }
            private void Enable()
            {
                if (Refrence != null && Refrence.eventRefrence != null)
                    Refrence.eventRefrence.AddListener(Trigger);
            }
            private void Disable()
            {
                if (Refrence != null && Refrence.eventRefrence != null)
                    Refrence.eventRefrence.RemoveListener(Trigger);
            }
            [Button]
            public void TriggerDefault() => Trigger(DefaultData);
            public void Trigger(object data)
            {
                if (Enabled)
                {
                    OnTrigger_Early?.Invoke(data);
                    OnTrigger?.Invoke(data);
                    OnTrigger_Late?.Invoke(data);
                }
            }
            public void SetDefaultData(object data) => DefaultData = (Object)data;
        }
        


    }
}
