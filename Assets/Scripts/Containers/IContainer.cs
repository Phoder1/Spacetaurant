using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataSaving;
using System;
using UnityEngine.Events;

namespace Spacetaurant.Containers
{
    public interface IContainer<T> where T : IDirtyData
    {
        DirtyDataList<T> Container { get; } 
        void Sort(Comparison<T> comparison);
        void Add(T item);
        bool TryGet(T item, out T outputItem);
    }
}
