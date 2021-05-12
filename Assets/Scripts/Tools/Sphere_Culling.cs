using System.Collections.Generic;
using UnityEngine;
namespace Spacetaurant
{
    public class Sphere_Culling : MonoBehaviour
    {
        public static List<CulledPart> sphereParts = new List<CulledPart>();

        [SerializeField]
        float _renderDistance;
        [SerializeField]
        float _colliderDistance;

#if UNITY_EDITOR
        [SerializeField]
        private bool _gizmos = false;
#endif
        // Start is called before the first frame update
        void Start()
        {
            Application.targetFrameRate = 300;
        }
        public void CheckParts()
        {
            foreach (CulledPart part in sphereParts)
            {
                float distance = Vector3.Distance(transform.position, part.center);

                if (distance > _renderDistance)
                    part.gameObject.SetActive(false);
                else
                {
                    part.gameObject.SetActive(true);

                    if (distance > _colliderDistance)
                    {
                        part.partCollider.enabled = false;
#if UNITY_EDITOR
                        if (_gizmos)
                            Debug.DrawLine(transform.position, part.center, Color.red);
#endif
                    }
                    else
                    {
                        part.partCollider.enabled = true;
#if UNITY_EDITOR
                        if (_gizmos)
                            Debug.DrawLine(transform.position, part.center, Color.green);
#endif
                    }
                }
            }
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!_gizmos)
                return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _renderDistance);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _colliderDistance);
        }
#endif
    }

}
