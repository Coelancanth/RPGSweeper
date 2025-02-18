using UnityEngine;

namespace RPGMinesweeper.States
{
    public abstract class BaseTurnState : BaseState, ITurnBasedState
    {
        #region Private Fields
        private int m_TurnsRemaining;
        #endregion

        #region Public Properties
        public int TurnsRemaining => m_TurnsRemaining;
        public override bool IsExpired => m_TurnsRemaining <= 0;
        #endregion

        #region Constructor
        protected BaseTurnState(string name, int turns) : base(name, float.MaxValue)
        {
            m_TurnsRemaining = turns;
        }
        #endregion

        #region Public Methods
        public override void Update(float deltaTime)
        {
            // Do nothing - we don't use time-based updates
        }

        public virtual void OnTurnEnd()
        {
            m_TurnsRemaining--;
        }
        #endregion
    }
} 