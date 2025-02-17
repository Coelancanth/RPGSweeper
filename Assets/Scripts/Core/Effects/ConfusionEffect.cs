using UnityEngine;
using System.Collections.Generic;
using RPGMinesweeper.Grid;

namespace RPGMinesweeper.Effects
{
    public class ConfusionEffect : IPersistentEffect
    {
        #region Private Fields
        private readonly float m_Duration;
        private readonly float m_Radius;
        private readonly GridShape m_Shape;
        private readonly HashSet<Vector2Int> m_AffectedCells;
        private bool m_IsActive;
        #endregion

        #region Public Properties
        public EffectType Type => EffectType.Persistent;
        public string Name => "Confusion";
        public bool IsActive => m_IsActive;
        #endregion

        public ConfusionEffect(float duration, float radius, GridShape shape = GridShape.Square)
        {
            m_Duration = duration;
            m_Radius = radius;
            m_Shape = shape;
            m_AffectedCells = new HashSet<Vector2Int>();
            m_IsActive = false;
        }

        #region Public Methods
        public void Apply(GameObject target, Vector2Int sourcePosition)
        {
            var gridManager = GameObject.FindFirstObjectByType<GridManager>();
            if (gridManager == null) return;

            m_IsActive = true;
            var affectedPositions = GridShapeHelper.GetAffectedPositions(sourcePosition, m_Shape, Mathf.RoundToInt(m_Radius));
            
            foreach (var pos in affectedPositions)
            {
                if (gridManager.IsValidPosition(pos))
                {
                    m_AffectedCells.Add(pos);
                    MineValueModifier.RegisterEffect(pos, this);
                }
            }

            var mineManager = GameObject.FindFirstObjectByType<MineManager>();
            if (mineManager != null)
            {
                MineValuePropagator.PropagateValues(mineManager, gridManager);
            }
        }

        public void Update(float deltaTime)
        {
            // Update confusion effect (e.g., visual feedback)
        }

        public void Remove(GameObject target)
        {
            m_IsActive = false;
            var gridManager = GameObject.FindFirstObjectByType<GridManager>();
            var mineManager = GameObject.FindFirstObjectByType<MineManager>();
            if (gridManager == null || mineManager == null) return;

            foreach (var pos in m_AffectedCells)
            {
                MineValueModifier.UnregisterEffect(pos, this);
            }
            m_AffectedCells.Clear();

            MineValuePropagator.PropagateValues(mineManager, gridManager);
        }
        #endregion
    }
} 