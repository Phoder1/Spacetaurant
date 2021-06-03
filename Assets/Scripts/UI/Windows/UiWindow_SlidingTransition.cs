using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spacetaurant.UI
{
    public class UiWindow_SlidingTransition : UiWindow_Tween
    {
        [SerializeField]
        private GameObject _centerPosition = default;
        [SerializeField]
        private GameObject _positionRoot = default;

        private void OnApplicationQuit()
        {
            
        }
        public override void ForceTransitionedIn()
        {
            _positionRoot.transform.localPosition -= transform.localPosition;
        }

        public override void ForceTransitionedOut()
        {
        }

        public override Tween OnTransitionIn(float duration, AnimationCurve curve)
            => _positionRoot.transform.DOMove(_centerPosition.transform.position-transform.localPosition, duration).SetEase(curve);
        public override Tween OnTransitionOut(float duration, AnimationCurve curve)
            => DOVirtual.DelayedCall(duration, null);
    }
}
