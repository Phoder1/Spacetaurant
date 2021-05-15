using CustomAttributes;
using PowerGamers.Misc;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CapsuleCollider))]
public class MoveControllerSimple : MonoBehaviour, IMoveController
{
    [SerializeField]
    GameObject planet = default;
    [SerializeField]
    private UnityEventForRefrence OnMove = default;

    #region Frame temp
    float planetRadius = default;
    float rotationRadians = default;
    float moveChord = default;
    float horizontal = default;
    float vertical = default;
    #endregion
    public void Move(Vector2 direction, float distance)
    {
        Vector3 totalMovement = Vector3.zero;

        UpdateFrameConstants(distance);

        Vector3 localDirection = transform.TransformDirection(new Vector3(direction.x, 0, direction.y)).normalized;
        
        var worldHorizontal = localDirection * horizontal;
        totalMovement += worldHorizontal;
        transform.position += worldHorizontal;

        transform.rotation = Quaternion.FromToRotation(transform.up, PlanetUp(transform.position)) * transform.rotation;
        
        var worldVertical = -transform.up * vertical;
        totalMovement += worldVertical;
        transform.position += worldVertical;

        OnMove?.Invoke(totalMovement);
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
    Vector3 PlanetUp(Vector3 pos) => (pos - planet.transform.position).normalized;
}