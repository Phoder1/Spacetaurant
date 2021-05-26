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

        [SerializeField, FoldoutGroup("Parameters"), Required, ValueDropdown("@AnimatorParameters")]
        [OnValueChanged("@CacheWalkParameter()")]
        private string _walkParameter;
        private void CacheWalkParameter() => CacheParameter(ref _walkID, _walkParameter);
        [SerializeField, FoldoutGroup("Parameters"), ReadOnly]
        private int _walkID;

        [SerializeField, FoldoutGroup("Parameters"), Required, ValueDropdown("@AnimatorParameters")]
        [OnValueChanged("@CachePickupLowParameter()")]
        private string _pickupLowParameter;
        [SerializeField, FoldoutGroup("Parameters"), ReadOnly]
        private int _pickupLowID;
        private void CachePickupLowParameter() => CacheParameter(ref _walkID, _walkParameter);

        [SerializeField, FoldoutGroup("Parameters"), Required, ValueDropdown("@AnimatorParameters")]
        [OnValueChanged("@CachePickupHighParameter()")]
        private string _pickupHighParameter;
        private void CachePickupHighParameter() => CacheParameter(ref _walkID, _walkParameter);
        [SerializeField, FoldoutGroup("Parameters"), ReadOnly]
        private int _pickupHighID;
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
        public void StartGathering(Gatherable gatherable)
        {
            switch (gatherable.GatherType)
            {
                case GatherType.PickupLow:
                    _animator.SetTrigger(_pickupLowID);
                    break;
                case GatherType.PickupHigh:
                    _animator.SetTrigger(_pickupHighID);
                    break;
                case GatherType.SawLow:
                    break;
                case GatherType.SawHigh:
                    break;
                case GatherType.DrillLow:
                    break;
                case GatherType.DrillHigh:
                    break;
            }
        }
        #endregion

    }
}
