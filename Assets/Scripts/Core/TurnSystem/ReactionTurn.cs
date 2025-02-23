using UnityEngine;

namespace RPGMinesweeper.TurnSystem
{
    public class ReactionTurn : ITurn
    {
        #region Private Fields
        private readonly MineType m_MineType;
        private bool m_IsComplete;
        private float m_ElapsedTime;
        private const float c_ReactionDuration = 0.5f; // Time to show reaction animation
        #endregion

        #region Public Properties
        public bool IsComplete => m_IsComplete;
        #endregion

        #region Constructor
        public ReactionTurn(MineType mineType)
        {
            m_MineType = mineType;
        }
        #endregion

        #region ITurn Implementation
        public void Begin()
        {
            m_ElapsedTime = 0f;
            // Additional setup for reaction animations or effects can be added here
        }

        public void Update()
        {
            m_ElapsedTime += Time.deltaTime;
            if (m_ElapsedTime >= c_ReactionDuration)
            {
                m_IsComplete = true;
            }
        }

        public void End()
        {
            // Cleanup any reaction-specific states or animations
        }
        #endregion
    }
} 