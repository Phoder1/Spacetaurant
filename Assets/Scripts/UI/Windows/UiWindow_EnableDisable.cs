using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spacetaurant.UI
{
    public class UiWindow_EnableDisable : UiWindow
    {
        public override void ForceTransitionedIn()
        {
            gameObject.SetActive(true);
        }

        public override void ForceTransitionedOut()
        {
            gameObject.SetActive(!disableWhenNotActive);
        }

        public override void TransitionIn()
        {
            gameObject.SetActive(true);
        }

        public override void TransitionOut()
        {
            gameObject.SetActive(!disableWhenNotActive);
        }
    }
}
