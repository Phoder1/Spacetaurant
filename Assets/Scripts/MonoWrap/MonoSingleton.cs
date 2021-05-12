using UnityEngine;

public class MonoSingleton<T> : MonoWrap where T : MonoSingleton<T>
{
    [SerializeField]
    private bool _dontDestroyOnLoad;

    public static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<T>();
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
    protected override void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            if (_dontDestroyOnLoad)
                DontDestroyOnLoad(this);
        }
        else if (_instance != this as T)
            Destroy(gameObject);

        base.Awake();
    }
}
