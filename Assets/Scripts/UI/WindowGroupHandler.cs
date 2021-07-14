using Assets.StateMachine;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spacetaurant.UI
{
    public class WindowGroupHandler : UiWindow_EnableDisable
    {
        #region Serielized

        #region States
        [FoldoutGroup("States"), ValueDropdown("statesDropdown", AppendNextDrawer = true, DisableGUIInAppendedDrawer = true)]
        [SerializeField] private MenuUiState _defaultState;

        [FoldoutGroup("States"), ListDrawerSettings(Expanded = true)]
        [SerializeField] private List<MenuUiState> states;
        #endregion

        #endregion
        private IEnumerable statesDropdown => states.ConvertAll((x) => new ValueDropdownItem<MenuUiState>(x.Name, x));

        #region State
        private StateMachine<MenuUiState> stateMachine;
        public MenuUiState CurrentState
        {
            get => stateMachine.State;
            set
            {
                if (CurrentState == null && value == null)
                    return;
                if (CurrentState != null && value != null && CurrentState.uiWindow == value.uiWindow)
                    return;

                stateMachine.State = value;

                OnStateChange();
            }
        }
        private bool _initiated = false;
        #endregion

        #region Unity callbacks
        private void Awake()
        {
            Init();
        }
        public override void Init()
        {
            if (!_initiated)
            {
                _initiated = true;
                if (GroupHandler != null)
                    base.Init();

                foreach (var state in states)
                {
                    state.uiWindow.GroupHandler = this;
                    state.uiWindow.Init();
                }

                stateMachine = new StateMachine<MenuUiState>(_defaultState);
                CurrentState.uiWindow.CancelTween();
                CurrentState.uiWindow.ForceTransitionedIn();
            }
        }
        #endregion

        #region State select
        public void SelectWindow(UiWindow window)
        {
            if (GroupHandler != null && !Selected)
                SelectWindow();

            CurrentState = states.Find((x) => x.uiWindow == window);
        }
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
