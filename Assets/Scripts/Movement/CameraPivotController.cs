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
        private Transform _cameraPivot;
        [SerializeField]
        private Transform _target;

        #region Movement
        [SerializeField, SuffixLabel("Uu/s"), FoldoutGroup("Movement"), MinValue(0)]
        private float _minMoveSpeed = 2;
        [SerializeField, SuffixLabel("Uu/s"), FoldoutGroup("Movement"),MinValue(0)]
        private float _maxMoveSpeed = 5;
        [InfoBox("X axis is the distance from the target, Y axis is the speed", InfoMessageType.None)]
        [SerializeField, FoldoutGroup("Movement")]
        private AnimationCurve _movementSpeedCurve = AnimationCurve.Linear(0,0,1,1);
        [InfoBox("The distance from the target, Y axis is the speed", InfoMessageType.None)]
        [SerializeField, SuffixLabel("Uu"), FoldoutGroup("Movement"), MinValue(0)]
        private float _maxMoveSpeedDistance = 5;
        [SerializeField, SuffixLabel("Uu/s^2"), FoldoutGroup("Movement"), MinValue(0)]
        private float _maxMoveAcceleration = 2;
        #endregion

        #region Rotation to target
        [InfoBox("The rotation speed at 0 degrees", InfoMessageType = InfoMessageType.None)]
        [SerializeField, SuffixLabel("angles/sec"), FoldoutGroup("Player Rotation"), MinValue(0)]
        private float _minRotationSpeed = 2;

        [InfoBox("The rotation speed at 180 degrees", InfoMessageType = InfoMessageType.None)]
        [SerializeField, SuffixLabel("angles/sec"), FoldoutGroup("Player Rotation"), MinValue(0)]
        private float _maxRotationSpeed = 15;

        [InfoBox("X axis is degrees from target rotation, Y axis is rotation speed", InfoMessageType.None)]
        [SerializeField, FoldoutGroup("Player Rotation")]
        private AnimationCurve _rotationSpeedCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [SerializeField, FoldoutGroup("Player Rotation"), MinValue(0)]
        private float _maxRotationAcceleration = 5;
        #endregion

        #region Touch control
        [InfoBox("In degress per screen width drag.", InfoMessageType.None)]
        [SerializeField, FoldoutGroup("Touch control"), MinValue(0)]
        private float _touchRotationSensetivity = 90;
        [SerializeField, FoldoutGroup("Touch control"), MinValue(0)]
        private bool _mirrorDragControl = false;
        [SerializeField, FoldoutGroup("Touch control"), MinValue(0)]
        private float _maxHorizontalTouchRotation = 90;
        [SerializeField, FoldoutGroup("Touch control"), MinValue(0)]
        private float _maxVerticalTouchRotation = 30;
        [SerializeField, FoldoutGroup("Touch control"), MinValue(0)]
        private float _dragRotationDecayDelay = 0;
        [SerializeField, FoldoutGroup("Touch control")]
        private AnimationCurve _decayCurve;
        [SerializeField, FoldoutGroup("Touch control"), MinValue(0)]
        private float _maxDecaySpeed = 5;
        #endregion
        #endregion

        #region State
        float _savedRotationSpeed = 0;

        Coroutine _lateFixedUpdateRoutine;

        [SerializeField, ReadOnly]
        Vector2 _totalDragOffset = Vector2.zero;

        Coroutine _decayRoutine = null;

        public Vector2 TotalDragOffset
        {
            get => _totalDragOffset;
            set
            {
                _totalDragOffset = value;
                var dragX = Mathf.Clamp(TotalDragOffset.x, -_maxHorizontalTouchRotation, _maxHorizontalTouchRotation);
                var dragY = Mathf.Clamp(TotalDragOffset.y, -_maxVerticalTouchRotation, _maxVerticalTouchRotation);
                _totalDragOffset = new Vector2(dragX, dragY);
            }
        }
        float _speed = 0;
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
            Quaternion targetRotation = Quaternion.LookRotation(_target.forward, planetUp);

            var _targetRotationSpeed = Mathf.Lerp(_minRotationSpeed, _maxRotationSpeed, _rotationSpeedCurve.Evaluate(Quaternion.Angle(transform.rotation, targetRotation) / 180));
            var _frameAccel = _maxRotationAcceleration * Time.deltaTime;
            var _rotationSpeed = Mathf.Clamp(_targetRotationSpeed, _savedRotationSpeed - _frameAccel, _savedRotationSpeed + _frameAccel);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

            _cameraPivot.localRotation = Quaternion.Euler(Vector3.up * TotalDragOffset.x);
            _savedRotationSpeed = _rotationSpeed;
        }

        private void MoveToTarget()
        {
            float distance = Vector3.Distance(transform.position, _target.position);
            float curveValue = Mathf.Clamp01(_movementSpeedCurve.Evaluate(distance / _maxMoveSpeedDistance));
            float frameAcceleration = _maxMoveAcceleration * Time.deltaTime;
            _speed = Mathf.Min(Mathf.Lerp(_minMoveSpeed, _maxMoveSpeed, curveValue), _speed + frameAcceleration);

            Vector3 _movement = Vector3.ClampMagnitude(_target.position - transform.position, _speed * Time.deltaTime);
            transform.position += _movement;
        }
        public void DragOffset(Vector2 dragDelta)
        {
            if (_decayRoutine != null)
                StopCoroutine(_decayRoutine);

            TotalDragOffset += (_mirrorDragControl ? -1 : 1) * -(dragDelta / Screen.width) * _touchRotationSensetivity;
        }
        public void StopDrag()
        {
            _decayRoutine = StartCoroutine(DecayDragRoutine());
        }
        IEnumerator DecayDragRoutine()
        {
            if (_dragRotationDecayDelay > 0)
                yield return new WaitForSeconds(_dragRotationDecayDelay);

            while (isActiveAndEnabled)
            {
                yield return null;

                var xRotationDelta = Mathf.Abs(_totalDragOffset.x);
                var curveValueX = 0f;
                if (_totalDragOffset.x != 0f)
                    curveValueX = _decayCurve.Evaluate(xRotationDelta / _maxHorizontalTouchRotation);

                var decaySpeedX = -Mathf.Sign(_totalDragOffset.x) * Mathf.Min(curveValueX * _maxDecaySpeed, xRotationDelta);

                var yRotationDelta = Mathf.Abs(_totalDragOffset.y);
                var curveValueY = 0f;
                if (_totalDragOffset.y != 0f)
                    curveValueY = _decayCurve.Evaluate(yRotationDelta / _maxVerticalTouchRotation);

                var decaySpeedY = -Mathf.Sign(_totalDragOffset.y) * Mathf.Min(curveValueY * _maxDecaySpeed, yRotationDelta);

                Vector2 decaySpeed = new Vector2(decaySpeedX, decaySpeedY);

                TotalDragOffset += decaySpeed * Time.deltaTime;
            }

        }
    }
}
