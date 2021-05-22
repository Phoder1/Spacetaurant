using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spacetaurant.Animations
{
    public abstract class BaseParameterController : StateMachineBehaviour
    {
        [SerializeField]
        protected string _parameterName = default;
        [SerializeField]
        protected AnimatorControllerParameterType _type = default;
        [SerializeField]
        protected bool _onExit = default;
        [SerializeField]
        protected bool _onEnter = default;

        [SerializeField]
        [HideIf("@_type != UnityEngine.AnimatorControllerParameterType.Float")]
        protected float _dampTime = default;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_onEnter)
                SetParameter(animator);
        }

        //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_onExit)
                SetParameter(animator);
        }

        public abstract void SetParameter(Animator animator);
    }
}