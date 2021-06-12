using UnityEngine;

namespace PowerGamers.Misc
{
    public static class SphereTools
    {
        public static Vector3 WorldDirectionToPoint(Vector3 originPoint, Vector3 targetPoint, Transform transform)
            => Vector3.ProjectOnPlane(targetPoint - originPoint, transform.up).normalized;
        public static Vector3 WorldDirectionToPoint(Transform originTransform, Vector3 targetPoint)
            => WorldDirectionToPoint(originTransform.position, targetPoint, originTransform);
        public static Vector2 LocalDirectionToPoint(Vector3 originPoint, Vector3 targetPoint, Transform transform)
        {
            Vector3 localDirection = transform.InverseTransformDirection(WorldDirectionToPoint(originPoint, targetPoint, transform));
            return new Vector2(localDirection.x, localDirection.z);
        }
        public static Vector2 LocalDirectionToPoint(Transform originTransform, Vector3 targetPoint)
            => LocalDirectionToPoint(originTransform.position, targetPoint, originTransform);
        public static float GetDistance(Vector3 origin, Vector3 posOnSphere, float radius)
            => GetSphereDistanceToPoint(Vector3.Distance(origin, posOnSphere), radius);
        public static float GetSphereDistanceToPoint(float distance, float radius)
            => 2 * radius * Mathf.Asin(distance / 2 * radius);
    }
}
