using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Refrences;
using Sirenix.OdinInspector;

public abstract class MonoWrap : MonoBehaviour
{
    [SerializeField]
    private bool _refrenceToObject;
    [SerializeField, ShowIf("refrenceToObject")]
    private ObjectRefrence _refrence;

    protected virtual void Awake()
    {
        if (_refrenceToObject)
            _refrence.Value = this;

        OnAwake();
    }
    protected virtual void OnAwake() { }
}
