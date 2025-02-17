using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using RPGMinesweeper.Grid;

namespace RPGMinesweeper.Effects
{
    public abstract class BaseEffect : IMultiModeEffect
    {
        protected EffectType m_CurrentMode;
        protected readonly HashSet<Vector2Int> m_AffectedCells = new();
        
        public virtual string Name => GetType().Name.Replace("Effect", "");
        public EffectType Type => m_CurrentMode;
        
        // By default, only support one mode. Effects can override this.
        public virtual EffectType[] SupportedTypes => new[] { m_CurrentMode };

        public virtual void SetMode(EffectType mode)
        {
            if (Array.IndexOf(SupportedTypes, mode) != -1)
            {
                m_CurrentMode = mode;
            }
            else
            {
                Debug.LogWarning($"Effect {Name} does not support mode {mode}. Using default mode {m_CurrentMode}");
            }
        }

        public virtual void Apply(GameObject target, Vector2Int sourcePosition)
        {
            switch (m_CurrentMode)
            {
                case EffectType.Persistent:
                    if (this is IPersistentEffect persistent)
                        ApplyPersistent(target, sourcePosition);
                    else
                        Debug.LogWarning($"{Name} does not implement IPersistentEffect but was used in Persistent mode");
                    break;
                    
                case EffectType.Triggerable:
                    if (this is ITriggerableEffect triggerable)
                        ApplyTriggerable(target, sourcePosition);
                    else
                        Debug.LogWarning($"{Name} does not implement ITriggerableEffect but was used in Triggerable mode");
                    break;
            }
        }

        // Protected virtual methods that derived classes can override
        protected virtual void ApplyPersistent(GameObject target, Vector2Int sourcePosition)
        {
            Debug.LogWarning($"{Name} does not implement persistent behavior");
        }

        protected virtual void ApplyTriggerable(GameObject target, Vector2Int sourcePosition)
        {
            Debug.LogWarning($"{Name} does not implement triggerable behavior");
        }

        // Helper methods for common effect functionality
        protected void RegisterEffectInArea(Vector2Int sourcePosition, float radius, GridShape shape)
        {
            var gridManager = GameObject.FindFirstObjectByType<GridManager>();
            if (gridManager == null) return;

            var affectedPositions = GridShapeHelper.GetAffectedPositions(sourcePosition, shape, Mathf.RoundToInt(radius));
            foreach (var pos in affectedPositions)
            {
                if (gridManager.IsValidPosition(pos))
                {
                    m_AffectedCells.Add(pos);
                    MineValueModifier.RegisterEffect(pos, this);
                }
            }
        }

        protected void UnregisterEffectFromArea()
        {
            foreach (var pos in m_AffectedCells)
            {
                MineValueModifier.UnregisterEffect(pos, this);
            }
            m_AffectedCells.Clear();
        }

        protected void PropagateValueChanges()
        {
            var gridManager = GameObject.FindFirstObjectByType<GridManager>();
            var mineManager = GameObject.FindFirstObjectByType<MineManager>();
            if (gridManager != null && mineManager != null)
            {
                MineValuePropagator.PropagateValues(mineManager, gridManager);
            }
        }
    }
} 