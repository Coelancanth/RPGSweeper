using UnityEngine;
using RPGMinesweeper.Grid;
using RPGMinesweeper.States;

namespace RPGMinesweeper.Effects
{
    public class UnfreezeEffect : BaseEffect, IPersistentEffect, ITriggerableEffect
    {
        #region Private Fields
        private readonly int m_Radius;
        private readonly GridShape m_Shape;
        private bool m_IsActive;
        private StateManager m_StateManager;
        private Vector2Int m_CurrentPosition;
        #endregion

        #region Public Properties
        public override EffectType[] SupportedTypes => new[] { EffectType.Persistent, EffectType.Triggerable };
        public bool IsActive => m_IsActive;
        public EffectType Type => EffectType.Triggerable;
        public string Name => "Unfreeze";
        #endregion

        public UnfreezeEffect(int radius, GridShape shape = GridShape.Square)
        {
            m_Radius = radius;
            m_Shape = shape;
            m_IsActive = false;
            m_CurrentMode = EffectType.Persistent;
        }

        #region Protected Methods
        protected override void ApplyPersistent(GameObject target, Vector2Int sourcePosition)
        {
            m_IsActive = true;
            EnsureStateManager(target);
            RemoveFrozenStates(target, sourcePosition);
        }

        protected override void ApplyTriggerable(GameObject target, Vector2Int sourcePosition)
        {
            m_IsActive = true;
            EnsureStateManager(target);
            RemoveFrozenStates(target, sourcePosition);
        }
        #endregion

        #region Public Methods
        public void Update(float deltaTime)
        {
            // State updates are handled by StateManager
        }

        public void Remove(GameObject target)
        {
            m_IsActive = false;
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

        private void RemoveFrozenStates(GameObject target, Vector2Int sourcePosition)
        {
            m_CurrentPosition = sourcePosition;
            
            // Get affected positions based on shape and radius
            var affectedPositions = GridShapeHelper.GetAffectedPositions(sourcePosition, m_Shape, m_Radius);
            
            // Remove frozen states from all affected positions
            foreach (var position in affectedPositions)
            {
                m_StateManager.RemoveState((Name: "Frozen", Target: StateTarget.Cell, TargetId: position));
            }
        }
        #endregion
    }
} 