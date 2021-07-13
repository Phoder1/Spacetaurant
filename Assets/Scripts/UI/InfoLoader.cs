using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Spacetaurant
{
    public abstract class InfoLoader<T> : MonoBehaviour
    {
        [SerializeField, EventsGroup]
        private UnityEvent<T> OnLoad;

        [Button]
        public virtual void Load(T info)
        {
            OnLoad?.Invoke(info);
        }
    }
}
