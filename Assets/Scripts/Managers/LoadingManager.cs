using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Spacetaurant
{
    [DefaultExecutionOrder(999)]
    public class LoadingManager : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent OnStartLoadingScene;
        [SerializeField]
        private UnityEvent OnFinishedLoadingScene;
        [SerializeField]
        private UnityEvent OnStartUnloadingCurrentScene;
        [SerializeField]
        private UnityEvent OnFinishedUnloadingCurrentScene;

        public static event Action CacheAllData;

        private int _currentSceneIndex;
        private bool _loadedNextScene = false;
        private void Start()
        {
            _currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            StartCoroutine(RealTimeHandler.UpdateStartTime((x) => LoadNextScene()));
            CacheAllData?.Invoke();

            DontDestroyOnLoad(gameObject);
        }
        public void LoadNextScene() => StartCoroutine(LoadNextSceneRoutine());
        private IEnumerator LoadNextSceneRoutine()
        {

            yield return null;
            if (!_loadedNextScene)
            {
                OnStartLoadingScene?.Invoke();
                _loadedNextScene = true;
                yield return SceneManager.LoadSceneAsync(_currentSceneIndex + 1, LoadSceneMode.Additive);
                OnFinishedLoadingScene?.Invoke();
            }
        }
        public void UnloadScene() => StartCoroutine(UnloadCurrentScene());
        private IEnumerator UnloadCurrentScene()
        {
            OnStartUnloadingCurrentScene?.Invoke();

            yield return SceneManager.UnloadSceneAsync(_currentSceneIndex);

            OnFinishedUnloadingCurrentScene?.Invoke();

            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
