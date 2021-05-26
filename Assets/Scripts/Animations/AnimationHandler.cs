using CustomAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using Spacetaurant.Interactable;

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
        [SerializeField, FoldoutGroup("Parameters"), Required, ValueDropdown("@AnimatorParameters")]
        [OnValueChanged("@CacheWalkParameter()")]
        private string _walkParameter;
        [SerializeField, FoldoutGroup("Parameters"), ReadOnly]
        private int _walkID;
        private void CacheWalkParameter() => CacheParameter(ref _walkID, _walkParameter);

        //Pickup Low Parameter
        [SerializeField, FoldoutGroup("Parameters"), Required, ValueDropdown("@AnimatorParameters")]
        [OnValueChanged("@CachePickupLowParameter()")]
        private string _pickupLowParameter;
        [SerializeField, FoldoutGroup("Parameters"), ReadOnly]
        private int _pickupLowID;
        private void CachePickupLowParameter() => CacheParameter(ref _pickupLowID, _pickupLowParameter);

        //Pickup High Parameter
        [SerializeField, FoldoutGroup("Parameters"), Required, ValueDropdown("@AnimatorParameters")]
        [OnValueChanged("@CachePickupHighParameter()")]
        private string _pickupHighParameter;
        [SerializeField, FoldoutGroup("Parameters"), ReadOnly]
        private int _pickupHighID;
        private void CachePickupHighParameter() => CacheParameter(ref _pickupHighID, _pickupHighParameter);
        private string[] AnimatorParameters => Array.ConvertAll(_animator.parameters, (x) => x.name);
        void CacheParameter(ref int ID, string name) => ID = Array.Find(_animator.parameters, (x) => x.name == name).nameHash;
        #endregion
        #endregion

        #region Interface
        public void StartWalking()
        {
            _animator.SetBool(_walkID, true);
        }
        public void StopWalking()
        {
            _animator.SetBool(_walkID, false);
        }
        public void StartGathering(object interactable)
        {
            if (interactable is IInteractable _interactable)
                StartGathering(_interactable);
        }
        public void StartGathering(IInteractable interactable)
        {
            switch (interactable.InteractType)
            {
                case InteractType.PickupLow:
                    _animator.SetTrigger(_pickupLowID);
                    break;
                case InteractType.PickupHigh:
                    _animator.SetTrigger(_pickupHighID);
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

    }
}
