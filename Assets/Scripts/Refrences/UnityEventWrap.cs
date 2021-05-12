using System;
using System.Reflection;

namespace UnityEngine.Events
{
    [Serializable]
    public class UnityEventForRefrence : UnityEventWrap<object> 
    {
        public static UnityEventForRefrence operator +(UnityEventForRefrence a, UnityAction<object> b)
        {
            a.AddListener(b);
            return a;
        }
        public static UnityEventForRefrence operator +(UnityEventForRefrence a, UnityEventForRefrence b)
        {
            a.AddListener((x) => b?.Invoke(x));
            return a;
        }
        public static UnityEventForRefrence operator -(UnityEventForRefrence a, UnityAction<object> b)
        {
            a.RemoveListener(b);
            return a;
        }
        public static UnityEventForRefrence operator -(UnityEventForRefrence a, UnityEventForRefrence b)
        {
            a.RemoveListener((x) => b?.Invoke(x));
            return a;
        }
        public void Trigger(object value) => Invoke(value);
    }
    [Serializable]
    public class UnityEventWrap<T> : UnityEvent<T>
    {
        public static UnityEventWrap<T> operator +(UnityEventWrap<T> a, UnityAction<T> b)
        {
            a.AddListener(b);
            return a;
        }
        public static UnityEventWrap<T> operator +(UnityEventWrap<T> a, UnityEventWrap<T> b)
        {
            a.AddListener((x) => b?.Invoke(x));
            return a;
        }
        public static UnityEventWrap<T> operator -(UnityEventWrap<T> a, UnityAction<T> b)
        {
            a.RemoveListener(b);
            return a;
        }
        public static UnityEventWrap<T> operator -(UnityEventWrap<T> a, UnityEventWrap<T> b)
        {
            a.RemoveListener((x) => b?.Invoke(x));
            return a;
        }
    }
    public class UnityEventWrap : UnityEvent
    {
        public static UnityEventWrap operator +(UnityEventWrap a, UnityAction b)
        {
            a.AddListener(b);
            return a;
        }
        public static UnityEventWrap operator +(UnityEventWrap a, UnityEventWrap b)
        {
            a.AddListener(() => b?.Invoke());
            return a;
        }
        public static UnityEventWrap operator -(UnityEventWrap a, UnityAction b)
        {
            a.RemoveListener(b);
            return a;
        }
        public static UnityEventWrap operator -(UnityEventWrap a, UnityEventWrap b)
        {
            a.RemoveListener(() => b?.Invoke());
            return a;
        }
    }
}
