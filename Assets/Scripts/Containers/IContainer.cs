using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataSaving;
using System;
using UnityEngine.Events;

namespace Spacetaurant
{
    public interface IContainer<T> where T : IDirtyData
    {
        DirtyDataList<T> Container { get; } 
        void Sort(Comparison<T> comparison);
        void Add(T item);
        bool TryGet(out T item);
        UnityEventForRefrence OnValueChange { get; }
    }
}
