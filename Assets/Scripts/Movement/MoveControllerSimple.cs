using CustomAttributes;
using PowerGamers.Misc;
using Sirenix.OdinInspector;
using Spacetaurant;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Spacetaurant.movement
{
    [RequireComponent(typeof(CapsuleCollider))]
    public class MoveControllerSimple : MonoBehaviour, IMoveController
    {
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
        public void ApplyGravity(float gravity)
        {
        }
        private void UpdateFrameConstants(float distance)
        {
            planetRadius = Vector3.Distance(transform.position, BlackBoard.planet.transform.position);
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
                => Mathf.Abs(Mathf.Sqrt(moveChord * moveChord - horizontalMove * horizontalMove));
        }
        Vector3 PlanetUp(Vector3 pos) => (pos - BlackBoard.planet.transform.position).normalized;

        public void RotateTowards(Vector3 pos)
        {
            throw new NotImplementedException();
        }

        public void Rotate(Quaternion rotation)
        {
            throw new NotImplementedException();
        }
    }
}