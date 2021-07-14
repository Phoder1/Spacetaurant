using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Spacetaurant.UI
{
    public abstract class UiWindow : MonoBehaviour
    {
        public bool disableWhenNotActive = true;
        #region Events
        [SerializeField, FoldoutGroup("Events", order: 999)]
        protected UnityEvent OnTransitionIn_Start;
        [SerializeField, FoldoutGroup("Events")]
        protected UnityEvent OnTransitionIn_End;
        [SerializeField, FoldoutGroup("Events")]
        protected UnityEvent OnTransitionOut_Start;
        [SerializeField, FoldoutGroup("Events")]
        protected UnityEvent OnTransitionOut_End;
        [SerializeField, FoldoutGroup("Events")]
        protected UnityEvent OnUiLock;
        [SerializeField, FoldoutGroup("Events")]
        protected UnityEvent OnUiUnlock;
        #endregion

        #region State
        [HideInInspector]
        public WindowGroupHandler groupHandler;

        public Tween transitionTween = default;
        private bool _uiLocked;
        public bool Selected => groupHandler != null && groupHandler.CurrentState.uiWindow == this;
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
        public void SelectWindow() => groupHandler.SelectWindow(this);

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
