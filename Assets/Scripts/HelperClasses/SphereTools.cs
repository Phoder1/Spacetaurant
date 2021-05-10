using UnityEngine;

namespace PowerGamers.Misc
{
    public static class SphereTools
    {
        public static Vector3 DirectionToPoint(Transform originTransform, Vector3 targetPoint)
            => Vector3.ProjectOnPlane(targetPoint - originTransform.position, originTransform.up).normalized;
        public static float GetDistance(Vector3 origin, Vector3 posOnSphere, float radius)
            => GetSphereDistanceToPoint(Vector3.Distance(origin, posOnSphere), radius);
        public static float GetSphereDistanceToPoint(float distance, float radius)
            => 2 * radius * Mathf.Asin(distance / 2 * radius);
    }
}
