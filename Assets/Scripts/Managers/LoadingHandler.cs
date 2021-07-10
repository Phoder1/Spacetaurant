using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Spacetaurant
{
    [DefaultExecutionOrder(999)]
    public class LoadingHandler : MonoBehaviour, IReadyable
    {
        [SerializeField]
        private UnityEvent OnReadyEvent;

        public static event Action CacheAllData;
        public event Action OnReady;

        private readonly List<IReadyable> ReadyCheck = new List<IReadyable>();
        private bool _ready = false;
        public bool Ready => _ready;

        private void Awake() => CacheAllData?.Invoke();
        private void Start()
        {
            GetComponents(ReadyCheck);
            ReadyCheck.Remove(this);

            DontDestroyOnLoad(gameObject);

            StartCoroutine(WaitForReady());
        }
        private IEnumerator WaitForReady()
        {
            if (ReadyCheck != null && ReadyCheck.Count > 0)
                yield return new WaitUntil(() => ReadyCheck.TrueForAll((x) => x.Ready || x == null));

            _ready = true;
            OnReady?.Invoke();
            OnReadyEvent?.Invoke();


            Destroy(gameObject);
        }
        private void OnDestroy() => StopAllCoroutines();
    }
    public interface IReadyable
    {
        bool Ready { get; }
        event Action OnReady;
    }
}