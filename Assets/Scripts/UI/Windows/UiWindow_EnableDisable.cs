using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spacetaurant.UI
{
    public class UiWindow_EnableDisable : UiWindow
    {
        public override void ForceTransitionedIn()
        {
            OnTransitionIn_Start?.Invoke();
            gameObject.SetActive(true);
            OnTransitionIn_End?.Invoke();
        }

        public override void ForceTransitionedOut()
        {
            OnTransitionOut_Start?.Invoke();
            gameObject.SetActive(!disableWhenNotActive);
            OnTransitionOut_End?.Invoke();
        }

        public override void TransitionIn() => ForceTransitionedIn();

        public override void TransitionOut() => ForceTransitionedOut();
    }
}
