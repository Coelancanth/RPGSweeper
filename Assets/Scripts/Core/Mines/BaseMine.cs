using UnityEngine;

namespace RPGMinesweeper.Core.Mines
{
    public abstract class BaseMine : IMine
    {
        #region Protected Fields
        protected readonly MineData m_Data;
        protected readonly Vector2Int m_Position;
        protected bool m_IsDestroyed;
        #endregion

        #region Public Properties
        public MineType Type => m_Data.Type;
        public virtual bool CanDisguise => false;
        public MineData GetMineData() => m_Data;
        #endregion

        #region Constructor
        protected BaseMine(MineData _data, Vector2Int _position)
        {
            m_Data = _data;
            m_Position = _position;
            m_IsDestroyed = false;
        }
        #endregion

        #region Public Methods
        public virtual void OnTrigger(Player _player)
        {
            if (m_IsDestroyed) return;

            foreach (var effect in m_Data.Effects)
            {
                ApplyEffect(effect);
            }

            OnDestroy();
        }

        public virtual void OnDestroy()
        {
            m_IsDestroyed = true;
        }
        #endregion

        #region Protected Methods
        protected virtual int CalculateDamage(Player _player)
        {
            return m_Data.Value;
        }

        protected virtual void ApplyEffect(EffectData _effect)
        {
            GameEvents.RaiseEffectApplied(m_Position);
        }
        #endregion
    }
} 