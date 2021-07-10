using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Spacetaurant
{
    public enum SceneLoadMode { NextInBuild = 0, ByName = 1, ByIndex = 2 }
    public enum SceneUnloadMode { LoadFinished = 0, Delay = 1, IReadyable = 2, Manually = 3 }
    public class SceneLoader : MonoBehaviour
    {
        [BoxGroup("Load")]
        [SerializeField, EnumToggleButtons]
        private LoadSceneMode _loadSceneMode;

        [BoxGroup("Load")]
        [SerializeField, EnumToggleButtons]
        private SceneLoadMode _loadMode;

        [BoxGroup("Load")]
        [Tooltip("Whether the sceneloader should be able to load multiple scenes at the same time.")]
        [SerializeField]
        private bool _lockAtLoad = true;

        [BoxGroup("Load")]
        [SerializeField, ShowIf("@_loadMode == SceneLoadMode.ByName"), ValueDropdown("ScenesNames"), InspectorName("Scene"), ValidateInput("@_sceneName != string.Empty")]
        private string _sceneName;

        [BoxGroup("Load")]
        [SerializeField, ShowIf("@_loadMode == SceneLoadMode.ByIndex"), ValueDropdown("ScenesIndexes"), InspectorName("Scene"), ValidateInput("@_sceneIndex >= 0")]
        private int _sceneIndex;


        [BoxGroup("Unload", VisibleIf = "@_loadSceneMode == LoadSceneMode.Additive")]
        [SerializeField, EnumToggleButtons]
        private SceneUnloadMode _sceneUnloadMode;

        [BoxGroup("Unload")]
        [SerializeField, MinValue(0), ShowIf("@_sceneUnloadMode == SceneUnloadMode.Delay"), SuffixLabel("S")]
        private float _delay;

        [BoxGroup("Unload")]
        [SerializeField, ShowIf("@_sceneUnloadMode == SceneUnloadMode.IReadyable"), ValidateInput("@_readyable is IReadyable")]
        private Component _readyable;

        [BoxGroup("Unload")]
        [Tooltip("Recocking means to enable ")]
        [SerializeField, ShowIf("_lockAtLoad")]
        private bool _unlockAfterUnload = true;
        #region Events
        [SerializeField, EventsGroup]
        private UnityEvent OnStartLoadingScene;
        [SerializeField, EventsGroup]
        private UnityEvent OnFinishedLoadingScene;
        [SerializeField, EventsGroup]
        private UnityEvent OnStartUnloadingScene;
        [SerializeField, EventsGroup]
        private UnityEvent OnFinishedUnloadingScene;
        #endregion

        private bool _locked = false;
        #region SceneLoad
        public void LoadScene()
        {
            switch (_loadMode)
            {
                case SceneLoadMode.NextInBuild:
                    LoadNextScene();
                    break;
                case SceneLoadMode.ByName:
                    LoadSceneByName();
                    break;
                case SceneLoadMode.ByIndex:
                    LoadSceneByIndex();
                    break;
            }
        }
        private void LoadSceneByIndex() => LoadSceneByIndex(_sceneIndex, _loadSceneMode);
        public void LoadSceneByIndex(int buildIndex, LoadSceneMode loadSceneMode)
        {
            if (_locked)
                return;
            var scene = SceneManager.GetSceneByBuildIndex(buildIndex);
            StartCoroutine(LoadSceneRoutine(scene.name, loadSceneMode));
        }
        private void LoadSceneByName() => LoadSceneByName(_sceneName, _loadSceneMode);
        public void LoadSceneByName(string sceneName, LoadSceneMode loadSceneMode)
        {
            if (_locked)
                return;

            StartCoroutine(LoadSceneRoutine(sceneName, loadSceneMode));
        }
        private void LoadNextScene() => LoadNextScene(_loadSceneMode);
        public void LoadNextScene(LoadSceneMode loadSceneMode)
        {
            if (_locked)
                return;

            StartCoroutine(LoadSceneRoutine(SceneManager.GetActiveScene().buildIndex + 1, loadSceneMode));
        }
        private IEnumerator LoadSceneRoutine(string sceneName, LoadSceneMode loadSceneMode)
        {
            if (!_locked)
            {
                yield return null;

                OnStartLoadingScene?.Invoke();

                if (_lockAtLoad)
                    _locked = true;

                yield return SceneManager.LoadSceneAsync(sceneName, loadSceneMode);

                OnFinishedLoadingScene?.Invoke();

                if (loadSceneMode == LoadSceneMode.Additive && _sceneUnloadMode != SceneUnloadMode.Manually)
                    UnloadScene();
            }
        }
        private IEnumerator LoadSceneRoutine(int sceneIndex, LoadSceneMode loadSceneMode)
        {
            if (!_locked)
            {
                yield return null;

                OnStartLoadingScene?.Invoke();

                if (_lockAtLoad)
                    _locked = true;

                yield return SceneManager.LoadSceneAsync(sceneIndex, loadSceneMode);

                OnFinishedLoadingScene?.Invoke();

                if (loadSceneMode == LoadSceneMode.Additive && _sceneUnloadMode != SceneUnloadMode.Manually)
                    UnloadScene();
            }
        }
        #endregion
        #region Unload

        private void UnloadScene()
        {
            switch (_sceneUnloadMode)
            {
                case SceneUnloadMode.LoadFinished:
                    UnloadActiveScene();
                    break;
                case SceneUnloadMode.Delay:
                    if (_delay <= 0)
                        UnloadActiveScene();
                    else
                        Invoke(nameof(UnloadActiveScene), _delay);
                    break;
                case SceneUnloadMode.IReadyable:
                    if (_readyable != null && _readyable is IReadyable readyable)
                        if (readyable.Ready)
                            UnloadActiveScene();
                        else
                            readyable.OnReady += UnloadActiveScene;
                    else
                        UnloadActiveScene();
                    break;
            }
        }
        private void UnloadActiveScene()
        {
            StartCoroutine(UnloadActiveSceneRoutine());
        }
        private IEnumerator UnloadActiveSceneRoutine()
        {
            OnStartUnloadingScene?.Invoke();

            var _activeScene = SceneManager.GetActiveScene();

            yield return SceneManager.UnloadSceneAsync(_activeScene);

            if (_unlockAfterUnload)
                _locked = false;

            OnFinishedUnloadingScene?.Invoke();
        }
        #endregion
#if UNITY_EDITOR

        private string[] ScenesNames
        {
            get
            {
                var scenes = UnityEditor.EditorBuildSettings.scenes;
                string[] sceneNames = new string[scenes.Length];

                for (int i = 0; i < scenes.Length; i++)
                    sceneNames[i] = GetFileName(scenes[i].path);

                return sceneNames;
            }
        }
        private int[] ScenesIndexes
        {
            get
            {
                int[] sceneIndexes = new int[UnityEditor.EditorBuildSettings.scenes.Length];

                for (int i = 0; i < sceneIndexes.Length; i++)
                    sceneIndexes[i] = i;

                return sceneIndexes;
            }
        }
        private string GetFileName(string path)
        {
            var splitPath = path.Split('/');
            splitPath = splitPath[splitPath.Length - 1].Split('.');
            return splitPath[splitPath.Length - 2];
        }
        public void UnlockLoader() => _locked = false;
#endif
    }
    public interface IClickable : IEventSystemHandler
    {

    }
}
