using Assets.StateMachine;
using CustomAttributes;
using PowerGamers.Misc;
using Sirenix.OdinInspector;
using Spacetaurant.Containers;
using Spacetaurant.Interactable;
using UnityEngine;
using UnityEngine.Events;
using DataSaving;
using Spacetaurant.Resources;
using Spacetaurant.movement;

namespace Spacetaurant.Player
{
    public class PlayerController : MonoWrap
    {
        [SerializeField, GUIColor("@Color.cyan")]
        private float _interactionRange = default;
        [SerializeField, GUIColor("@Color.yellow")]
        private float _detectionRange = default;


        [SerializeField]
        private float _speed = 10;

        [SerializeField]
        private EventRefrence _onMove_vector2 = default;

        public StateMachine PlayerStateMachine;
        private IMoveController _moveController = default;
        private PlayerState DefaultState => WalkState;
        private PlayerWalkState WalkState => new PlayerWalkState(this);
        
        private PlayerInventory _playerInventory;

        #region Events
        [SerializeField, FoldoutGroup("Events"), SuffixLabel("Interactable")]
        private UnityEventForRefrence OnNewClosestInteractable = default;
        #endregion
        #region State
        private Vector2 _jsDir = default;
        public Vector2 JoystickDirection => _jsDir;

        private bool _interactionButton = default;
        public bool InteractionButton => _interactionButton;

        private bool _interacting = default;
        public bool Interactiong => _interacting;

        private InteractableHit _closestInteractableHit = InteractableHit.Clean;
        public InteractableHit InteractableHit => _closestInteractableHit;
        #endregion
        #region UnityCallbacks
        private void OnEnable() => _onMove_vector2.eventRefrence += (x) => _jsDir = (Vector2)x;
        protected override void OnAwake()
        {
            PlayerStateMachine = new StateMachine(DefaultState);
            _moveController = GetComponent<IMoveController>();
        }
        private void Start()
        {
            UpdateClosestInteractable();
            _playerInventory = DataHandler.GetData<PlayerInventory>();
            //DataHandler.autoSaveInterval = 30;
            //DataHandler.StartAutoSave();
        }
        private void Update()
        {
            var wasd = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

            if (wasd != Vector2.zero)
                _jsDir = wasd;
        }
        private void FixedUpdate()
        {
            PlayerStateMachine.State.FixedUpdate();
            _moveController.ApplyGravity(1);
        }

        private void OnDisable()
        {
            PlayerStateMachine.State = null;

            _onMove_vector2.eventRefrence -= (x) => _jsDir = (Vector2)x;

            StopInteraction();
        }
        #endregion
        #region Internal
        public void UpdateClosestInteractable()
        {
            InteractableHit closestHit = Interactables.GetClosest(transform.position);
            bool sameInteractable = (InteractableHit.interactable != null && closestHit.interactable != null) && InteractableHit.interactable != closestHit.interactable;
            bool moved = (InteractableHit.distance - closestHit.distance) > 0.01f;

            if (!sameInteractable || moved || !_closestInteractableHit.interactable.IsInteractable)
            {
                _closestInteractableHit = closestHit;


                if (closestHit.interactable == null || closestHit.distance > _detectionRange)
                {
                    _closestInteractableHit = InteractableHit.Clean;
                    OnNewClosestInteractable.Invoke(null);
                }
                else if (!sameInteractable)
                    OnNewClosestInteractable?.Invoke(closestHit.interactable);
            }
        }
        public void MoveTowards(Vector3 direction)
        {
            float distance = _speed * Time.deltaTime;
            _moveController.Move(direction, distance);
        }
        private void MoveTo(Vector3 targetPos, float distanceToTarget)
        {
            float distance = Mathf.Clamp(_speed * Time.deltaTime, 0, distanceToTarget);
            _moveController.Move(SphereTools.LocalDirectionToPoint(transform, targetPos), distance);
        }
        public void StartInteraction()
        {
            _interactionButton = true;
        }
        public void UpdateInteraction()
        {
            if (!_interacting)
                _interactionButton = true;
        }
        public void StopInteraction()
        {
            _interactionButton = false;
        }
        public void AddReward(ResourceSlot reward) => _playerInventory.Add(reward);
        #endregion
        #region Debug
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!_debug)
                return;

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _interactionRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _detectionRange);

            if (InteractableHit != null && InteractableHit.interactable != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, InteractableHit.interactable.Position);
            }
        }
#endif
        #endregion
        #region StateMachine
        public abstract class PlayerState : State
        {
            protected readonly PlayerController ctrl;

            protected PlayerState(PlayerController controller)
            {
                this.ctrl = controller;

            }
        }
        public class PlayerWalkState : PlayerState
        {
            public PlayerWalkState(PlayerController controller) : base(controller) { }
            protected override void OnEnable()
            {

            }
            protected override void OnUpdate()
            {

            }
            protected override void OnFixedUpdate()
            {
                ctrl.UpdateClosestInteractable();

                if (ctrl.InteractionButton && ctrl.InteractableHit != null && ctrl.InteractableHit.interactable != null)
                {
                    ctrl.PlayerStateMachine.State = new PlayerInteractState(ctrl);
                    return;
                }

                ctrl.MoveTowards(ctrl.JoystickDirection);


            }
            protected override void OnDisable() { }

        }
        public class PlayerInteractState : PlayerState
        {
            bool _interacting = false;
            public PlayerInteractState(PlayerController controller) : base(controller) { }
            protected override void OnEnable()
            {
                if (ctrl.InteractableHit.interactable != null && ctrl.InteractableHit.interactable.IsInteractable && ctrl.InteractableHit.distance <= ctrl._detectionRange)
                    ctrl.InteractableHit.interactable.OnInteractionFinish.AddListener(FinishedInteraction);
                else
                    SetStateToWalk();
            }
            protected override void OnFixedUpdate()
            {
                if (!ctrl.InteractionButton && ctrl.JoystickDirection != Vector2.zero)
                    SetStateToWalk();

                if (ctrl.InteractableHit.interactable != null && ctrl.InteractableHit.interactable.IsInteractable && ctrl.InteractableHit.distance <= ctrl._detectionRange)
                {
                    ctrl.InteractableHit.UpdateDistance(ctrl.transform.position);

                    if (ctrl.InteractableHit.distance <= ctrl._interactionRange)
                    {
                        if (!_interacting)
                        {
                            ctrl.InteractableHit.interactable.StartInteraction();
                            _interacting = true;
                        }
                    }
                    else
                        ctrl.MoveTo(ctrl.InteractableHit.interactable.Position, ctrl.InteractableHit.distance);
                }
                else
                    SetStateToWalk();
            }
            protected void FinishedInteraction(object interactable)
            {
                if(interactable is Gatherable _gatherable)
                    ctrl.AddReward(_gatherable.Reward);
            }
            void SetStateToWalk() => ctrl.PlayerStateMachine.State = new PlayerWalkState(ctrl);
            protected override void OnDisable()
            {
                if (ctrl.InteractableHit != null && ctrl.InteractableHit.interactable != null && _interacting)
                {
                    ctrl.InteractableHit.interactable.OnInteractionFinish.RemoveListener(FinishedInteraction);
                    ctrl.InteractableHit.interactable.CancelInteraction();
                }
            }
        }
        #endregion
    }

}
