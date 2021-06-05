using Assets.StateMachine;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using PowerGamers.Misc;

namespace Spacetaurant.UI
{
    public class WindowGroupHandler : MonoBehaviour
    {
        #region Serielized

        #region States
        [FoldoutGroup("States") ,ValueDropdown("statesDropdown", AppendNextDrawer = true, DisableGUIInAppendedDrawer = true)]
        [SerializeField] private MenuUiState _defaultState;

        [FoldoutGroup("States"), ListDrawerSettings(Expanded = true)]
        [SerializeField] private List<MenuUiState> states;
        #endregion

        #endregion
        private IEnumerable statesDropdown => states.ConvertAll((x) => new ValueDropdownItem<MenuUiState>(x.Name, x));
        #region Events
        [SerializeField, FoldoutGroup("Events")]
        private UnityEvent OnUiLock;
        [SerializeField, FoldoutGroup("Events")]
        private UnityEvent OnUiUnlock;
        #endregion

        #region State
        private Tween transitionTween;
        private StateMachine<MenuUiState> stateMachine;
        public MenuUiState CurrentState
        {
            get => stateMachine.State;
            set
            {
                if (CurrentState.uiWindow == value.uiWindow)
                    return;

                value.uiWindow.groupHandler = this;

                stateMachine.State = value;

                OnStateChange();
            }
        }
        private bool _uiLocked;
        public bool UiLocked
        {
            get => _uiLocked;
            set
                => Misc.Setter(ref _uiLocked, value, () =>
                {
                    if (_uiLocked)
                        OnUiLock?.Invoke();
                    else
                        OnUiUnlock?.Invoke();
                });
        }
        #endregion

        #region Unity callbacks
        private void Start()
        {
            states.ForEach((x) => x.uiWindow.groupHandler = this);
            _defaultState.uiWindow.groupHandler = this;
            stateMachine = new StateMachine<MenuUiState>(_defaultState);
            CurrentState.uiWindow.CancelTween();
            CurrentState.uiWindow.ForceTransitionedIn();
        }
        #endregion

        #region State select
        public void SelectWindow(UiWindow window) => CurrentState = states.Find((x) => x.uiWindow == window);
        #endregion
        private void OnStateChange()
        {
            UiLocked = true;

            if (CurrentState != null && CurrentState.uiWindow != null)
            {
                if (CurrentState.uiWindow.transitionTween != null)
                    CurrentState.uiWindow.transitionTween.onComplete = OnComplete;
                else
                    OnComplete();
            }

            void OnComplete()
            {
                UiLocked = false;
            }
        }
        [Serializable]
        [InlineProperty]
        public class MenuUiState : State
        {
            [Required, HideLabel]
            public UiWindow uiWindow;
            public string Name => (uiWindow == null) ? "Unassigned window" : uiWindow.name.SplitCamelCase();
            protected override void OnEnable()
            {
                uiWindow.gameObject.SetActive(true);
                uiWindow.TransitionIn();
            }

            protected override void OnDisable()
                => uiWindow.TransitionOut();
            protected override void OnUpdate() { }
        }
    }
}
