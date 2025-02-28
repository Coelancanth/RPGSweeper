using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RPGMinesweeper.Grid;

namespace RPGMinesweeper.Effects
{
    public class ConfusionEffect : BaseEffect, IPersistentEffect, ITriggerableEffect
    {
        #region Private Fields
        private readonly float m_Duration;
        private readonly float m_Radius;
        private readonly GridShape m_Shape;
        private bool m_IsActive;
        #endregion

        #region Public Properties
        public override EffectType[] SupportedTypes => new[] { EffectType.Persistent, EffectType.Triggerable };
        public bool IsActive => m_IsActive;
        #endregion

        public ConfusionEffect(float duration, float radius, GridShape shape = GridShape.Square)
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
            RegisterEffectInArea(sourcePosition, m_Radius, m_Shape);
            PropagateValueChanges();
        }

        protected override void ApplyTriggerable(GameObject target, Vector2Int sourcePosition)
        {
            m_IsActive = true;
            RegisterEffectInArea(sourcePosition, m_Radius, m_Shape);
            PropagateValueChanges();
            
            // Auto-remove after duration for triggerable mode
            GameObject.FindFirstObjectByType<MonoBehaviour>()?.StartCoroutine(RemoveAfterDelay());
        }
        #endregion

        #region Public Methods
        public void Update(float deltaTime)
        {
            // Update confusion effect (e.g., visual feedback)
            if (m_CurrentMode == EffectType.Persistent)
            {
                // Add any persistent-specific update logic here
            }
        }

        public void Remove(GameObject target)
        {
            //Debug.Log("ConfusionEffect: Remove called");
            m_IsActive = false;
            UnregisterEffectFromArea();
            PropagateValueChanges();
        }
        #endregion

        #region Private Methods
        private IEnumerator RemoveAfterDelay()
        {
            yield return new WaitForSeconds(m_Duration);
            if (m_IsActive)
            {
                Remove(null);
            }
        }
        #endregion
    }
} 