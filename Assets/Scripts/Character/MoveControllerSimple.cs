using CustomAttributes;
using PowerGamers.Misc;
using System;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(CapsuleCollider))]
public class MoveControllerSimple : MonoBehaviour
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

    public event Action<Vector3> OnMove = default;

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
    private void Update()
    {
        //moveDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        moveDir = new Vector2(variableJoystick.Horizontal, variableJoystick.Vertical).normalized;
    }
    private void FixedUpdate()
    {

        float distance = Time.fixedDeltaTime * _speed;
        UpdateFrameConstants(distance);
        localDirection = transform.TransformDirection(new Vector3(moveDir.x, 0, moveDir.y)).normalized;

        transform.position += localDirection * horizontal;
        transform.rotation = Quaternion.FromToRotation(transform.up, PlanetUp(transform.position)) * transform.rotation;
        transform.position += -transform.up * vertical;

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
    Vector3 PlanetUp(Vector3 pos) => (pos - planet.transform.position).normalized;

}
