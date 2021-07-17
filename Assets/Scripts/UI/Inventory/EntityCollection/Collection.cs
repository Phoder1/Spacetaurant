using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Spacetaurant
{
    public abstract class Collection<T> : MonoBehaviour
    {
        [SerializeField]
        private bool _loadCollectionOnStart;

        [SerializeField]
        protected List<T> _collection = new List<T>();

        protected abstract Sorter<T>[] Sorters { get; }
        protected virtual void Start()
        {
            if (_loadCollectionOnStart)
                ReloadCollection();
        }
        [Button]
        public virtual void ReloadCollection()
        {
            if (_collection == null || _collection.Count == 0)
                return;

            Sort();
            Filter();
        }
        public virtual void SetCollection(object collection)
        {
            if (collection is IList<T> _slots)
            {
                SetCollection(_slots);
                return;
            }

            if (collection is T _T)
            {
                SetCollection(new List<T>() { _T });
                return;
            }
        }
        public virtual void SetCollection(IList<T> collection)
        {
            _collection = new List<T>(collection);

            ReloadCollection();
        }

        private void Filter()
        {
            for (int i = 0; i < _collection.Count; i++)
                if (_collection[i] == null || !CheckFilters(_collection[i]))
                    Disable(_collection[i]);
                else
                    Enable(_collection[i]);
        }

        protected virtual bool CheckFilters(T slot) => true;
        protected virtual void Sort()
        {
            if (Sorters == null || Sorters.Length == 0)
                return;

            _collection.Sort(ListHelper.CombineComparers(Array.ConvertAll(Sorters, (x) => x.Comparer)));
        }
        protected abstract void Enable(T item);
        protected abstract void Disable(T item);
    }
    public abstract class ComponentCollection<T> : Collection<T> where T : Component
    {
        public void InstantiateCollection(List<T> collection)
        {
            foreach (var item in collection)
            {
                var newItem = Instantiate(item, transform);
                _collection.Add(newItem);
                OnInstantiation(newItem);
            }

            ReloadCollection();
        }
        protected virtual void OnInstantiation(T item) { }
        [Button]
        protected override void Sort()
        {
            base.Sort();

            for (int i = 0; i < _collection.Count; i++)
                _collection[i].transform.SetSiblingIndex(i);
        }
        protected override void Enable(T item) => item.gameObject.SetActive(true);
        protected override void Disable(T item) => item.gameObject.SetActive(false);
    }
    public abstract class ButtonCollection<T, TData> : ComponentCollection<T> where T : UiButton<TData>
    {
        [SerializeField, EventsGroup]
        private UnityEvent<T> OnClick;
        [SerializeField]
        private InfoLoader<TData> _infoPanel;
        public InfoLoader<TData> InfoPanel => _infoPanel;
        private void Awake()
        {
            if (InfoPanel != null)
                _collection.ForEach((x) => { if (x != null) x.OnPress.AddListener((y) => ButtonPressed(x, y)); });

            void ButtonPressed(T button, TData info)
            {
                if (InfoPanel != null)
                    InfoPanel.Load(info);

                OnClick?.Invoke(button);
            }
        }
        public override void SetCollection(object collection)
        {
            base.SetCollection(collection);

            if (collection is IList<TData> dataCollection)
            {
                SetCollection(dataCollection);
                return;
            }
        }
        public virtual void SetCollection(IList<TData> newCollection)
        {
            for (int i = 0; i < _collection.Count; i++)
                if (newCollection.Count <= i)
                    Disable(_collection[i]);
                else
                {
                    Enable(_collection[i]);
                    _collection[i].Load(newCollection[i]);
                }
        }
        protected override void OnInstantiation(T item)
        {
            base.OnInstantiation(item);
            item.OnPress.AddListener(InfoPanel.Load);
        }
        public override void ReloadCollection()
        {
            base.ReloadCollection();

            _collection.ForEach((x) => x.Load());
        }
    }
    [Serializable, InlineProperty, HideLabel]
    public abstract class Sorter<T>
    {
        [HorizontalGroup(LabelWidth = 50), GUIColor("ToggleColor")]
        public bool reverse = false;
        public Comparison<T> Comparer
        {
            get
            {
                var comparer = AddValidCheck(GetComparer());

                if (reverse)
                    comparer = ReverseComparer(comparer);

                return comparer;
            }
        }
#if UNITY_EDITOR
        private Color ToggleColor => reverse ? new Color(1f, 0.8f, 0.8f) : new Color(0.8f, 1f, 0.8f);
#endif
        protected abstract Comparison<T> GetComparer();

        private Comparison<T> AddValidCheck(Comparison<T> comparer)
        {
            return Compare;
            int Compare(T a, T b)
            {
                if (Invalid(a))
                {
                    if (Invalid(b))
                        return 0;

                    return -1;
                }
                else if (Invalid(b))
                    return 1;

                return comparer(a, b);
            }
        }
        protected virtual bool Invalid(T item) => item == null;
        private Comparison<T> ReverseComparer(Comparison<T> comparer)
        {
            return Reverse;
            int Reverse(T a, T b) => -1 * comparer(a, b);
        }
        public static Comparison<T> operator +(Sorter<T> a, Sorter<T> b) => ListHelper.CombineComparers(a.Comparer, b.Comparer);
        public static Comparison<T> operator +(Comparison<T> a, Sorter<T> b) => ListHelper.CombineComparers(a, b.Comparer);


    }
    [IncludeMyAttributes]
    [HideLabel, EnumToggleButtons, HorizontalGroup(MaxWidth = 1)]
    public class SorterEnum : Attribute { }
}
