using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using CustomAttributes;
using Spacetaurant;


//This script continually aligns the rigidbody it is attached to toward a target transform;
//As a result, the rigidbody's 'up' direction will always point away from the target;
//It can be used to align a controller toward the center of a planet, for games featuring planetary gravity;
public class AlignRigidbodyToPlanet : MonoBehaviour {
    [SerializeField, ReadOnly, Tooltip("Target transform used for alignment")]
    private Transform planet;
    [SerializeField, LocalComponent]
    private Rigidbody r;

	// Use this for initialization
	void Start () {
        planet = BlackBoard.planet.transform;
        r = GetComponent<Rigidbody>();

        if(planet == null)
        {
            Debug.LogWarning("No target has been assigned.", this);
            enabled = false;
        }
	}
	
	void FixedUpdate () {

        //Get this transform's 'forward' direction;
        Vector3 _forwardDirection = transform.forward;

        //Calculate new 'up' direction;
        Vector3 _newUpDirection = (transform.position - planet.position).normalized;

        //Calculate rotation between this transform's current 'up' direction and the new 'up' direction;
        Quaternion _rotationDifference = Quaternion.FromToRotation(transform.up, _newUpDirection);
        //Apply the rotation to this transform's 'forward' direction;
        Vector3 _newForwardDirection = _rotationDifference * _forwardDirection;

        //Calculate final new rotation and set this rigidbody's rotation;
        Quaternion _newRotation = Quaternion.LookRotation(_newForwardDirection, _newUpDirection);
        r.MoveRotation(_newRotation);
    }
}
