using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rose.Daphne.ARCollect.Utility
{
    public class EventToggler : MonoBehaviour
    {
        public enum EventTogglerState
        {
            OFF,
            ON
        }

        [Serializable] public class EventTogglerEvent : UnityEvent { }

        [SerializeField]
        private EventTogglerEvent eventToggledON = null;

        [SerializeField]
        private EventTogglerEvent eventToggledOFF = null;

        [SerializeField]
        private EventTogglerState startState = EventTogglerState.OFF;

        [SerializeField]
        private bool triggerStartStateEventOnStart = false;

        internal EventTogglerState CurrentState { private set; get; } = EventTogglerState.OFF;

        private void Awake()
        {
            CurrentState = startState;
        }

        private void Start()
        {
            if (triggerStartStateEventOnStart)
            {
                if (CurrentState == EventTogglerState.OFF)
                    eventToggledOFF?.Invoke();
                else if (CurrentState == EventTogglerState.ON)
                    eventToggledON?.Invoke();
            }
        }

        public void Toggle()
        {
            ToggleInternal();
        }

        internal EventTogglerState ToggleInternal()
        {
            if (CurrentState == EventTogglerState.ON)
            {
                CurrentState = EventTogglerState.OFF;
                eventToggledOFF?.Invoke();
            }
            else if (CurrentState == EventTogglerState.OFF)
            {
                CurrentState = EventTogglerState.ON;
                eventToggledON?.Invoke();
            }

            return CurrentState;
        }

        public void ToggleOn()
        {
            if (CurrentState != EventTogglerState.ON)
            {
                CurrentState = EventTogglerState.ON;
                eventToggledON?.Invoke();
            }
        }

        public void ToggleOff()
        {
            if (CurrentState != EventTogglerState.OFF)
            {
                CurrentState = EventTogglerState.OFF;
                eventToggledOFF?.Invoke();
            }
        }

        internal void Subscribe(EventTogglerState state, Action action)
        {
            if (state == EventTogglerState.OFF)
            {
                eventToggledOFF.AddListener(() => action());
            }
            else if (state == EventTogglerState.ON)
            {
                eventToggledON.AddListener(() => action());
            }
        }

        internal void Unsubscribe(EventTogglerState state, Action action)
        {
            if (state == EventTogglerState.OFF)
            {
                eventToggledOFF.RemoveListener(() => action());
            }
            else if (state == EventTogglerState.ON)
            {
                eventToggledON.RemoveListener(() => action());
            }
        }

        internal void UnsubscribeAll(EventTogglerState state)
        {
            if (state == EventTogglerState.OFF)
            {
                eventToggledOFF.RemoveAllListeners();
            }
            else if (state == EventTogglerState.ON)
            {
                eventToggledON.RemoveAllListeners();
            }
        }
    }
}


