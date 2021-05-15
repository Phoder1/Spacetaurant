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

        #region Events
        [SerializeField, FoldoutGroup("Events"), SuffixLabel("Interactable")]
        private UnityEventForRefrence OnNewClosestInteractable = default;
        #endregion
        #region State
        private Vector2 _moveDir = default;
        private bool _interactionButtonPressed = default;
        private bool _interacting = default;
        private InteractableHit _closestInteractableHit = default;

        public InteractableHit ClosestInteractableHit 
        { 
            get => _closestInteractableHit; 
            set => _closestInteractableHit = value; 
        }
        #endregion
        private void OnEnable() => _onMove_vector2.eventRefrence += (x) => _moveDir = (Vector2)x;
        private void OnDisable() => _onMove_vector2.eventRefrence -= (x) => _moveDir = (Vector2)x;
        private void Start()
        {
            UpdateClosestInteractable();
        }
        private void Update()
        {
            var wasd = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            if (wasd != Vector2.zero)
                _moveDir = wasd;
        }
        private void FixedUpdate()
        {
            UpdateClosestInteractable();
            if (ClosestInteractableHit == null 
                || ClosestInteractableHit.interactable == null 
                || ClosestInteractableHit.distance > _detectionRange
                || !_interactionButtonPressed)
            {
                MoveTowards(_moveDir);

                if (_interacting && ClosestInteractableHit.interactable != null)
                {
                    ClosestInteractableHit.interactable.EndInteraction();
                    _interacting = false;
                }
            }
            else
            {
                if (ClosestInteractableHit != null && ClosestInteractableHit.interactable != null)
                {
                    if (ClosestInteractableHit.distance <= _detectionRange)
                        if (ClosestInteractableHit.distance <= _interactionRange)
                        {
                            if (!_interacting)
                                ClosestInteractableHit.interactable.StartInteraction();

                            _interacting = true;
                        }
                        else
                            MoveTo(ClosestInteractableHit.interactable.Position, ClosestInteractableHit.distance);
                }
            }


            Landmark.landmarksPositions.ForEach(DrawLine);

            void DrawLine(Landmark landmark)
            {
                var direction = SphereTools.LocalDirectionToPoint(transform, landmark.transform.position);
                Debug.DrawLine(transform.position, transform.position + transform.TransformDirection(new Vector3(direction.x, 0, direction.y)));
            }
        }
        private void UpdateClosestInteractable()
        {
            InteractableHit closestHit = Interactables.GetClosest(transform.position);

            if (ClosestInteractableHit != closestHit)
            {
                if (closestHit == null || closestHit.interactable == null || closestHit.distance > _detectionRange)
                {
                    ClosestInteractableHit = null;
                    OnNewClosestInteractable.Invoke(null);
                }
                else
                {
                    if (ClosestInteractableHit == null || ClosestInteractableHit != closestHit)
                        OnNewClosestInteractable?.Invoke(closestHit.interactable);

                    ClosestInteractableHit = closestHit;
                }
            }
        }
        private void MoveTowards(Vector3 direction)
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
            _interactionButtonPressed = true;
            _interacting = true;
        }
        public void StopInteraction()
        {
            _interactionButtonPressed = false;
        }
        private void OnDrawGizmos()
        {
            if (!_debug)
                return;

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _interactionRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _detectionRange);

            if (ClosestInteractableHit != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, ClosestInteractableHit.interactable.Position);
            }

        }
    }
}
