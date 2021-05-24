using CustomAttributes;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using static Spacetaurant.BlackBoard;

namespace Spacetaurant.movement
{
    [RequireComponent(typeof(CapsuleCollider))]
    public class MoveController : MonoWrap, IMoveController
    {
        [SerializeField, LocalComponent]
        private CapsuleCollider _collider = default;
        [SerializeField]
        float maxSlopeAngle = 60;
        //[SerializeField]
        //float minSlopeAngle = -30;
        [SerializeField, FoldoutGroup("Offsets"), Tooltip("The offset on the Y from which the ground ray will be shot"), SuffixLabel("Uu")]
        private float groundRayOffset = default;
        [SerializeField, FoldoutGroup("Offsets"), Tooltip("The offset on the Y from which the forward ray will be shot"), SuffixLabel("Uu")]
        private float forwardRayOffset = default;
        [SerializeField, SuffixLabel("Uu")]
        private float skinWidth = default;

        private float _colliderRadius = default;

        public UnityEventForRefrence OnMove = default;

        const int MAX_MOVE_STEPS = 5;

        #region Frame temp
        float planetRadius = default;
        float rotationRadians = default;
        Vector3 localDirection = default;
        float moveChord = default;
        float horizontal = default;
        float vertical = default;
        #endregion
        private void Start()
        {
            _colliderRadius = _collider.radius;
        }
        public void Move(Vector2 direction, float distance)
        {
            UpdateFrameConstants(distance);

            if (direction != Vector2.zero)
                MoveHorizontally(direction, distance);

            MoveVertically(vertical);

            transform.rotation = Quaternion.FromToRotation(transform.up, PlanetUp) * transform.rotation;
            
            MoveVertically(0.1f);
        }
        public void ApplyGravity(float gravityForce)
        {
        }
        private void UpdateFrameConstants(float distance)
        {

            planetRadius = Vector3.Distance(transform.position, planet.transform.position);
            rotationRadians = RotationAngle(distance, planetRadius);

            moveChord = MovementChord(rotationRadians, planetRadius);

            horizontal = HorizontalMovement(rotationRadians, planetRadius);
            vertical = VerticalMovement(horizontal, moveChord);


            //Math calculation for moving on a sphere

            float RotationAngle(float moveDistance, float radius)
                => moveDistance / radius;

            float HorizontalMovement(float rotationRadians, float radius)
                => Mathf.Abs(Mathf.Sin(rotationRadians) * radius);

            float MovementChord(float rotationRadians, float radius)
                => Mathf.Abs(2 * radius * Mathf.Sin(rotationRadians / 2));

            float VerticalMovement(float horizontalMove, float moveChord)
                => Mathf.Abs(Mathf.Sqrt((moveChord * moveChord) - (horizontalMove * horizontalMove)));
        }

        private void MoveVertically(float distance)
        {
            bool rightWasHit = Physics.Raycast(GroundRayOrigin() + transform.right * _colliderRadius, -transform.up, out var hitRight, distance + skinWidth);
            bool leftWasHit = Physics.Raycast(GroundRayOrigin() - transform.right * _colliderRadius, -transform.up, out var hitLeft, distance + skinWidth);
            
            if (rightWasHit || leftWasHit)
                distance = Mathf.Clamp(Mathf.Min(hitRight.distance, hitLeft.distance) - skinWidth, 0, distance);

            transform.position += -transform.up * distance;

            Vector3 GroundRayOrigin()
        => transform.position
        + transform.up * (groundRayOffset + skinWidth);
        }
        public void MoveHorizontally(Vector2 direction, float distance)
        {
            if (distance == 0 || direction.magnitude == 0)
                return;

            int moveSteps = 0;
            localDirection = transform.TransformDirection(new Vector3(direction.x, 0, direction.y)).normalized;

            StartMoveSteps(localDirection * horizontal);


            if (moveSteps > 3)
                Debug.Log(moveSteps);

            void StartMoveSteps(Vector3 moveVector)
            {
                if (moveSteps >= MAX_MOVE_STEPS)
                    return;
                if (moveVector.magnitude < 0.01f)
                    return;

                moveSteps++;

                //The distance the move vector takes inside the character skin
                Vector3 nextMoveVector = Vector3.zero;
                if (RayCastForward(moveVector, out var hit, out var skinDistance))
                {
                    var hitDistance = Mathf.Max(hit.distance - skinDistance, 0);
                    distance -= hitDistance;
                    //Apply
                    transform.position += Vector3.ClampMagnitude(moveVector, hitDistance);

                    var slopeAngle = Vector3.Angle(hit.normal, PlanetUp);
                    if (slopeAngle <= maxSlopeAngle)
                    {
                        nextMoveVector = (Quaternion.AngleAxis(90, Vector3.Cross(hit.normal, moveVector)) * hit.normal).normalized * distance;
                    }
                    else
                    {
                        var leftoverMove = moveVector.normalized * distance;

                        var rightVector = Vector3.Project(leftoverMove, transform.right);
                        if (RayCastForward(rightVector, out hit, out skinDistance))
                            nextMoveVector += (hit.point - transform.position).normalized * Mathf.Max(hit.distance - skinDistance, 0);

                        var forwardVector = Vector3.Project(leftoverMove, transform.forward);
                        if (RayCastForward(forwardVector, out hit, out skinDistance))
                            nextMoveVector += (hit.point - transform.position).normalized * Mathf.Max(hit.distance - skinDistance, 0);
                        transform.position += nextMoveVector;
                        return;
                    }

                    if (nextMoveVector.magnitude != 0)
                        StartMoveSteps(nextMoveVector);
                    //horizontal = Mathf.Min(horizontal, hit.distance - colliderWidth);
                }
                else
                {
                    transform.position += moveVector;
                    return;
                }


                bool RayCastForward(Vector3 move, out RaycastHit raycastHit, out float skinDistance)
                    => Physics.Raycast(ForwardRayOrigin(), move, out raycastHit, move.magnitude + (skinDistance = SkinDistance(move)));
            }
            Vector3 ForwardRayOrigin()
                => transform.position
                + transform.up * Mathf.Sign(forwardRayOffset) * (Mathf.Abs(forwardRayOffset) - skinWidth)
                + localDirection * (_colliderRadius - skinWidth);
        }
        void ApplyMove(Vector3 movement) => transform.position += movement;
        float SkinDistance(Vector3 direction) => Mathf.Abs(skinWidth / Mathf.Cos(90 - Vector3.Angle(direction, transform.up)));
        Vector3 PlanetUp => (transform.position - planet.transform.position).normalized;

        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(transform.position + transform.up * (groundRayOffset + skinWidth), 0.05f);
            Gizmos.DrawSphere(transform.position + transform.up * forwardRayOffset, 0.05f);
            Gizmos.color = Color.red;
        }
    }
}