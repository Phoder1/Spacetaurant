using UnityEngine;
using Sirenix.OdinInspector;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    [SerializeField, PropertyOrder(999)]
    private bool _dontDestroyOnLoad = true;

    public static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    var singletonObj = new GameObject();
                    singletonObj.name = typeof(T).ToString();
                    _instance = singletonObj.AddComponent<T>();

                    Debug.LogWarning("Singleton was loaded automatically, manual refrencing in scene is prefered.");
                }
            }
            return _instance;
        }
    }
    protected void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            if (_dontDestroyOnLoad)
                DontDestroyOnLoad(this);
        }
        else if (_instance != this as T)
            Destroy(gameObject);
    }
}
