using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Spacetaurant.UI
{
    public abstract class UiWindow : MonoBehaviour
    {
        [FoldoutGroup("Transition in")]
        [SerializeField] float transitionInDuration = 0.5f;
        [FoldoutGroup("Transition in")]
        [SerializeField] AnimationCurve transitionInCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [FoldoutGroup("Transition out")]
        [SerializeField] float transitionOutDuration = 0.5f;
        [FoldoutGroup("Transition out")]
        [SerializeField] AnimationCurve transitionOutCurve = AnimationCurve.Linear(0, 0, 1, 1);

        #region Events
        [SerializeField, FoldoutGroup("Events",order: 999)]
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

        [HideInInspector]
        public WindowGroupHandler groupHandler;

        public Tween transitionTween = default;
        #endregion
        private bool _uiLocked;
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
        void Awake()
        {
            ForceTransitionedOut();
            Init();
        }
        public virtual void Init() { }
        public void CancelTween()
        {
            if (transitionTween != null)
            {
                transitionTween.onComplete?.Invoke();
                transitionTween.Kill();
            }
        }
        public void SelectWindow() => groupHandler.SelectWindow(this);
        public virtual void OnUpdate() { }
        public virtual void OnReset() { }
        public virtual void TransitionInEnd()
        {
            UiLocked = false;
            OnTransitionIn_End?.Invoke();
        }
        public virtual void TransitionOutEnd()
        {
            gameObject.SetActive(false);
            OnTransitionOut_End?.Invoke();
        }
        public abstract void ForceTransitionedOut();
        public abstract void ForceTransitionedIn();
        public virtual void TransitionInStart() => OnTransitionIn_Start?.Invoke();
        public virtual void TransitionOutStart() => OnTransitionOut_Start?.Invoke();
        public virtual void UnlockUI() { }
        public Tween TransitionIn()
        {
            gameObject.SetActive(true);
            UiLocked = true;
            TransitionInStart();
            transitionTween = OnTransitionIn(transitionInDuration, transitionInCurve);
            transitionTween.onComplete += TransitionInEnd;
            return transitionTween;
        }
        public abstract Tween OnTransitionIn(float duration, AnimationCurve curve);
        public Tween TransitionOut()
        {
            TransitionOutStart();
            transitionTween = OnTransitionOut(transitionOutDuration, transitionOutCurve);
            transitionTween.onComplete += TransitionOutEnd;
            return transitionTween;
        }
        public abstract Tween OnTransitionOut(float duration, AnimationCurve curve);
    }
}
