using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Spacetaurant
{
    public abstract class UiButton<T> : MonoBehaviour
    {
        [SerializeField]
        protected T _content;
        [SerializeField]
        private bool _loadOnStart = false;
        [SerializeField]
        private OnClick _onClick;
        [SerializeField]
        private InfoLoader<T> _infoLoader;

        #region Refrences

        #endregion

        #region Events
        [SerializeField, EventsGroup]
        protected UnityEvent<T> OnLoad;
        [EventsGroup]
        public UnityEvent<T> OnPress;
        #endregion

        public T Content => _content;
        private void Awake()
        {
            if (_onClick == null)
                _onClick = GetComponent<OnClick>();

            _onClick.OnTrigger.AddListener(() => OnPress?.Invoke(Content));
        }

        private void Start()
        {
            if (_loadOnStart)
                Load();
        }

        [Button]
        public virtual void Load()
        {
            if (Content == null)
                return;

            gameObject.name = Content.ToString();

            if (_infoLoader != null)
                _infoLoader.Load(Content);

            OnLoad?.Invoke(Content);
        }
    }
}
