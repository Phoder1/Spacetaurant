using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataSaving;
using UnityEngine.Events;
using System;

namespace Spacetaurant
{
    public abstract class DirtyDataListner<T> : MonoBehaviour where T : IDirtyData
    {
        [Tooltip("Triggered whenever a value changes")]
        [SerializeField, EventsGroup]
        private UnityEvent<T> OnValueChangedEvent;

        [Tooltip("Triggered whenever the class gets changed for the 1st time")]
        [SerializeField, EventsGroup]
        private UnityEvent<T> OnDirtyEvent;

        private T _data;
        public T Data => _data;
        protected virtual void Awake()
        {
            _data = Load();
        }
        protected virtual void OnEnable()
        {
            Data.OnValueChange += ValueChanged;
            Data.OnDirty += OnDirty;
        }



        protected virtual void OnDisable()
        {
            Data.OnValueChange -= ValueChanged;
            Data.OnDirty -= OnDirty;
        }
        private void ValueChanged() => OnValueChangedEvent?.Invoke(_data);
        private void OnDirty() => OnDirtyEvent?.Invoke(_data);
        protected abstract T Load();
    }
    public abstract class SaveableDataListner<T> : DirtyDataListner<T> where T : class, ISaveable, new()
    {
        [Tooltip("Triggered whenever a value changes")]
        [SerializeField, EventsGroup]
        private UnityEvent<T> OnSaveStartedEvent;

        [Tooltip("Triggered whenever the class gets changed for the 1st time")]
        [SerializeField, EventsGroup]
        private UnityEvent<T> OnSaveFinishedEvent;
        

        protected override void OnEnable()
        {
            base.OnEnable();
            Data.OnSaveStarted += SaveStarted;
            Data.OnSaveFinished += SaveFinished;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            Data.OnSaveStarted -= SaveStarted;
            Data.OnSaveFinished -= SaveFinished;
        }
        private void SaveStarted() => OnSaveStartedEvent?.Invoke(Data);
        private void SaveFinished() => OnSaveFinishedEvent?.Invoke(Data);

        protected override T Load() => DataHandler.Load<T>();
    }
}
