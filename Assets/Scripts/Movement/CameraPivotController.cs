using CustomAttributes;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace Spacetaurant
{
    public class CameraPivotController : MonoBehaviour
    {
        #region Serielized
        [SerializeField, LocalComponent(getComponentFromChildrens: true)]
        private Camera _cam;
        [SerializeField]
        private Transform _target;

        [SerializeField, SuffixLabel("Uu")]
        private float _maxMoveSpeed = 10;

        [FoldoutGroup("Rotation")]
        [InfoBox("The rotation speed at 0 degrees", InfoMessageType = InfoMessageType.None)]
        [SerializeField, SuffixLabel("angles/sec")]
        private float _minRotationSpeed = 2;

        [FoldoutGroup("Rotation")]
        [InfoBox("The rotation speed at 180 degrees", InfoMessageType = InfoMessageType.None)]
        [SerializeField, SuffixLabel("angles/sec")]
        private float _maxRotationSpeed = 15;

        [FoldoutGroup("Rotation")]
        [InfoBox("X axis is degrees from target rotation, Y axis is rotation speed", InfoMessageType.None)]
        [SerializeField]
        private AnimationCurve _rotationSpeedCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [SerializeField, FoldoutGroup("Rotation")]
        private float _maxRotationAcceleration = 5;

        [InfoBox("In degress per screen width drag.", InfoMessageType.None)]
        [SerializeField]
        private float _touchRotationSensetivity = 90;
        #endregion

        #region State
        float _savedRotationSpeed = 0;

        Coroutine _lateFixedUpdateRoutine;

        [SerializeField, ReadOnly]
        Vector2 _totalDragOffset = Vector2.zero;
        #endregion

        private void Awake()
        {
            transform.parent = null;

        }
        private void OnEnable()
        {
            _lateFixedUpdateRoutine = StartCoroutine(LateFixedUpdateRoutine());
        }
        private void OnDisable()
        {
            if (_lateFixedUpdateRoutine != null)
            {
                StopCoroutine(_lateFixedUpdateRoutine);
                _lateFixedUpdateRoutine = null;
            }
        }
        private IEnumerator LateFixedUpdateRoutine()
        {
            while (isActiveAndEnabled)
            {
                yield return new WaitForFixedUpdate();
                LateFixedUpdate();
            }
        }
        // LateFixedUpdate is called at the end of every FixedUpdate
        private void LateFixedUpdate()
        {
            MoveToTarget();

            var planetUp = (transform.position - BlackBoard.planet.transform.position).normalized;

            AlignToPlanet(planetUp);

            RotateToTarget(planetUp);
        }

        private void AlignToPlanet(Vector3 planetUp)
            => transform.rotation = Quaternion.FromToRotation(transform.up, planetUp) * transform.rotation;

        private void RotateToTarget(Vector3 planetUp)
        {
            Quaternion offsetRotation = Quaternion.Euler(planetUp * (_totalDragOffset.x / Screen.width) * _touchRotationSensetivity);
            Quaternion targetRotation = offsetRotation * Quaternion.LookRotation(_target.forward, planetUp);

            var _targetRotationSpeed = Mathf.Lerp(_minRotationSpeed, _maxRotationSpeed, _rotationSpeedCurve.Evaluate(Quaternion.Angle(transform.rotation, targetRotation) / 180));
            var _frameAccel = _maxRotationAcceleration * Time.deltaTime;
            var _rotationSpeed = Mathf.Clamp(_targetRotationSpeed, _savedRotationSpeed - _frameAccel, _savedRotationSpeed + _frameAccel);


            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

            _savedRotationSpeed = _rotationSpeed;
        }

        private void MoveToTarget()
        {
            Vector3 _movement = Vector3.ClampMagnitude(_target.position - transform.position, _maxMoveSpeed);
            transform.position += _movement;
        }
        public void DragOffset(Vector2 dragDelta)
        {
            _totalDragOffset += dragDelta;
        }
        public void StopDrag()
        {

        }
    }
}
