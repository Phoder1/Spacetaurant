using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spacetaurant.UI
{
    public class UiWindow_ScaleTransition : UiWindow
    {
            [SerializeField, FoldoutGroup("Transition in")]
            private float transitionInValue = 1;
            [SerializeField, FoldoutGroup("Transition in")]
            private float transitionOutValue = 0;

        public override void ForceTransitionedIn()
        {
            transform.localScale = Vector3.one * transitionInValue;

        }

        public override void ForceTransitionedOut()
        {
            transform.localScale = Vector3.one * transitionOutValue;
        }
        public override Tween OnTransitionIn(float duration, AnimationCurve curve)
        {
            CancelTween();

            Vector3 scale = transform.localScale;
            return transitionTween = transform.DOScale(transitionInValue, duration).SetEase(curve);
        }
        public override Tween OnTransitionOut(float duration, AnimationCurve curve)
        {
            CancelTween();

            Vector3 scale = transform.localScale;
            return transitionTween = transform.DOScale(transitionOutValue, duration).SetEase(curve);
        }
    }
}
