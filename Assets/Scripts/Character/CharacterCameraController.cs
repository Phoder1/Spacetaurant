using CustomAttributes;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class CharacterCameraController : MonoBehaviour
{
    [SerializeField]
    private GameObject _planet;
    [SerializeField, LocalComponent(getComponentFromChildrens: true)]
    private Camera _cam;
    [SerializeField]
    private Transform _target;

    [SerializeField, SuffixLabel("S"), Tooltip("Approximately the time it will take to reach the target. A smaller value will reach the target faster.")]
    private float _positionSmootheTime;
    [SerializeField, SuffixLabel("Uu")]
    private float _maxMoveSpeed = 10;
    [SerializeField, SuffixLabel("angles/sec")]
    private float _maxRotationSpeed = 10;

    private Vector3 _speed;




    private void Start()
    {
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
        transform.position = Vector3.SmoothDamp(transform.position, _target.position, ref _speed, _positionSmootheTime, _maxMoveSpeed, Time.deltaTime);
        var planetUp = (transform.position - _planet.transform.position).normalized;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.FromToRotation(transform.up, planetUp) * transform.rotation, _maxRotationSpeed * Time.deltaTime);
    }
}
