using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Spacetaurant.Interactable
{

    
    public class Gatherable : MonoWrap, IInteractable
    {
        #region Serielized

        #region Interactable
        [SerializeField, TabGroup("Interaction settings")]
        private bool _interactable = true;
        [SerializeField, TabGroup("Interaction settings"), SuffixLabel("S")]
        private float _interactionTime = 3;
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
        private UnityEventForRefrence OnInteractionEnd = default;
        [SerializeField, TabGroup("Events"), SuffixLabel("This")]
        private UnityEventForRefrence OnInteractionCancel = default;
        [SerializeField, TabGroup("Events"), SuffixLabel("This")]
        private UnityEventForRefrence OnInteractable = default;
        [SerializeField, TabGroup("Events"), SuffixLabel("This")]
        private UnityEventForRefrence OnUninteractable = default;
        #endregion
        #region State
        private float _interactionStartTime = Mathf.Infinity;
        #endregion
        #region Unity callbacks

        private void OnEnable()
        {
            if (Interactable)
                Interactables.available.Add(this);
        }
        private void OnDisable()
        {
            if (Interactable)
                Interactables.available.Remove(this);
        }
        #endregion
        public bool Interactable
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
        public float InteractionTime => _interactionTime;

        public Sprite ButtonIcon => _buttonSprite;

        public Vector3 Position => transform.position;
        public void StartInteraction()
        {
            _interactionStartTime = Time.time;
            OnInteractionStart?.Invoke(this);
        }
        public void EndInteraction()
        {
            if(Time.time - _int)
            OnInteractionEnd?.Invoke(this);
        }
        void Update()
        {

        }
    }
}
