using Assets.StateMachine;
using CustomAttributes;
using PowerGamers.Misc;
using Sirenix.OdinInspector;
using Spacetaurant.Interactable;
using UnityEngine;
using UnityEngine.Events;

namespace Spacetaurant.Player
{
    public class PlayerController : MonoWrap
    {
        [SerializeField, GUIColor("@Color.cyan")]
        private float _interactionRange = default;
        [SerializeField, GUIColor("@Color.yellow")]
        private float _detectionRange = default;
        [SerializeField, LocalComponent]
        private MoveControllerSimple _moveController = default;
        [SerializeField]
        private float _speed = 10;
        [SerializeField]
        private EventRefrence _onMove_vector2 = default;

        public StateMachine PlayerStateMachine;
        private PlayerState DefaultState => WalkState;
        private PlayerWalkState WalkState => new PlayerWalkState(this);

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

        private InteractableHit _closestInteractableHit = default;
        public InteractableHit InteractableHit => _closestInteractableHit;
        #endregion
        #region UnityCallbacks
        private void OnEnable() => _onMove_vector2.eventRefrence += (x) => _jsDir = (Vector2)x;
        protected override void OnAwake()
        {
            PlayerStateMachine = new StateMachine(DefaultState);
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
        }
        private void FixedUpdate()
        {
            PlayerStateMachine.State.FixedUpdate();
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

            if (InteractableHit != closestHit)
            {
                if (closestHit == null || closestHit.interactable == null || closestHit.distance > _detectionRange)
                {
                    _closestInteractableHit = null;
                    OnNewClosestInteractable.Invoke(null);
                }
                else
                {
                    if (InteractableHit == null || InteractableHit != closestHit)
                        OnNewClosestInteractable?.Invoke(closestHit.interactable);

                    _closestInteractableHit = closestHit;
                }
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

            if (InteractableHit != null)
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
            public PlayerInteractState(PlayerController controller) : base(controller) { }
            protected override void OnEnable()
            {

            }
            protected override void OnFixedUpdate()
            {
                if (!ctrl.InteractionButton && ctrl.JoystickDirection != Vector2.zero)
                    SetStateToWalk();

                if (ctrl.InteractableHit != null && ctrl.InteractableHit.interactable != null && ctrl.InteractableHit.distance <= ctrl._detectionRange)
                {
                    ctrl.InteractableHit.UpdateDistance(ctrl.transform.position);

                    if (ctrl.InteractableHit.distance <= ctrl._interactionRange)
                        ctrl.InteractableHit.interactable.StartInteraction();
                    else
                        ctrl.MoveTo(ctrl.InteractableHit.interactable.Position, ctrl.InteractableHit.distance);
                }
                else
                    SetStateToWalk();
            }
            void SetStateToWalk() => ctrl.PlayerStateMachine.State = new PlayerWalkState(ctrl);
            protected override void OnDisable()
            {
                if (ctrl.InteractableHit != null && ctrl.InteractableHit.interactable != null)
                    ctrl.InteractableHit.interactable.EndInteraction();
            }
        }
        #endregion
    }

}
