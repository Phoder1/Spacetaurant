using Refrences;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class MonoWrap : MonoBehaviour
{
    [SerializeField]
    private ObjectRefrence _refrence;
    [Space(order = 998)]
    [SerializeField, PropertyOrder(999)]
    protected bool _debug;

    protected virtual void Awake()
    {
        if (_refrence != null)
            _refrence.Value = this;

        OnAwake();
    }
    protected virtual void OnAwake() { }
}
