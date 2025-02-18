using UnityEngine;
using RPGMinesweeper.Grid;
using RPGMinesweeper.States;

namespace RPGMinesweeper.Effects
{
    public class FreezeEffect : BaseEffect, IPersistentEffect, ITriggerableEffect
    {
        #region Private Fields
        private readonly int m_Duration;
        private readonly int m_Radius;
        private readonly GridShape m_Shape;
        private bool m_IsActive;
        private StateManager m_StateManager;
        private Vector2Int m_CurrentPosition; // Store the current position for removal
        private bool m_DebugMode = false;
        #endregion

        #region Public Properties
        public override EffectType[] SupportedTypes => new[] { EffectType.Persistent, EffectType.Triggerable };
        public bool IsActive => m_IsActive;
        public EffectType Type => EffectType.Triggerable;
        public string Name => "Freeze";
        #endregion

        public FreezeEffect(int duration, int radius, GridShape shape = GridShape.Square)
        {
            m_Duration = duration;
            m_Radius = radius;
            m_Shape = shape;
            m_IsActive = false;
            m_CurrentMode = EffectType.Persistent; // Default to persistent mode
        }

        #region Protected Methods
        protected override void ApplyPersistent(GameObject target, Vector2Int sourcePosition)
        {
            m_IsActive = true;
            EnsureStateManager();
            ApplyFrozenState(target, sourcePosition);
        }

        protected override void ApplyTriggerable(GameObject target, Vector2Int sourcePosition)
        {
            m_IsActive = true;
            EnsureStateManager();
            ApplyFrozenState(target, sourcePosition);
        }
        #endregion

        #region Public Methods
        public void Update(float deltaTime)
        {
            // State updates are handled by StateManager
        }

        public void Remove(GameObject target)
        {
            if (m_StateManager != null && m_IsActive)
            {
                m_StateManager.RemoveState(("Frozen", StateTarget.Cell, m_CurrentPosition));
                m_IsActive = false;
            }
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
                        Debug.Log("[FreezeEffect] Created new StateManager");
                    }
                }
            }
        }

        private void ApplyFrozenState(GameObject target, Vector2Int sourcePosition)
        {
            m_CurrentPosition = sourcePosition; // Store the position for later removal
            var frozenState = new FrozenState(m_Duration, m_Radius, sourcePosition, m_Shape);
            m_StateManager.AddState(frozenState);
            if (m_DebugMode)
            {
                Debug.Log($"[FreezeEffect] Applied frozen state at {sourcePosition} with duration {m_Duration}");
            }
        }
        #endregion
    }
} 