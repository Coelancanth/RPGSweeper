using UnityEngine;

namespace RPGMinesweeper.TurnSystem
{
    public class PlayerTurn : ITurn
    {
        #region Private Fields
        private readonly Vector2Int m_Position;
        private bool m_IsComplete;
        #endregion

        #region Public Properties
        public bool IsComplete => m_IsComplete;
        #endregion

        #region Constructor
        public PlayerTurn(Vector2Int position)
        {
            m_Position = position;
        }
        #endregion

        #region ITurn Implementation
        public void Begin()
        {
            // Player turn begins immediately when cell is revealed
            // No additional setup needed
        }

        public void Update()
        {
            // Player turn completes immediately after cell reveal
            m_IsComplete = true;
        }

        public void End()
        {
            // Notify that player's action is complete
            GameEvents.RaiseRoundAdvanced();
        }
        #endregion
    }
} 