using CustomAttributes;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace Spacetaurant
{
    public class CameraPivotAlign : MonoBehaviour
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
        private AnimationCurve _rotationSpeedCurve = AnimationCurve.Linear(0,0,1,1);

        [SerializeField, FoldoutGroup("Rotation")]
        private float _maxRotationAcceleration = 5;
        #endregion

        #region State
        float _savedRotationSpeed = 0;
        #endregion

        private void Start()
        {
            transform.parent = null;
            StartCoroutine(LateFixedUpdateRoutine());

        }
        private IEnumerator LateFixedUpdateRoutine()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();
                LateFixedUpdate();
            }
        }
        // Update is called once per frame
        private void LateFixedUpdate()
        {
            MoveToTarget();

            var planetUp = (transform.position - BlackBoard.planet.transform.position).normalized;

            //Align to planet
            transform.rotation = Quaternion.FromToRotation(transform.up, planetUp) * transform.rotation;


            Quaternion targetRotation = Quaternion.LookRotation(_target.forward, planetUp);

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
    }
}
