using CustomAttributes;
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
        [SerializeField, LocalComponent(lockProperty: false)]
        private OnClick _onClick;
        [SerializeField]
        protected InfoLoader<T> _infoLoader;

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
            if (_onClick != null)
                _onClick.OnTrigger.AddListener(() => OnPress?.Invoke(Content));
        }

        private void Start()
        {
            if (_loadOnStart)
                Load();
        }
        public virtual void Load(T content)
        {
            _content = content;
            Load();
        }
        [Button]
        public virtual void Load()
        {
            if (Content == null)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);
            gameObject.name = Content.ToString();

            if (_infoLoader != null)
                _infoLoader.Load(Content);

            OnLoad?.Invoke(Content);
        }
    }
}
