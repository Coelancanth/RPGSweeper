using UnityEngine;
using System.Collections.Generic;

namespace RPGMinesweeper.States
{
    public class StateManager : MonoBehaviour
    {
        #region Private Fields
        private Dictionary<string, IState> m_ActiveStates = new Dictionary<string, IState>();
        #endregion

        #region Events
        public event System.Action<IState> OnStateAdded;
        public event System.Action<IState> OnStateRemoved;
        #endregion

        #region Unity Lifecycle
        private void Update()
        {
            UpdateStates();
        }
        #endregion

        #region Public Methods
        public void AddState(IState state)
        {
            if (m_ActiveStates.ContainsKey(state.Name))
            {
                RemoveState(state.Name);
            }

            m_ActiveStates[state.Name] = state;
            state.Enter(gameObject);
            OnStateAdded?.Invoke(state);
        }

        public void RemoveState(string stateName)
        {
            if (m_ActiveStates.TryGetValue(stateName, out IState state))
            {
                state.Exit(gameObject);
                m_ActiveStates.Remove(stateName);
                OnStateRemoved?.Invoke(state);
            }
        }

        public bool HasState(string stateName)
        {
            return m_ActiveStates.ContainsKey(stateName);
        }
        #endregion

        #region Private Methods
        private void UpdateStates()
        {
            List<string> expiredStates = new List<string>();
            
            foreach (var state in m_ActiveStates.Values)
            {
                state.Update(Time.deltaTime);
                if (state.IsExpired)
                {
                    expiredStates.Add(state.Name);
                }
            }

            foreach (var stateName in expiredStates)
            {
                RemoveState(stateName);
            }
        }
        #endregion
    }
} 