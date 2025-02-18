using UnityEngine;

namespace RPGMinesweeper.TurnSystem
{
    public class PlayerTurn : ITurn
    {
        #region Private Fields
        private readonly Vector2Int m_Position;
        private bool m_IsComplete;
        private bool m_DebugMode = true; // Enable debug logging for player turns
        private TurnManager m_TurnManager;
        #endregion

        #region Public Properties
        public bool IsComplete => m_IsComplete;
        #endregion

        #region Constructor
        public PlayerTurn(Vector2Int position)
        {
            m_Position = position;
            m_TurnManager = GameObject.FindFirstObjectByType<TurnManager>();
            if (m_DebugMode)
            {
                Debug.Log($"[PlayerTurn] Created new turn for position {position}");
            }
        }
        #endregion

        #region ITurn Implementation
        public void Begin()
        {
            if (m_DebugMode)
            {
                Debug.Log($"[PlayerTurn] Beginning turn at position {m_Position}, Turn #{m_TurnManager?.CurrentTurn ?? 0}");
            }
            
            // Player turn completes immediately when it begins
            m_IsComplete = true;
            
            // Complete the turn right away
            if (m_TurnManager != null)
            {
                m_TurnManager.CompleteTurn();
            }
            else if (m_DebugMode)
            {
                Debug.LogError("[PlayerTurn] Could not find TurnManager!");
            }
        }

        public void Update()
        {
            // Not used since we complete in Begin()
        }

        public void End()
        {
            if (m_DebugMode)
            {
                Debug.Log($"[PlayerTurn] Ending turn at position {m_Position}, Turn #{m_TurnManager?.CurrentTurn ?? 0}");
            }
            // Notify that player's action is complete
            GameEvents.RaiseRoundAdvanced();
        }
        #endregion
    }
} 