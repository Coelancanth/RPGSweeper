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
        private bool m_DebugMode = false;
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
            EnsureStateManager();
            RemoveFrozenStates(target, sourcePosition);
        }

        protected override void ApplyTriggerable(GameObject target, Vector2Int sourcePosition)
        {
            m_IsActive = true;
            EnsureStateManager();
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
        private void EnsureStateManager()
        {
            if (m_StateManager == null)
            {
                // Try to find existing StateManager
                m_StateManager = GameObject.FindFirstObjectByType<StateManager>();
                
                // If none exists, create a new one on a dedicated GameObject
                if (m_StateManager == null)
                {
                    var stateManagerObj = new GameObject("StateManager");
                    m_StateManager = stateManagerObj.AddComponent<StateManager>();
                    if (m_DebugMode)
                    {
                        Debug.Log("[UnfreezeEffect] Created new StateManager");
                    }
                }
            }
        }

        private void RemoveFrozenStates(GameObject target, Vector2Int sourcePosition)
        {
            m_CurrentPosition = sourcePosition;
            
            // Get affected positions based on shape and radius
            var affectedPositions = GridShapeHelper.GetAffectedPositions(sourcePosition, m_Shape, m_Radius);
            
            if (m_DebugMode)
            {
                Debug.Log($"[UnfreezeEffect] Removing frozen states in radius {m_Radius} at {sourcePosition}");
            }
            
            // Remove frozen states from all affected positions
            foreach (var position in affectedPositions)
            {
                m_StateManager.RemoveState((Name: "Frozen", Target: StateTarget.Cell, TargetId: position));
                if (m_DebugMode)
                {
                    Debug.Log($"[UnfreezeEffect] Removed frozen state at {position}");
                }
            }
        }
        #endregion
    }
} 