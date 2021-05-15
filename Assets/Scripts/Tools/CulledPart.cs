using CustomAttributes;
using UnityEngine;

namespace Spacetaurant
{
    [RequireComponent(typeof(Collider))]
    public class CulledPart : MonoBehaviour
    {
        [HideInInspector]
        public Collider partCollider;
        [HideInInspector]
        public Vector3 center;
        private void Awake()
        {
            partCollider = GetComponent<Collider>();

            center = partCollider.bounds.center;
            Sphere_Culling.sphereParts.Add(this);
        }
        private void OnDestroy()
        {
            Sphere_Culling.sphereParts.Remove(this);
        }
    }
}
