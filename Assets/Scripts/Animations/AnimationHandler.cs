using CustomAttributes;
using Sirenix.OdinInspector;
using Spacetaurant.Interactable;
using System;
using UnityEngine;

namespace Spacetaurant.Animations
{
    [RequireComponent(typeof(Animator)), DisallowMultipleComponent]
    public class AnimationHandler : MonoBehaviour
    {
        #region Serielized
        [SerializeField, LocalComponent, Required]
        private Animator _animator;

        #region Animator Parameters

        //Walk Parameter
        [SerializeField, AnimatorParameter]
        private string _walkParameter;

        //Pickup Low Parameter
        [SerializeField, AnimatorParameter]
        private string _pickupLowParameter;

        //Pickup High Parameter
        [SerializeField, AnimatorParameter]
        private string _pickupHighParameter;

        [SerializeField, AnimatorParameter]
        private string _movementSpeed;

        [SerializeField, AnimatorParameter]
        private string _goingToPickUp;
        private string[] AnimatorParameters => Array.ConvertAll(_animator.parameters, (x) => x.name);
        #endregion
        #endregion

        #region Interface
        public void StartWalking()
        {
            _animator.SetBool(_walkParameter, true);
        }
        public void StopWalking() => _animator.SetBool(_walkParameter, false);
        public void SetPickupState(bool state) => _animator.SetBool(_goingToPickUp, state);
        public void StartGathering(object interactable)
        {
            if (interactable is IInteractable _interactable)
                StartGathering(_interactable);
        }
        public void SetSpeed(Vector3 speed) => SetSpeed(speed.magnitude);
        public void SetSpeed(float speed) => _animator.SetFloat(_movementSpeed, speed);
        public void StartGathering(IInteractable interactable)
        {
            switch (interactable.InteractType)
            {
                case InteractType.PickupLow:
                    _animator.SetTrigger(_pickupLowParameter);
                    break;
                case InteractType.PickupHigh:
                    _animator.SetTrigger(_pickupHighParameter);
                    break;
                case InteractType.SawLow:
                    break;
                case InteractType.SawHigh:
                    break;
                case InteractType.DrillLow:
                    break;
                case InteractType.DrillHigh:
                    break;
            }
        }
        #endregion
        /// <summary>
        /// A dropdown attribute for animator parameters, requires a function called AnimatorParameters that returns a string array with all the animator parameter names.
        /// </summary>
        [IncludeMyAttributes]
        [FoldoutGroup("Parameters"), Required, ValueDropdown("@AnimatorParameters")]

        public class AnimatorParameterAttribute : Attribute
        {
        }

    }

}
