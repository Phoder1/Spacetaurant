using UnityEngine;

namespace PowerGamers.Misc
{
    public static class SphereFuncs
    {
        public static Vector3 DirectionToPoint(Transform originTransform, Vector3 targetPoint)
        {
            return Vector3.ProjectOnPlane(targetPoint - originTransform.position, originTransform.up).normalized;
        }

    }
}
