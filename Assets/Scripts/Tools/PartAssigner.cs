using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Spacetaurant.Tools
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
    public class PartAssigner : MonoBehaviour
    {
        [Flags] private enum Modes { AutoRotation = 1, AutoFlat = 2, AutoAssign = 4 }
        [SerializeField, EnumToggleButtons]
        private Modes _workModes = (Modes)~0;

        [SerializeField]
        private LayerMask layer;
        private Vector3 _lastPos;

        private RaycastHit? _hit;
        private void Update()
        {
            if (!Application.isPlaying && _lastPos != transform.position)
            {
                if (!CastRay(out _hit))
                    return;

                if (_workModes.HasFlag(Modes.AutoAssign))
                    AssignPart();

                if (_workModes.HasFlag(Modes.AutoRotation))
                    Rotate();

                if (_workModes.HasFlag(Modes.AutoFlat))
                    Flatten();

                _hit = null;

                _lastPos = transform.position;
            }
        }
        [Button, FoldoutGroup("Manual activation")]
        private void AssignPart()
        {
            if (_hit == null && !CastRay(out _hit))
                    return;

            transform.SetParent(_hit.Value.transform);
        }
        [Button, FoldoutGroup("Manual activation")]
        private void Rotate()
        {
            if (_hit == null && !CastRay(out _hit))
                    return;

            transform.rotation = Quaternion.FromToRotation(transform.up, _hit.Value.normal) * transform.rotation;
        }
        [Button, FoldoutGroup("Manual activation")]
        private void Flatten()
        {
            if (_hit == null && !CastRay(out _hit))
                    return;

            var meshFilter = GetComponent<MeshFilter>();
            var closestPoint = meshFilter.sharedMesh.bounds.ClosestPoint(_hit.Value.point);
            transform.position += _hit.Value.point - (transform.position - closestPoint);
        }
        private bool CastRay(out RaycastHit? hit)
        {
            Planet planet = FindObjectOfType<Planet>();
            var distance = Vector3.Distance(transform.position, planet.transform.position);
            Vector3 planetDirection = planet.transform.position - transform.position;
            Ray ray = new Ray(transform.position, planetDirection);
            var success = Physics.Raycast(ray, out var tempHit, distance, layer, QueryTriggerInteraction.Collide);
            hit = (RaycastHit?)tempHit;
            return success;
        }
    }
#endif
}
