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
        [SerializeField] private bool m_DebugMode = true;
        #endregion

        #region Events
        public event System.Action<IState> OnStateAdded;
        public event System.Action<IState> OnStateRemoved;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            GameEvents.OnRoundAdvanced += HandleRoundAdvanced;
            if (m_DebugMode)
            {
                Debug.Log("[StateManager] Initialized and subscribed to round events");
            }
        }

        private void OnDestroy()
        {
            GameEvents.OnRoundAdvanced -= HandleRoundAdvanced;
        }

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
            
            if (m_DebugMode)
            {
                Debug.Log($"[StateManager] Added state {state.Name} for target {state.Target} at {state.TargetId}");
            }
        }

        public void RemoveState((string Name, StateTarget Target, object TargetId) key)
        {
            if (m_ActiveStates.TryGetValue(key, out IState state))
            {
                if (m_DebugMode)
                {
                    Debug.Log($"[StateManager] Removing state {state.Name} for target {state.Target} at {state.TargetId}");
                }
                
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
            if (m_DebugMode)
            {
                Debug.Log($"[StateManager] Processing turn end for {m_ActiveStates.Count} states");
            }

            // Create a safe copy of the states to process
            var statesToProcess = m_ActiveStates.ToList();
            var expiredStates = new List<(string Name, StateTarget Target, object TargetId)>();
            
            try
            {
                foreach (var kvp in statesToProcess)
                {
                    if (kvp.Value is ITurnBasedState turnState)
                    {
                        try
                        {
                            if (m_DebugMode)
                            {
                                Debug.Log($"[StateManager] Processing turn end for state {turnState.Name} at {turnState.TargetId}");
                            }
                            
                            turnState.OnTurnEnd();
                            
                            if (turnState.IsExpired)
                            {
                                expiredStates.Add(kvp.Key);
                                if (m_DebugMode)
                                {
                                    Debug.Log($"[StateManager] State {turnState.Name} at {turnState.TargetId} has expired");
                                }
                            }
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogError($"[StateManager] Error processing turn end for state {turnState.Name}: {e}");
                            // Add to expired states to remove problematic state
                            expiredStates.Add(kvp.Key);
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[StateManager] Error during turn end processing: {e}");
            }

            // Remove expired states
            foreach (var key in expiredStates)
            {
                try
                {
                    if (m_DebugMode)
                    {
                        Debug.Log($"[StateManager] Removing expired state {key.Name}");
                    }
                    RemoveState(key);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[StateManager] Error removing expired state {key.Name}: {e}");
                }
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
        private void HandleRoundAdvanced()
        {
            if (m_DebugMode)
            {
                Debug.Log("[StateManager] Round advanced, processing turn-based states");
            }
            OnTurnEnd();
        }

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
                        if (m_DebugMode)
                        {
                            Debug.Log($"[StateManager] Time-based state {kvp.Value.Name} at {kvp.Value.TargetId} has expired");
                        }
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