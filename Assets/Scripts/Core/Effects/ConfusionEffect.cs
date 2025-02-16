using UnityEngine;
using System.Collections.Generic;
using RPGMinesweeper.Effects;
using RPGMinesweeper.Grid;

namespace RPGMinesweeper.Effects
{
    public class ConfusionEffect : IPassiveEffect
    {
        #region Private Fields
        private readonly float m_Duration;
        private readonly float m_Radius;
        private readonly GridShape m_Shape;
        private readonly HashSet<Vector2Int> m_AffectedCells;
        #endregion

        #region Public Properties
        public EffectType Type => EffectType.Confusion;
        public EffectTargetType TargetType => EffectTargetType.Grid;
        public float Duration => m_Duration;
        #endregion

        public ConfusionEffect(float duration, float radius, GridShape shape = GridShape.Square)
        {
            m_Duration = duration;
            m_Radius = radius;
            m_Shape = shape;
            m_AffectedCells = new HashSet<Vector2Int>();
        }

        #region Public Methods
        public void Apply(GameObject source, Vector2Int sourcePosition)
        {
            var gridManager = GameObject.FindFirstObjectByType<GridManager>();
            if (gridManager == null) return;

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

        public void Remove(GameObject source, Vector2Int sourcePosition)
        {
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

        public void OnTick(GameObject source, Vector2Int sourcePosition)
        {
            Apply(source, sourcePosition);
        }
        #endregion
    }
} 