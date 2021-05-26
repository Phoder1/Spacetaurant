using UnityEngine;
using Sirenix.Serialization;
using System;

namespace Spacetaurant.movement
{
    public interface IMoveController
    {
        /// <summary>
        /// Applying a local movement relative to the planet's up direction.
        /// Should always be used in FixedUpdate!
        /// </summary>
        /// <param name="direction"> The direction relative to the planet's up direction.</param>
        /// <param name="distance">The distance to move in the direction.</param>
        void Move(Vector2 direction, float distance);
        /// <summary>
        /// Applies gravity in the direction of the planet.
        /// </summary>
        /// <param name="gravity">The amount of gravity to apply.</param>
        void ApplyGravity(float gravity);
        void RotateTowards(Vector3 pos, float? maxRotationAngle = null);
        void Rotate(Quaternion rotation, float? maxRotationAngle = null);
    }
}