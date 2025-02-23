using UnityEngine;

namespace RPGMinesweeper.States
{
    public abstract class BaseState : IState
    {
        #region Protected Fields
        protected int m_ElapsedTurn;
        protected float m_Duration;
        protected string m_Name;
        protected StateTarget m_Target;
        protected object m_TargetId;
        #endregion

        #region Public Properties
        public string Name => m_Name;
        public float Duration => m_Duration;
        public virtual bool IsExpired => m_ElapsedTurn >= m_Duration;
        public StateTarget Target => m_Target;
        public object TargetId => m_TargetId;
        #endregion

        #region Constructor
        protected BaseState(string name, int duration, StateTarget target, object targetId)
        {
            m_Name = name;
            m_Duration = duration;
            m_ElapsedTurn = 0;
            m_Target = target;
            m_TargetId = targetId;
        }
        #endregion

        #region IState Implementation
        public virtual void Enter(GameObject target)
        {
            m_ElapsedTurn = 0;
        }

        public virtual void Update(float deltaTime)
        {
            m_ElapsedTurn += 1;
        }

        public virtual void Exit(GameObject target)
        {
            // Base cleanup logic
        }
        #endregion
    }
} 