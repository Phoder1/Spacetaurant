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

    [SerializeField, SuffixLabel("Uu")]
    private float _maxMoveSpeed = 10;
    [SerializeField, SuffixLabel("angles/sec")]
    private float _maxRotationSpeed = 10;

    [SerializeField]
    private float minMovement = 0.1f;




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
        Vector3 _movement = Vector3.ClampMagnitude(_target.position - transform.position, _maxMoveSpeed);
        if (_movement.sqrMagnitude > minMovement)
            transform.position += _movement;
        var planetUp = (transform.position - _planet.transform.position).normalized;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.FromToRotation(transform.up, planetUp) * transform.rotation, _maxRotationSpeed * Time.deltaTime);
    }
}
