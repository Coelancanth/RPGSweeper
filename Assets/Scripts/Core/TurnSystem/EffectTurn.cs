using UnityEngine;

namespace RPGMinesweeper.TurnSystem
{
    public class EffectTurn : ITurn
    {
        #region Private Fields
        private readonly Vector2Int m_Position;
        private bool m_IsComplete;
        private float m_ElapsedTime;
        private const float c_EffectDuration = 0.3f; // Time to show effect animation
        #endregion

        #region Public Properties
        public bool IsComplete => m_IsComplete;
        #endregion

        #region Constructor
        public EffectTurn(Vector2Int position)
        {
            m_Position = position;
        }
        #endregion

        #region ITurn Implementation
        public void Begin()
        {
            m_ElapsedTime = 0f;
            // Setup effect animations or visual feedback
        }

        public void Update()
        {
            m_ElapsedTime += Time.deltaTime;
            if (m_ElapsedTime >= c_EffectDuration)
            {
                m_IsComplete = true;
            }
        }

        public void End()
        {
            // Cleanup effect-specific states or animations
        }
        #endregion
    }
} 