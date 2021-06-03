using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Spacetaurant.UI
{
    public abstract class UiWindow_Tween : UiWindow
    {
        #region Serielized
        [FoldoutGroup("Transition in")]
        [SerializeField] float transitionInDuration = 0.5f;
        [FoldoutGroup("Transition in")]
        [SerializeField] AnimationCurve transitionInCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [FoldoutGroup("Transition out")]
        [SerializeField] float transitionOutDuration = 0.5f;
        [FoldoutGroup("Transition out")]
        [SerializeField] AnimationCurve transitionOutCurve = AnimationCurve.Linear(0, 0, 1, 1);
        #endregion

        public override void TransitionIn()
        {
            gameObject.SetActive(true);
            UiLocked = true;
            TransitionInStart();

            transitionTween = OnTransitionIn(transitionInDuration, transitionInCurve);

            if (transitionTween != null)
                transitionTween.onComplete += TransitionInEnd;
            else
                TransitionInEnd();
        }

        public override void TransitionOut()
        {
            TransitionOutStart();
            transitionTween = OnTransitionOut(transitionOutDuration, transitionOutCurve);

            if (transitionTween != null)
                transitionTween.onComplete += TransitionOutEnd;
            else
                TransitionOutEnd();
        }
        public abstract Tween OnTransitionIn(float duration, AnimationCurve curve);
        public virtual void TransitionInStart() => OnTransitionIn_Start?.Invoke();
        public virtual void TransitionInEnd()
        {
            UiLocked = false;
            OnTransitionIn_End?.Invoke();
        }

        public virtual void TransitionOutStart() => OnTransitionOut_Start?.Invoke();
        public abstract Tween OnTransitionOut(float duration, AnimationCurve curve);
        public virtual void TransitionOutEnd()
        {
            gameObject.SetActive(!disableWhenNotActive);
            OnTransitionOut_End?.Invoke();
        }
    }
}
