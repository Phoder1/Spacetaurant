using Refrences;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectRefrencer : MonoBehaviour
{
    [SerializeField]
    private ObjectRefrence _refrence;
    protected virtual void Awake()
    {
        if (_refrence != null)
            _refrence.Value = gameObject;
    }
}
