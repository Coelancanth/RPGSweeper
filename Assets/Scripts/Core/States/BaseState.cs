using UnityEngine;

namespace RPGMinesweeper.States
{
    public abstract class BaseState : IState
    {
        #region Protected Fields
        protected float m_ElapsedTime;
        protected float m_Duration;
        protected string m_Name;
        #endregion

        #region Public Properties
        public string Name => m_Name;
        public float Duration => m_Duration;
        public virtual bool IsExpired => m_ElapsedTime >= m_Duration;
        #endregion

        #region Constructor
        protected BaseState(string name, float duration)
        {
            m_Name = name;
            m_Duration = duration;
            m_ElapsedTime = 0f;
        }
        #endregion

        #region IState Implementation
        public virtual void Enter(GameObject target)
        {
            m_ElapsedTime = 0f;
        }

        public virtual void Update(float deltaTime)
        {
            m_ElapsedTime += deltaTime;
        }

        public virtual void Exit(GameObject target)
        {
            // Base cleanup logic
        }
        #endregion
    }
} 