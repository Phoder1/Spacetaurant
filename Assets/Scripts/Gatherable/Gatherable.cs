using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Spacetaurant.Interactable
{


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
        #endregion
        #region Respawnable
        #endregion
        #endregion

        #region Events
        [SerializeField, TabGroup("Events"), SuffixLabel("This")]
        private UnityEventForRefrence OnInteraction = default;
        [SerializeField, TabGroup("Events"), SuffixLabel("This")]
        private UnityEventForRefrence OnInteractionStart = default;
        [SerializeField, TabGroup("Events"), SuffixLabel("This")]
        private UnityEventForRefrence OnInteractionFinish = default;
        [SerializeField, TabGroup("Events"), SuffixLabel("This")]
        private UnityEventForRefrence OnInteractionCancel = default;
        [SerializeField, TabGroup("Events"), SuffixLabel("This")]
        private UnityEventForRefrence OnInteractable = default;
        [SerializeField, TabGroup("Events"), SuffixLabel("This")]
        private UnityEventForRefrence OnUninteractable = default;
        #endregion
        #region State
        private float _gatherProgressTime = 0;
        public float GatherProgressTime
        {
            get => _gatherProgressTime;
            set
            {
                _gatherProgressTime = Mathf.Clamp(value, 0, _gatherTime);

                UpdateProgress();
            }
        }
        private float _progress = 0;
        public float Progress => _progress;

        private bool _beingInteracted = false;

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
            OnInteractionStart?.Invoke(this);

            _beingInteracted = true;

            if (InteractionTime == 0)
                FinishInteraction();
        }
        void Update()
        {
            if (_beingInteracted)
                GatherProgressTime += Time.deltaTime;
            else if (GatherProgressTime > 0)
                GatherProgressTime -= ProgressDecay * Time.deltaTime;
        }
        public void CancelInteraction()
        {
            OnInteractionCancel?.Invoke(this);
            _beingInteracted = false;
        }
        private void UpdateProgress()
        {
            _progress = Mathf.Clamp(GatherProgressTime / _gatherTime, 0, 1);

            if (_progress >= 1)
                FinishInteraction();
        }
        void FinishInteraction()
        {
            _beingInteracted = false;
            OnInteractionFinish?.Invoke(this);
            _interactable = false;

            gameObject.SetActive(false);
        }
    }
}
