using Refrences;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class MonoWrap : MonoBehaviour
{
    [SerializeField, FoldoutGroup("Wrap", order: 999, Expanded = false)]
    private ObjectRefrence _refrence;
    [Space(order = 998)]
    [SerializeField, FoldoutGroup("Wrap", order: 999, Expanded = false)]
    protected bool _debug;

    protected virtual void Awake()
    {
        if (_refrence != null)
            _refrence.Value = this;

        OnAwake();
    }
    protected virtual void OnAwake() { }
}
