using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spacetaurant.Animations
{
    public class ParameterRandomizer : BaseParameterController
    {
        [SerializeField]
        [HideIf("@_type != UnityEngine.AnimatorControllerParameterType.Int")]
        [Tooltip("Minimum value (Inclusive)")]
        private int _rangeMin_int;
        [SerializeField]
        [Tooltip("Maximum value (Inclusive)")]
        [HideIf("@_type != UnityEngine.AnimatorControllerParameterType.Int")]
        private int _rangeMax_int;
        [SerializeField]
        [Tooltip("Minimum value (Inclusive)")]
        [HideIf("@_type != UnityEngine.AnimatorControllerParameterType.Float")]
        private float _rangeMin_float;
        [SerializeField]
        [Tooltip("Maximum value (Inclusive)")]
        [HideIf("@_type != UnityEngine.AnimatorControllerParameterType.Float")]
        private float _rangeMax_float;
        public override void SetParameter(Animator animator)
        {
            switch (_type)
            {
                case AnimatorControllerParameterType.Float:
                    animator.SetFloat(_parameterName, Random.Range(_rangeMin_float, _rangeMax_float), _dampTime, Time.deltaTime);
                    break;
                case AnimatorControllerParameterType.Int:
                    animator.SetInteger(_parameterName, Random.Range(_rangeMin_int, _rangeMax_int + 1));
                    break;
                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(_parameterName, Random.Range(0, 2) == 1);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    if (Random.Range(0, 2) == 1)
                        animator.SetTrigger(_parameterName);
                    break;
                default:
                    break;
            }
        }
    }
}