using CustomAttributes;
using PowerGamers.Misc;
using System;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

[RequireComponent(typeof(CapsuleCollider))]
public class MoveController : MonoWrap
{
    [SerializeField]
    GameObject planet = default;
    [SerializeField]
    private float _speed = 10;
    [SerializeField, LocalComponent]
    private Rigidbody _rb = default;
    [SerializeField, LocalComponent]
    private CapsuleCollider _collider = default;
    [SerializeField]
    private float _gravity = default;
    [SerializeField]
    float maxSlopeAngle = 60;
    [SerializeField]
    float minSlopeAngle = -30;
    [SerializeField, Tooltip("The offset on the Y from which the ground ray will be shot"), SuffixLabel("Uu")]
    private float groundRayOffset = default;
    [SerializeField, Tooltip("The offset on the Y from which the forward ray will be shot"), SuffixLabel("Uu")]
    private float forwardRayOffset = default;
    [SerializeField, SuffixLabel("Uu")]
    private float skinWidth = default;

    Vector2 moveDir = Vector2.zero;
    private float _colliderRadius = default;

    public UnityEventWrap<object> OnMove = default;

    const int MAX_MOVE_STEPS = 20;


    public VariableJoystick variableJoystick;

    #region Frame temp
    float planetRadius = default;
    float rotationRadians = default;
    Vector3 localDirection = default;
    float moveChord = default;
    float horizontal = default;
    float vertical = default;

    Vector3 totalMovement = Vector3.zero;

    #endregion
    private void Start()
    {
        _colliderRadius = _collider.radius;
    }
    private void Update()
    {
        //moveDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        moveDir = new Vector2(variableJoystick.Horizontal, variableJoystick.Vertical).normalized;
#if UNITY_EDITOR
        Landmark.landmarksPositions.ForEach(DrawLine);

        void DrawLine(Landmark landmark)
        {
            var direction = SphereTools.DirectionToPoint(transform, landmark.transform.position);
            Debug.DrawLine(transform.position, transform.position + direction);
        }
#endif
    }
    private void FixedUpdate()
    {

        float distance = Time.fixedDeltaTime * _speed;
        UpdateFrameConstants(distance);

        if (moveDir != Vector2.zero)
            MoveHorizontally(moveDir, distance);

        MoveVertically(vertical);

        ApplyMove(totalMovement);
        _rb.MoveRotation(Quaternion.FromToRotation(transform.up, PlanetUp(transform.position)) * transform.rotation);
        _rb.AddForce((transform.position - planet.transform.position) * _gravity);

    }

    private void UpdateFrameConstants(float distance)
    {
        totalMovement = Vector3.zero;

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

        var vertSkinwidth = (totalMovement == Vector3.zero ? skinWidth : 0);
        if (Physics.Raycast(GroundRayOrigin(), -transform.up, out var hit, distance + vertSkinwidth))
            distance = Mathf.Clamp(hit.distance - vertSkinwidth, 0, distance);

        totalMovement += -transform.up * distance;

        Vector3 GroundRayOrigin()
            => transform.position
            + transform.up * Mathf.Sign(groundRayOffset) * (Mathf.Abs(groundRayOffset) - skinWidth)
            + totalMovement;

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
                totalMovement += Vector3.ClampMagnitude(moveVector, hitDistance);

                var slopeAngle = Vector3.Angle(hit.normal, PlanetUp(transform.position + totalMovement));
                if (moveSteps <= 1)
                    Debug.Log(slopeAngle);
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
                    totalMovement += nextMoveVector;
                    return;
                }

                if (nextMoveVector.magnitude != 0)
                    StartMoveSteps(nextMoveVector);
                //horizontal = Mathf.Min(horizontal, hit.distance - colliderWidth);
            }
            else
            {
                totalMovement += moveVector;
                return;
            }


            bool RayCastForward(Vector3 move, out RaycastHit raycastHit, out float skinDistance)
                => Physics.Raycast(ForwardRayOrigin(), move, out raycastHit, move.magnitude + (skinDistance = (totalMovement == Vector3.zero?SkinDistance(move):0)));
        }
        Vector3 ForwardRayOrigin()
            => transform.position
            + transform.up * Mathf.Sign(forwardRayOffset) * (Mathf.Abs(forwardRayOffset) - skinWidth)
            + localDirection * (_colliderRadius - skinWidth)
            + totalMovement;
    }
    void ApplyMove(Vector3 movement) => _rb.MovePosition(transform.position + movement);
    float SkinDistance(Vector3 direction) => Mathf.Abs(skinWidth / Mathf.Cos(90 - Vector3.Angle(direction, transform.up)));
    Vector3 PlanetUp(Vector3 pos) => (pos - planet.transform.position).normalized;
    

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position + transform.up * groundRayOffset, 0.05f);
        Gizmos.DrawSphere(transform.position + transform.up * forwardRayOffset, 0.05f);
        Gizmos.color = Color.red;
    }
}
