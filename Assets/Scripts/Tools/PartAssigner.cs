using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Spacetaurant.Tools
{
    [ExecuteInEditMode]
    public class PartAssigner : MonoBehaviour
    {
        const float RaycastExtraHeight = 5;
        [Flags] private enum Modes { AutoRotation = 1, AutoFlat = 2}
        [SerializeField, EnumToggleButtons]
        private Modes _workModes = Modes.AutoRotation;

        [SerializeField]
        private LayerMask layer;
#if UNITY_EDITOR
        private void Awake()
        {
                ApplyModes();
        }
#endif
#if UNITY_EDITOR
        private Vector3 _lastPos;

        private RaycastHit? _hit;
        private void Update()
        {
            if (!Application.isPlaying && _lastPos != transform.position)
                ApplyModes();
        }
        public void ApplyModes()
        {
            if (!CastRay(out _hit))
                return;

            if (_workModes.HasFlag(Modes.AutoRotation))
                Rotate();

            if (_workModes.HasFlag(Modes.AutoFlat))
                Flatten();

            _hit = null;

            _lastPos = transform.position;
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

            var mesh = GetComponentInChildren<MeshFilter>()?.sharedMesh;
            if (mesh == null)
            {
                mesh = GetComponentInChildren<SkinnedMeshRenderer>()?.sharedMesh;
                if (mesh == null)
                    _workModes &= ~Modes.AutoFlat;
            }
            var closestPoint = mesh.bounds.ClosestPoint(_hit.Value.point);
            transform.position += _hit.Value.point - (transform.position - closestPoint);
        }
        private bool CastRay(out RaycastHit? hit)
        {
            hit = default;

            PlanetAssigner planet = FindObjectOfType<PlanetAssigner>();

            if (planet == null)
                return false;

            var distance = Vector3.Distance(transform.position, planet.transform.position);
            Vector3 planetDirection = (planet.transform.position - transform.position).normalized;
            Ray ray = new Ray(transform.position - planetDirection * RaycastExtraHeight, planetDirection);
            var success = Physics.Raycast(ray, out var tempHit, distance, layer, QueryTriggerInteraction.Collide);
            hit = (RaycastHit?)tempHit;
            return success;
        }
#endif
    }
}
