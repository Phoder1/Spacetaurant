using DataSaving;
using Sirenix.OdinInspector;
using Spacetaurant.Containers;
using Spacetaurant.Crafting;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Spacetaurant.Interactable
{
    public enum InteractType { PickupLow, PickupHigh, SawLow, SawHigh, DrillLow, DrillHigh }
    [SelectionBase]

    public class Gatherable : MonoWrap, IInteractable
    {
        #region Constants
        //Decay of progress in progress seconds per gametime seconds.
        const float ProgressDecay = 0.7f;
        #endregion
        #region Serielized
        #region Interactable
        [SerializeField, TabGroup("Interaction settings")]
        private bool _interactable = true;
        [SerializeField, TabGroup("Interaction settings"), SuffixLabel("S")]
        private float _gatherTime = 3;
        public float InteractionTime => _gatherTime;
        [SerializeField, TabGroup("Interaction settings"), PreviewField]
        private Sprite _buttonSprite = null;
        [SerializeField, TabGroup("Interaction settings")]
        private InteractType _interactType;
        public InteractType InteractType => _interactType;
        #endregion
        #region Respawnable
        #endregion

        [SerializeField]
        private ResourceSlot _reward;
        public ResourceSlot Reward => _reward;
        #endregion

        #region Events
        [SerializeField, TabGroup("Events"), SuffixLabel("This")]
        private UnityEventForRefrence _onInteraction = default;
        public UnityEventForRefrence OnInteraction => _onInteraction;

        [SerializeField, TabGroup("Events"), SuffixLabel("This")]
        private UnityEventForRefrence _onInteractionStart = default;
        public UnityEventForRefrence OnInteractionStart => _onInteraction;

        [SerializeField, TabGroup("Events"), SuffixLabel("This")]
        private UnityEventForRefrence _onInteractionFinish = default;
        public UnityEventForRefrence OnInteractionFinish => _onInteraction;

        [SerializeField, TabGroup("Events"), SuffixLabel("This")]
        private UnityEventForRefrence _onInteractionCancel = default;
        public UnityEventForRefrence OnInteractionCancel => _onInteraction;

        [SerializeField, TabGroup("Events"), SuffixLabel("This")]
        private UnityEventForRefrence _onInteractable = default;
        public UnityEventForRefrence OnInteractable => _onInteraction;

        [SerializeField, TabGroup("Events"), SuffixLabel("This")]
        private UnityEventForRefrence _onUninteractable = default;
        public UnityEventForRefrence OnUninteractable => _onInteraction;
        #endregion
        #region State
        private float _gatherProgressTime = 0;
        public float GatherProgressTime
        {
            get => _gatherProgressTime;
            set
            {
                _gatherProgressTime = Mathf.Clamp(value, 0, _gatherTime);

                _progress = Mathf.Clamp01(_gatherProgressTime / _gatherTime);

                if (_progress >= 1)
                    FinishInteraction();
            }
        }
        private float _progress = 0;
        public float Progress => _progress;

        private bool _beingInteracted = false;
        public bool BeingInteracted
        {
            get => _beingInteracted;
            set
            {
                if (value == _beingInteracted)
                    return;

                _beingInteracted = value;

                if (_beingInteracted)
                {
                    OnInteractionStart?.Invoke(this);

                    StopAllCoroutines();
                    StartCoroutine(InteractRoutine());
                }
                else if (GatherProgressTime > 0)
                {
                    StopAllCoroutines();
                    StartCoroutine(ProgressDecayRoutine());
                }

            }
        }

        public Sprite ButtonIcon => _buttonSprite;

        public Vector3 Position => transform.position;

        #endregion
        #region Unity callbacks

        private void OnEnable()
        {
            Interactables.available.Add(this);
        }
        private void OnDisable()
        {
            Interactables.available.Remove(this);
        }
        #endregion
        public bool IsInteractable
        {
            get => _interactable;
            set
            {
                if (_interactable == value)
                    return;

                _interactable = value;

                if (_interactable)
                    OnInteractable?.Invoke(this);
                else
                    OnUninteractable?.Invoke(this);
            }
        }


        public void StartInteraction()
        {

            if (InteractionTime == 0)
                FinishInteraction();
            else
                BeingInteracted = true;

        }
        IEnumerator InteractRoutine()
        {
            while (BeingInteracted)
            {
                yield return null;
                GatherProgressTime += Time.deltaTime;
            }
        }
        IEnumerator ProgressDecayRoutine()
        {
            while (GatherProgressTime > 0)
            {
                yield return null;
                GatherProgressTime -= ProgressDecay * Time.deltaTime;
            }
        }
        public void CancelInteraction()
        {
            OnInteractionCancel?.Invoke(this);
            BeingInteracted = false;
        }

        void FinishInteraction()
        {
            StopAllCoroutines();

            BeingInteracted = false;
            _interactable = false;

            gameObject.SetActive(false);

            GatherProgressTime = 0;

            DataHandler.GetData<PlayerInventory>().Add(Reward);
            
            OnInteractionFinish?.Invoke(this);
        }
    }
}
