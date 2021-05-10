using Refrences;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class MonoWrap : MonoBehaviour
{
    [SerializeField, FoldoutGroup("Refrence", AnimateVisibility = false, Expanded = false)]
    private ObjectRefrence _refrence;

    protected virtual void Awake()
    {
        if (_refrence != null)
            _refrence.Value = this;

        OnAwake();
    }
    protected virtual void OnAwake() { }
}
