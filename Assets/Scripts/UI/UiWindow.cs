using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Spacetaurant.UI
{
    public abstract class UiWindow : MonoBehaviour
    {
        [LabelWidth(200)]
        public bool disableWhenNotActive = true;
        #region Events
        [SerializeField, EventsGroup]
        protected UnityEvent OnTransitionIn_Start;
        [SerializeField, EventsGroup]
        protected UnityEvent OnTransitionIn_End;
        [SerializeField, EventsGroup]
        protected UnityEvent OnTransitionOut_Start;
        [SerializeField, EventsGroup]
        protected UnityEvent OnTransitionOut_End;
        [SerializeField, EventsGroup]
        protected UnityEvent OnUiLock;
        [SerializeField, EventsGroup]
        protected UnityEvent OnUiUnlock;
        #endregion

        #region State
        [HideInInspector]
        private WindowGroupHandler groupHandler;
        public WindowGroupHandler GroupHandler 
        { 
            get => groupHandler; 
            set
            {
                if (groupHandler != null)
                    Debug.LogError("UiWindow is refrenced in multiple managers! Previous manager: " + groupHandler.name + ", New manager: " + value.name);
                groupHandler = value;
            } 
        }

        public Tween transitionTween = default;
        private bool _uiLocked;
        public bool Selected => GroupHandler != null && GroupHandler.CurrentState.uiWindow == this;
        public virtual bool UiLocked
        {
            get => _uiLocked;
            set
                => Misc.Setter(ref _uiLocked, value, () =>
                {
                    if (_uiLocked)
                        OnUiLock?.Invoke();
                    else
                        OnUiUnlock?.Invoke();
                });
        }
        #endregion
        public virtual void Init()
        {
            ForceTransitionedOut();
            gameObject.SetActive(!disableWhenNotActive);
        }
        public void CancelTween()
        {
            if (transitionTween != null)
            {
                transitionTween.onComplete?.Invoke();
                transitionTween.Kill();
            }
        }
        public void SelectWindow() => GroupHandler.SelectWindow(this);

        #region Transition In
        [Button]
        public abstract void ForceTransitionedIn();
        public abstract void TransitionIn();
        #endregion

        #region Transition out
        [Button]
        public abstract void ForceTransitionedOut();
        public abstract void TransitionOut();
        #endregion
    }
}
