using UnityEngine;
using RPGMinesweeper.Grid;
using RPGMinesweeper.States;

namespace RPGMinesweeper.Effects
{
    public class UnfreezeEffect : BaseEffect
    {
        #region Private Fields
        private readonly int m_Radius;
        private readonly GridShape m_Shape;
        private StateManager m_StateManager;
        #endregion

        #region Public Properties
        public override EffectType[] SupportedTypes => new[] { EffectType.Triggerable };
        #endregion

        public UnfreezeEffect(int radius, GridShape shape = GridShape.Square)
        {
            m_Radius = radius;
            m_Shape = shape;
            m_CurrentMode = EffectType.Triggerable;
        }

        #region Protected Methods
        protected override void ApplyTriggerable(GameObject target, Vector2Int sourcePosition)
        {
            EnsureStateManager(target);
            var affectedPositions = GridShapeHelper.GetAffectedPositions(sourcePosition, m_Shape, m_Radius);
            
            foreach (var pos in affectedPositions)
            {
                if (m_StateManager.HasState("Frozen", StateTarget.Cell, pos))
                {
                    m_StateManager.RemoveState(("Frozen", StateTarget.Cell, pos));
                }
            }
        }
        #endregion

        #region Private Methods
        private void EnsureStateManager(GameObject target)
        {
            if (m_StateManager == null)
            {
                m_StateManager = target.GetComponent<StateManager>();
                if (m_StateManager == null)
                {
                    m_StateManager = target.AddComponent<StateManager>();
                }
            }
        }
        #endregion
    }
} 