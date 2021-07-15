using Spacetaurant;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoolPassthrough : MonoBehaviour
{
    [SerializeField, EventsGroup]
    private UnityEvent OnTrueEvent;
    [SerializeField, EventsGroup]
    private UnityEvent OnFalseEvent;
    [SerializeField, EventsGroup]
    private UnityEvent<bool> OnTriggerEvent;
    [SerializeField, EventsGroup]
    private UnityEvent<bool> OnTriggerInversedEvent;
    public void Trigger(bool value)
    {
        OnTriggerEvent?.Invoke(value);
        OnTriggerInversedEvent?.Invoke(!value);

        if (value)
            OnTrueEvent?.Invoke();
        else
            OnFalseEvent?.Invoke();
    }
}
