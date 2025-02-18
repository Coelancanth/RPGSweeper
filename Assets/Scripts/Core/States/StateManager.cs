using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace RPGMinesweeper.States
{
    public class StateManager : MonoBehaviour
    {
        #region Private Fields
        private Dictionary<(string Name, StateTarget Target, object TargetId), IState> m_ActiveStates = 
            new Dictionary<(string Name, StateTarget Target, object TargetId), IState>();
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
            var key = (state.Name, state.Target, state.TargetId);
            if (m_ActiveStates.ContainsKey(key))
            {
                RemoveState(key);
            }

            m_ActiveStates[key] = state;
            state.Enter(gameObject);
            OnStateAdded?.Invoke(state);
        }

        public void RemoveState((string Name, StateTarget Target, object TargetId) key)
        {
            if (m_ActiveStates.TryGetValue(key, out IState state))
            {
                state.Exit(gameObject);
                m_ActiveStates.Remove(key);
                OnStateRemoved?.Invoke(state);
            }
        }

        public bool HasState(string stateName, StateTarget target, object targetId)
        {
            return m_ActiveStates.ContainsKey((stateName, target, targetId));
        }

        public void OnTurnEnd()
        {
            var expiredStates = new List<(string Name, StateTarget Target, object TargetId)>();
            
            foreach (var kvp in m_ActiveStates)
            {
                if (kvp.Value is ITurnBasedState turnState)
                {
                    turnState.OnTurnEnd();
                    if (turnState.IsExpired)
                    {
                        expiredStates.Add(kvp.Key);
                    }
                }
            }

            foreach (var key in expiredStates)
            {
                RemoveState(key);
            }
        }

        public IEnumerable<IState> GetStatesForTarget(StateTarget target, object targetId)
        {
            return m_ActiveStates
                .Where(kvp => kvp.Key.Target == target && Equals(kvp.Key.TargetId, targetId))
                .Select(kvp => kvp.Value);
        }
        #endregion

        #region Private Methods
        private void UpdateStates()
        {
            var expiredStates = new List<(string Name, StateTarget Target, object TargetId)>();
            
            foreach (var kvp in m_ActiveStates)
            {
                if (!(kvp.Value is ITurnBasedState)) // Only update time-based states
                {
                    kvp.Value.Update(Time.deltaTime);
                    if (kvp.Value.IsExpired)
                    {
                        expiredStates.Add(kvp.Key);
                    }
                }
            }

            foreach (var key in expiredStates)
            {
                RemoveState(key);
            }
        }
        #endregion
    }
} 