using Assets.StateMachine;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Spacetaurant.UI
{
    public class UiManager : MonoSingleton<UiManager>
    {
        #region Serielized
        [SerializeField] private MenuUiState _hudState;
        [SerializeField] private MenuUiState _inventoryState;
        #region States
        #endregion
        #endregion

        #region Events
        [SerializeField, EventsGroup]
        private UnityEvent OnUiLock;
        [SerializeField, EventsGroup]
        private UnityEvent OnUiUnlock;
        #endregion

        #region State
        private StateMachine<MenuUiState> stateMachine;
        public MenuUiState CurrentState
        {
            get => stateMachine.State;
            set
            {
                if (CurrentState == value)
                    return;


                var lastState = CurrentState;

                stateMachine.State = value;

                OnStateChange(lastState);
            }
        }
        private bool _uiLocked;
        public bool UiLocked
        {
            get => _uiLocked;
            set
            {
                if (UiLocked == value)
                    return;

                _uiLocked = value;

                if (_uiLocked)
                    OnUiLock?.Invoke();
                else
                    OnUiUnlock?.Invoke();
            }
        }
        #endregion

        #region Unity callbacks
        private void Start()
        {
            stateMachine = new StateMachine<MenuUiState>(_hudState);
            CurrentState.gameobject.SetActive(true);
        }
        #endregion

        #region State select
        [Button]
        public void SelectInventoryState() => CurrentState = _inventoryState;
        public void SelectHudState() => CurrentState = _hudState;
        #endregion

        private void OnStateChange(MenuUiState from)
        {
            UiLocked = true;

            if (CurrentState != null)
                CurrentState.uiWindow.transitionTween.onComplete += OnComplete;

            //Vector3 uiStatePos = CurrentState.uiWindow.transform.localPosition;

            //fadeInTween = transform.DOLocalMove(-uiStatePos, transitionTime).SetEase(transitionCurve);

            //fadeInTween.OnComplete(OnComplete);

            void OnComplete()
            {
                UiLocked = false;

                if (from != null && from != CurrentState)
                    from.gameobject.SetActive(false);

                CurrentState.uiWindow.UiLocked = false;
            }
        }
        [Serializable]
        public class MenuUiState : State
        {
            [Required]
            public UiWindow uiWindow;

            protected UiManager uiManager;
            public GameObject gameobject => uiWindow.gameObject;
            protected override void OnEnable()
            {
                uiManager = Instance;
                gameobject.SetActive(true);

                uiWindow.TransitionIn();
            }
            protected override void OnDisable()
            {
                uiWindow.UiLocked = true;
                uiWindow.TransitionOut();
            }
            protected override void OnUpdate() { }
        }
    }
}

