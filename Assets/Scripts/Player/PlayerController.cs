using Assets.StateMachine;
using CMF;
using CustomAttributes;
using PowerGamers.Misc;
using Sirenix.OdinInspector;
using Spacetaurant.Interactable;
using Spacetaurant.movement;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Spacetaurant.Player
{
    public class PlayerController : MonoWrap, ICharacterInput
    {
        #region Serielized
        [SerializeField, GUIColor("@Color.cyan")]
        private float _interactionRange = default;
        [SerializeField, GUIColor("@Color.yellow")]
        private float _detectionRange = default;
        [SerializeField]
        private float _maxAcceleration = Mathf.Infinity;
        [SerializeField]
        private float _maxRotationSpeed = Mathf.Infinity;
        [SerializeField]
        private GameObject _vfx;
        [SerializeField, LocalComponent]
        private AdvancedWalkerController _ctrl;
        
        #endregion

        #region Events
        [SerializeField, FoldoutGroup("Events"), SuffixLabel("Interactable")]
        private UnityEventForRefrence OnNewClosestInteractable = default;
        [FoldoutGroup("Events"), SuffixLabel("Vector2")]
        public UnityEventForRefrence OnStartMove = default;
        [FoldoutGroup("Events"), SuffixLabel("Vector2")]
        public UnityEventForRefrence OnMove = default;
        [FoldoutGroup("Events"), SuffixLabel("Vector2")]
        public UnityEventForRefrence OnEndMove = default;

        [FoldoutGroup("Events"), SuffixLabel("Interactable")]
        public UnityEventForRefrence OnInteractionStart = default;
        [FoldoutGroup("Events"), SuffixLabel("Interactable")]
        public UnityEventForRefrence OnInteractionEnd = default;
        #endregion

        #region State
        #region Joystick

        private Vector2 _jsDir = default;
        public Vector2 JoystickDirection => _jsDir;
        public void SetJoystickDirection(Vector2 direction) => _jsDir = direction;
        public void SetJoystickDirection(object direction) => SetJoystickDirection((Vector2)direction);
        #endregion
        public StateMachine<PlayerState> PlayerStateMachine;
        private IMoveController _moveController = default;
        private PlayerState DefaultState => WalkState;
        private PlayerWalkState WalkState => new PlayerWalkState(this);

        [HideInInspector]
        private Vector2 moveVector = Vector2.zero;
        public Vector2 MoveVector 
        { 
            get => moveVector; 
            set
            {
                var accelerationVector = value - lastMoveDirection;
                accelerationVector = Vector3.ClampMagnitude(accelerationVector, _maxAcceleration * Time.deltaTime);

                moveVector = lastMoveDirection + accelerationVector;
            }
        }
        [HideInInspector]
        public Vector2 lastMoveDirection = Vector2.zero;

        private bool _interactionButton = default;
        public bool InteractionButton => _interactionButton;

        private InteractableHit _closestInteractableHit = InteractableHit.Clean;
        public InteractableHit InteractableHit => _closestInteractableHit;


        private Vector3 _lastPos;
        #endregion

        #region UnityCallbacks
        protected override void OnAwake()
        {
            PlayerStateMachine = new StateMachine<PlayerState>(DefaultState);
            _moveController = GetComponent<IMoveController>();
            _lastPos = transform.position;
        }
        private void Start()
        {
            UpdateClosestInteractable();
        }
        private void Update()
        {
            var wasd = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

            if (wasd != Vector2.zero)
                _jsDir = wasd;

            moveVector = Vector2.zero;

            PlayerStateMachine.State.FixedUpdate();

            if (lastMoveDirection == Vector2.zero && MoveVector != Vector2.zero)
                OnStartMove?.Invoke(MoveVector);
            else if (lastMoveDirection != Vector2.zero && MoveVector == Vector2.zero)
                OnEndMove?.Invoke(MoveVector);
            else if (MoveVector != Vector2.zero)
                OnMove?.Invoke(MoveVector);

            lastMoveDirection = MoveVector;

            ApplyRotation();

            _lastPos = transform.position; 
        }

        private void ApplyRotation()
        {
            var direction = _ctrl.GetVelocity().normalized;
            if (direction == Vector3.zero)
                return;

            var planetUp = (transform.position - BlackBoard.planet.transform.position).normalized;

            var targetRotation = Quaternion.LookRotation(direction, planetUp);
            _vfx.transform.rotation = Quaternion.RotateTowards(_vfx.transform.rotation, targetRotation, _maxRotationSpeed * Time.deltaTime);
        }

        private void OnDisable()
        {
            PlayerStateMachine.State = null;

            StopInteraction();
        }
        #endregion



        #region ICharacterInput interface
        private void MoveTo(Vector3 targetPos) => MoveTowards(SphereTools.LocalDirectionToPoint(transform.position, targetPos, BlackBoard.ingameCamera.transform));
        private void MoveTowards(Vector2 direction) => MoveVector = direction;
        public float GetHorizontalMovementInput() => MoveVector.x;
        public float GetVerticalMovementInput() => MoveVector.y;
        public bool IsJumpKeyPressed() => false;
        #endregion

        #region Interaction
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
        public void StartInteraction() => _interactionButton = true;
        public void StopInteraction() => _interactionButton = false;
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
                if (!(ctrl.InteractableHit.interactable != null && ctrl.InteractableHit.interactable.IsInteractable && ctrl.InteractableHit.distance <= ctrl._detectionRange))
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
                            IInteractable interactable = ctrl.InteractableHit.interactable;
                            interactable.StartInteraction();
                            ctrl.OnInteractionStart?.Invoke(interactable);
                            _interacting = true;
                        }
                    }
                    else
                        ctrl.MoveTo(ctrl.InteractableHit.interactable.Position);
                }
                else
                    SetStateToWalk();
            }
            void SetStateToWalk() => ctrl.PlayerStateMachine.State = new PlayerWalkState(ctrl);
            protected override void OnDisable()
            {
                if (ctrl.InteractableHit != null && ctrl.InteractableHit.interactable != null && _interacting)
                {
                    ctrl.InteractableHit.interactable.CancelInteraction();

                    ctrl.OnInteractionEnd?.Invoke(ctrl.InteractableHit.interactable);
                }
            }
        }
        #endregion
    }

}
