using UnityEngine;
using RPGMinesweeper.Grid;
using RPGMinesweeper.States;
using System.Collections.Generic;

namespace RPGMinesweeper.Effects
{
    public class TeleportEffect : BaseEffect, IPersistentEffect
    {
        #region Private Fields
        private readonly int m_Duration;
        private readonly int m_Radius;
        private readonly GridShape m_Shape;
        private bool m_IsActive;
        private StateManager m_StateManager;
        private Vector2Int m_CurrentPosition;
        private bool m_DebugMode = false;
        #endregion

        #region Public Properties
        public override EffectType[] SupportedTypes => new[] { EffectType.Persistent };
        public bool IsActive => m_IsActive;
        public override string Name => "Teleport";
        #endregion

        public TeleportEffect(int duration, int radius, GridShape shape = GridShape.Square)
        {
            m_Duration = duration;
            m_Radius = radius;
            m_Shape = shape;
            m_IsActive = false;
            m_CurrentMode = EffectType.Persistent;
        }

        #region Protected Methods
        protected override void ApplyPersistent(GameObject target, Vector2Int sourcePosition)
        {
            m_IsActive = true;
            m_CurrentPosition = sourcePosition;
            EnsureStateManager();
            ApplyTeleportState(target, sourcePosition);
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
                m_StateManager = GameObject.FindFirstObjectByType<StateManager>();
                if (m_StateManager == null)
                {
                    Debug.LogError("[TeleportEffect] Could not find StateManager!");
                }
            }
        }

        private void ApplyTeleportState(GameObject target, Vector2Int sourcePosition)
        {
            //Debug.Log($"[TeleportEffect] Applying teleport state at {sourcePosition} with radius {m_Radius}, duration {m_Duration} turns");
            var state = new TeleportState(m_Duration, m_Radius, sourcePosition, m_Shape);
            m_StateManager.AddState(state);
            
            if (m_DebugMode)
            {
                Debug.Log($"[TeleportEffect] Applied teleport state at {sourcePosition} with radius {m_Radius}, duration {m_Duration} turns");
            }
        }
        #endregion
    }
} 